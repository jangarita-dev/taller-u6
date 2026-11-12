namespace PedidosWeb.Core.Totales;

/// <summary>Una línea del pedido, como TLineaCalculo en legado/src/UCalculoTotales.pas.</summary>
public record LineaCalculo(int Cantidad, decimal PrecioUnitario, string Categoria);

/// <summary>Resultado del cálculo, como TResultadoTotales en el legado.</summary>
public record ResultadoTotales(decimal Subtotal, decimal BaseDescuento, decimal PorcDescuento,
    decimal Descuento, decimal Flete, decimal Neto, decimal Iva, decimal Total);

/// <summary>
/// Migración de legado/src/UCalculoTotales.pas, igual al legado (defectos incluidos).
/// Cada método indica la función Delphi que reproduce.
/// </summary>
public static class CalculoTotales
{
    /// <summary>
    /// Calcula los totales del pedido igual que CalcularTotales (UCalculoTotales.pas:108).
    /// </summary>
    public static ResultadoTotales Calcular(IReadOnlyList<LineaCalculo> lineas, bool esVip,
        int region, decimal fleteBase, decimal tasaIva)
    {
        var subtotal = CalcularSubtotal(lineas);
        var baseDescuento = CalcularBaseDescuento(lineas);
        var porcDescuento = PorcentajeDescuento(baseDescuento, esVip);
        var descuento = Calc3(baseDescuento * porcDescuento);
        var neto = subtotal - descuento;
        var flete = lineas.Count == 0 ? 0m : CalcularFlete(fleteBase, region);
        var iva = CalcularIva(neto, flete, tasaIva);
        var total = neto + flete + iva;

        return new ResultadoTotales(subtotal, baseDescuento, porcDescuento,
            descuento, flete, neto, iva, total);
    }

    /// <summary>
    /// Redondeo al peso: la mitad sube (UCalculoTotales.pas:48).
    /// R05 corregido: el legado hacía Trunc(valor + 0.49), que bajaba las fracciones en [0.50, 0.51).
    /// Ver docs/migracion/defectos-corregidos.md.
    /// </summary>
    internal static decimal Calc3(decimal valor) => Math.Round(valor, 0, MidpointRounding.AwayFromZero);

    /// <summary>Suma cantidad × precio de todas las líneas (UCalculoTotales.pas:53).</summary>
    internal static decimal CalcularSubtotal(IReadOnlyList<LineaCalculo> lineas) =>
        lineas.Sum(l => l.Cantidad * l.PrecioUnitario);

    /// <summary>Suma de las líneas que entran al descuento: todas menos la categoría OFE (UCalculoTotales.pas:63).</summary>
    internal static decimal CalcularBaseDescuento(IReadOnlyList<LineaCalculo> lineas) =>
        lineas.Where(l => !string.Equals(l.Categoria.Trim(), "OFE", StringComparison.OrdinalIgnoreCase))
              .Sum(l => l.Cantidad * l.PrecioUnitario);

    /// <summary>Descuento por volumen: más de 1.000.000 → 8 %, más de 500.000 → 5 % (UCalculoTotales.pas:75).</summary>
    internal static decimal PorcentajeVolumen(decimal baseDescuento) =>
        baseDescuento > 1_000_000m ? 0.08m
        : baseDescuento > 500_000m ? 0.05m
        : 0m;

    /// <summary>Volumen + 3 % si es VIP, con tope de 10 % (UCalculoTotales.pas:85).</summary>
    internal static decimal PorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var porc = PorcentajeVolumen(baseDescuento);
        if (esVip)
            porc += 0.03m;
        return porc > 0.10m ? 0.10m : porc;
    }

    /// <summary>Flete de la región, con 15 % de recargo en las regiones 11 y 12 (UCalculoTotales.pas:95).</summary>
    internal static decimal CalcularFlete(decimal fleteBase, int region) =>
        region is 11 or 12 ? Calc3(fleteBase * 1.15m) : Calc3(fleteBase);

    /// <summary>IVA sobre neto + flete (UCalculoTotales.pas:103).</summary>
    internal static decimal CalcularIva(decimal neto, decimal flete, decimal tasa) =>
        Calc3((neto + flete) * tasa);
}
