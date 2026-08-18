/* ============================================================================================
   SAL Y FUEGO — Pedidos de prueba (para Historial y Detalle de Pedido, Avance 5)
   Ejecutar sobre SalYFuegoDB, DESPUÉS de SalYFuegoDB_Maestro.sql y SeedUsuarios.sql.
   Incluye 5 pedidos con variedad de estado (real, ver ESTADO_PEDIDO), método de entrega
   (recogida/domicilio), cliente (registrado y anónimo) y método de pago, para poder probar
   el historial, los filtros, el detalle tipo factura y el cobro de pedidos pendientes.
============================================================================================ */

USE SalYFuegoDB;
GO

-- Idempotente: si el script ya se había corrido antes (incluso con datos viejos,
-- p. ej. MetodoEntrega = 'Local' o estados fuera de secuencia por pruebas manuales),
-- borra los 5 pedidos de prueba y sus filas relacionadas antes de re-insertarlos.
DELETE FROM PAGO WHERE IdPedido IN (SELECT IdPedido FROM PEDIDO WHERE CodigoOrden LIKE 'ORD-SEED%');
DELETE FROM HISTORIAL_ESTADO_PEDIDO WHERE IdPedido IN (SELECT IdPedido FROM PEDIDO WHERE CodigoOrden LIKE 'ORD-SEED%');
DELETE FROM DETALLE_PEDIDO WHERE IdPedido IN (SELECT IdPedido FROM PEDIDO WHERE CodigoOrden LIKE 'ORD-SEED%');
DELETE FROM PEDIDO WHERE CodigoOrden LIKE 'ORD-SEED%';
DELETE FROM DIRECCION_USUARIO WHERE Alias = 'Entrega Prueba (Seed)';
GO

DECLARE @IdCliente INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'cliente@salyfuego.com');
DECLARE @IdAdmin INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'admin@salyfuego.com');
DECLARE @IdEncargado INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'encargado@salyfuego.com');

-- Direcciones de prueba para los dos pedidos a domicilio (Pedido 2 y 5), para que
-- Repartidor tenga una dirección real que mostrar en su cola.
INSERT INTO DIRECCION_USUARIO (IdUsuario, Alias, Provincia, Canton, Distrito, DireccionExacta, Referencia, EsPredeterminada)
VALUES (@IdCliente, 'Entrega Prueba (Seed)', 'San José', 'Central', 'Carmen', '100m norte del parque central, casa portón negro', 'Frente a la panadería', 0);
DECLARE @Direccion2 INT = SCOPE_IDENTITY();

INSERT INTO DIRECCION_USUARIO (IdUsuario, Alias, Provincia, Canton, Distrito, DireccionExacta, Referencia, EsPredeterminada)
VALUES (@IdCliente, 'Entrega Prueba (Seed)', 'San José', 'Montes de Oca', 'San Pedro', 'Del Mall San Pedro, 200m este, edificio azul, tercer piso', 'Oficina 3B', 0);
DECLARE @Direccion5 INT = SCOPE_IDENTITY();

/* ------------------------------------------------------------------
   Pedido 1: Cliente registrado, Recogida en tienda, Retirado (10),
   hace 5 días, pago en efectivo
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado)
VALUES ('ORD-SEED0001', DATEADD(DAY, -5, GETDATE()), 'Caja', 'Recogida en tienda', 5100.00, 663.00, 0, 5763.00,
        10, 3, @IdCliente, @IdEncargado);

DECLARE @Pedido1 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 3500.00, 3500.00, NULL,               @Pedido1, 1, NULL),
(2, 800.00,  1600.00, 'Sin sal por favor', @Pedido1, 4, NULL);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (6000.00, 237.00, NULL, NULL, DATEADD(DAY, -5, GETDATE()), @Pedido1, 1);

/* ------------------------------------------------------------------
   Pedido 2: Cliente registrado, Entrega a domicilio, En Espera Repartidor (5),
   hoy, pago con tarjeta de crédito (cobro simulado en línea) — listo para
   probar la cola de Repartidor ("Para recoger")
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, IdDireccionEntrega)
VALUES ('ORD-SEED0002', GETDATE(), 'Cliente', 'Entrega a domicilio', 4500.00, 585.00, 1500.00, 6585.00,
        5, 2, @IdCliente, NULL, @Direccion2);

DECLARE @Pedido2 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 4500.00, 4500.00, NULL, @Pedido2, NULL, 1);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (6585.00, 0.00, 'Crédito', '4242', GETDATE(), @Pedido2, 4);

/* ------------------------------------------------------------------
   Pedido 3: Cliente anónimo (con cédula), Recogida en tienda,
   Pendiente de Pago (1), hace 1 día — SIN registro de Pago todavía
   (paga en efectivo al retirar; pendiente de que Encargado lo cobre)
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, NombreClienteInvitado, CedulaClienteInvitado)
VALUES ('ORD-SEED0003', DATEADD(DAY, -1, GETDATE()), 'Caja', 'Recogida en tienda', 7200.00, 936.00, 0, 8136.00,
        1, 1, NULL, @IdEncargado, 'María Solano', '2-0456-0789');

DECLARE @Pedido3 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(2, 3000.00, 6000.00, NULL, @Pedido3, 2, NULL),
(2, 600.00,  1200.00, NULL, @Pedido3, 5, NULL);

/* ------------------------------------------------------------------
   Pedido 4: Cliente anónimo (sin cédula), Recogida en tienda, Pagado (2),
   hace 2 días, tarjeta de débito
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, NombreClienteInvitado, CedulaClienteInvitado)
VALUES ('ORD-SEED0004', DATEADD(DAY, -2, GETDATE()), 'Caja', 'Recogida en tienda', 4000.00, 520.00, 0, 4520.00,
        2, 2, NULL, @IdAdmin, 'Carlos Vindas', NULL);

DECLARE @Pedido4 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 4000.00, 4000.00, NULL, @Pedido4, NULL, 2);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (4520.00, 0.00, 'Débito', '3021', DATEADD(DAY, -2, GETDATE()), @Pedido4, 3);

/* ------------------------------------------------------------------
   Pedido 5: Cliente registrado, Entrega a domicilio, En Ruta (6),
   hace 3 días, tarjeta de débito — listo para probar la cola de
   Repartidor ("En ruta" → "Marcar entregado")
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, IdDireccionEntrega)
VALUES ('ORD-SEED0005', DATEADD(DAY, -3, GETDATE()), 'Cliente', 'Entrega a domicilio', 7500.00, 975.00, 1500.00, 9975.00,
        6, 2, @IdCliente, NULL, @Direccion5);

DECLARE @Pedido5 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 3500.00, 3500.00, NULL, @Pedido5, 1, NULL),
(1, 4000.00, 4000.00, 'Extra queso', @Pedido5, NULL, 2);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (9975.00, 0.00, 'Débito', '1188', DATEADD(DAY, -3, GETDATE()), @Pedido5, 4);

GO
