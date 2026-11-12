# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno. La U5 terminó con 74 tests en verde (golden master incluido).

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** CalculoTotales
- **Tests que propuso Claude:** 5 (`CalculoTotalesBordeTests.cs`)
- **Cambio que hice a mano para probarlos:** en `CalculoTotales.Calcular`, `baseDescuento = subtotal` en vez de `CalcularBaseDescuento(lineas)` (las OFE entran al descuento). Fallaron `SoloOfertasSobreUnMillon` y `OfeEnMinusculasYConEspacios`. Para los otros tres, una rotura cada uno:
  - flete sin el `lineas.Count == 0 ? 0m` → falla `PedidoSinLineas_TodoEnCeroYSinFlete`
  - `Calc3` con el redondeo del legado `Math.Truncate(valor + 0.49m)` → falla `IvaJustoEnLaMitad_RedondeaHaciaArriba`
  - `CalcularIva` con `0.19m` fijo en vez de `tasa` → falla `TasaIvaDistintaDe19_UsaLaTasaRecibida`
- **Tests que me quedé:** los 5; cada uno falló ante al menos una rotura. Deshice las roturas: 79/79 en verde.
- **Tests que borré y por qué:** ninguno; no hubo un test que pasara con y sin su cambio.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** https://github.com/jangarita-dev/taller-u6/actions/runs/43213781851 (79/79 en verde)
- **`claude.yml` en `main`:** sí (commit 8d2fa4d). Solo responde a `@claude` del dueño del repo, porque el repo es público.
- **Revisé:** permisos mínimos ☑ (`ci.yml`: `contents: read`) · timeout ☑ (10 min en ambos) · versiones fijas ☑ (acciones por SHA, `ubuntu-24.04`, SDK 10.0.403) · token solo como secreto ☑ (`secrets.CLAUDE_CODE_OAUTH_TOKEN`, ningún archivo lo contiene)

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
