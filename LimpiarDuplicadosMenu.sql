/* ============================================================================================
   SAL Y FUEGO — Limpieza de duplicados en MENU_DISPONIBILIDAD y MENU_ITEM
   Ejecutar UNA vez sobre SalYFuegoDB para corregir datos ya duplicados (por ejemplo,
   "Menú Cena" mostrando Viernes/Sábado repetidos, o productos repetidos en Caja/Carrito/
   Menú del día).

   Causa raíz (ya corregida en el código, en RepositoryMenu.UpdateAsync): al editar un menú
   desde el panel de Administrador, la relación MENU_DISPONIBILIDAD/MENU_ITEM -> MENU tiene
   la FK obligatoria configurada como "ClientSetNull". Eso significa que limpiar la colección
   en memoria (menu.MenuDisponibilidad.Clear() / menu.MenuItem.Clear()) NO borraba las filas
   viejas de la base de datos: cada vez que se guardaba una edición, las disponibilidades e
   ítems se iban ACUMULANDO en vez de reemplazarse. Este script es una limpieza de una sola
   vez para los datos que ya quedaron duplicados; es seguro volver a ejecutarlo (si no hay
   duplicados, no borra nada).
============================================================================================ */

USE SalYFuegoDB;
GO

PRINT 'Duplicados encontrados en MENU_DISPONIBILIDAD:';
SELECT IdMenu, DiaSemana, FechaInicio, FechaFin, COUNT(*) AS Copias
FROM MENU_DISPONIBILIDAD
GROUP BY IdMenu, DiaSemana, FechaInicio, FechaFin
HAVING COUNT(*) > 1;

-- Deja solo una fila por combinación (IdMenu, DiaSemana, FechaInicio, FechaFin),
-- eliminando las copias de más (se conserva la de menor IdDisponibilidad).
;WITH Duplicados AS (
    SELECT IdDisponibilidad,
           ROW_NUMBER() OVER (
               PARTITION BY IdMenu, ISNULL(DiaSemana, ''), ISNULL(FechaInicio, '1900-01-01'), ISNULL(FechaFin, '1900-01-01')
               ORDER BY IdDisponibilidad
           ) AS Fila
    FROM MENU_DISPONIBILIDAD
)
DELETE FROM MENU_DISPONIBILIDAD
WHERE IdDisponibilidad IN (SELECT IdDisponibilidad FROM Duplicados WHERE Fila > 1);

PRINT 'Duplicados encontrados en MENU_ITEM:';
SELECT IdMenu, IdProducto, IdCombo, COUNT(*) AS Copias
FROM MENU_ITEM
GROUP BY IdMenu, IdProducto, IdCombo
HAVING COUNT(*) > 1;

-- Igual para MENU_ITEM: deja solo una fila por combinación (IdMenu, IdProducto, IdCombo).
;WITH DuplicadosItem AS (
    SELECT IdMenuItem,
           ROW_NUMBER() OVER (
               PARTITION BY IdMenu, ISNULL(IdProducto, -1), ISNULL(IdCombo, -1)
               ORDER BY IdMenuItem
           ) AS Fila
    FROM MENU_ITEM
)
DELETE FROM MENU_ITEM
WHERE IdMenuItem IN (SELECT IdMenuItem FROM DuplicadosItem WHERE Fila > 1);

PRINT 'Limpieza completa.';
GO
