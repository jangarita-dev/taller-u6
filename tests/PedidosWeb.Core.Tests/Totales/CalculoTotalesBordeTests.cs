using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// U6 · Tests de borde que el golden master no cubre (pedido vacío, OFE mal escrita,
/// solo ofertas, redondeo en ,5 exacto y tasa de IVA distinta de 0,19).
/// Cada valor esperado está calculado a mano: ninguno compara el resultado consigo mismo.
/// </summary>
public class CalculoTotalesBordeTests
{
    private const int RegionSinRecargo = 13;
    private const decimal FleteBase = 3500m;
    private const decimal Iva19 = 0.19m;

    [Fact]
    public void Calcular_PedidoSinLineas_TodoEnCeroYSinFlete()
    {
        var r = CalculoTotales.Calcular(new List<LineaCalculo>(), esVip: false,
            RegionSinRecargo, FleteBase, Iva19);

        Assert.Equal(new ResultadoTotales(0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m), r);
    }

    [Fact]
    public void Calcular_OfeEnMinusculasYConEspacios_NoEntraAlDescuento()
    {
        // ABA 1.000.000 + " ofe " 200.000 → base 1.000.000 (no supera 1M) → 5 %.
        var lineas = new List<LineaCalculo>
        {
            new(10, 100_000m, "ABA"),
            new(1, 200_000m, " ofe "),
        };

        var r = CalculoTotales.Calcular(lineas, esVip: false, RegionSinRecargo, FleteBase, Iva19);

        Assert.Equal(1_000_000m, r.BaseDescuento);
        Assert.Equal(0.05m, r.PorcDescuento);
        Assert.Equal(50_000m, r.Descuento);
        Assert.Equal(219_165m, r.Iva);          // (1.150.000 + 3.500) × 0,19
        Assert.Equal(1_372_665m, r.Total);
    }

    [Fact]
    public void Calcular_SoloOfertasSobreUnMillon_SinDescuento()
    {
        var lineas = new List<LineaCalculo> { new(3, 400_000m, "OFE") };

        var r = CalculoTotales.Calcular(lineas, esVip: false, RegionSinRecargo, FleteBase, Iva19);

        Assert.Equal(0m, r.BaseDescuento);
        Assert.Equal(0m, r.Descuento);
        Assert.Equal(1_200_000m, r.Neto);
        Assert.Equal(228_665m, r.Iva);          // (1.200.000 + 3.500) × 0,19
        Assert.Equal(1_432_165m, r.Total);
    }

    [Fact]
    public void Calcular_IvaJustoEnLaMitad_RedondeaHaciaArriba()
    {
        // 50 × 0,19 = 9,50 exacto → 10 (R05). El legado, con Trunc(9,50 + 0,49), daba 9.
        var lineas = new List<LineaCalculo> { new(1, 50m, "BEB") };

        var r = CalculoTotales.Calcular(lineas, esVip: false, RegionSinRecargo, fleteBase: 0m, Iva19);

        Assert.Equal(10m, r.Iva);
        Assert.Equal(60m, r.Total);
    }

    [Fact]
    public void Calcular_TasaIvaDistintaDe19_UsaLaTasaRecibida()
    {
        var lineas = new List<LineaCalculo> { new(1, 100_000m, "BEB") };

        var r = CalculoTotales.Calcular(lineas, esVip: false, RegionSinRecargo, FleteBase, tasaIva: 0.10m);

        Assert.Equal(10_350m, r.Iva);           // (100.000 + 3.500) × 0,10
        Assert.Equal(113_850m, r.Total);
    }
}
