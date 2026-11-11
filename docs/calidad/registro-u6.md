# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno. La U5 terminó con 74 tests en verde (golden master incluido).

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** `CalculoTotales`.
- **Tests que propuso Claude:** 5, en `tests/PedidosWeb.Core.Tests/Totales/CalculoTotalesBordeTests.cs`: pedido vacío, `" ofe "` en minúsculas y con espacios, solo ofertas sobre 1M, IVA en ,5 exacto (R05) y tasa de IVA 0,10. Bordes que el golden master no tiene (todas sus filas usan 0,19, `OFE` en mayúsculas y al menos una línea).
- **Cambio que hice a mano para probarlos:** una rotura por test en `CalculoTotales.cs`, restaurada después:
  1. Quitar `lineas.Count == 0 ? 0m :` del flete → cae `PedidoSinLineas` (golden master: 0 rojos).
  2. `l.Categoria.Trim()` → `l.Categoria` → cae `OfeEnMinusculasYConEspacios` (golden master: 0 rojos).
  3. `baseDescuento = subtotal` → cae `SoloOfertasSobreUnMillon` (y 17 casos del golden master).
  4. `Calc3` → `Math.Truncate(valor + 0.49m)` → cae `IvaJustoEnLaMitad` (y 13 del golden master).
  5. IVA fijo `* 0.19m` → cae `TasaIvaDistintaDe19` (golden master: 0 rojos).
- **Tests que me quedé:** los 5. Tres de ellos (1, 2 y 5) detectan errores que el golden master deja pasar.
- **Tests que borré y por qué:** ninguno: los 5 fallaron con su rotura.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** <enlace al primer run en verde>
- **`claude.yml` en `main`:** <sí / no>
- **Revisé:** permisos mínimos ☐ · timeout ☐ · versiones fijas ☐ · token solo como secreto ☐

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
