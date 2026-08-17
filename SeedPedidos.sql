/* ============================================================================================
   SAL Y FUEGO — Pedidos de prueba (para Historial y Detalle de Pedido, Avance 5)
   Ejecutar sobre SalYFuegoDB, DESPUÉS de SalYFuegoDB_Maestro.sql y SeedUsuarios.sql.
   Incluye 5 pedidos con variedad de estado, fecha, cliente (registrado y anónimo) y método
   de pago, para poder probar el historial, los filtros y el detalle tipo factura.
============================================================================================ */

USE SalYFuegoDB;
GO

DECLARE @IdCliente INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'cliente@salyfuego.com');
DECLARE @IdAdmin INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'admin@salyfuego.com');
DECLARE @IdEncargado INT = (SELECT IdUsuario FROM USUARIO WHERE Correo = 'encargado@salyfuego.com');

/* ------------------------------------------------------------------
   Pedido 1: Cliente registrado, Entregada, hace 5 días, pago en efectivo
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado)
VALUES ('ORD-SEED0001', DATEADD(DAY, -5, GETDATE()), 'Caja', 'Local', 5100.00, 663.00, 0, 5763.00,
        5, 3, @IdCliente, @IdEncargado);

DECLARE @Pedido1 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 3500.00, 3500.00, NULL,               @Pedido1, 1, NULL),
(2, 800.00,  1600.00, 'Sin sal por favor', @Pedido1, 4, NULL);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (6000.00, 237.00, NULL, NULL, DATEADD(DAY, -5, GETDATE()), @Pedido1, 1);

/* ------------------------------------------------------------------
   Pedido 2: Cliente registrado, Preparación, hoy, pago con tarjeta de crédito
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado)
VALUES ('ORD-SEED0002', GETDATE(), 'Caja', 'Local', 4500.00, 585.00, 0, 5085.00,
        3, 2, @IdCliente, @IdEncargado);

DECLARE @Pedido2 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 4500.00, 4500.00, NULL, @Pedido2, NULL, 1);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (5085.00, 0.00, 'Crédito', '4242', GETDATE(), @Pedido2, 2);

/* ------------------------------------------------------------------
   Pedido 3: Cliente anónimo (con cédula), Pendiente de Pago, hace 1 día
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, NombreClienteInvitado, CedulaClienteInvitado)
VALUES ('ORD-SEED0003', DATEADD(DAY, -1, GETDATE()), 'Caja', 'Para llevar', 7200.00, 936.00, 0, 8136.00,
        1, 1, NULL, @IdEncargado, 'María Solano', '2-0456-0789');

DECLARE @Pedido3 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(2, 3000.00, 6000.00, NULL, @Pedido3, 2, NULL),
(2, 600.00,  1200.00, NULL, @Pedido3, 5, NULL);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (8200.00, 64.00, NULL, NULL, DATEADD(DAY, -1, GETDATE()), @Pedido3, 1);

/* ------------------------------------------------------------------
   Pedido 4: Cliente anónimo (sin cédula), Aceptada, hace 2 días, SINPE
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado, NombreClienteInvitado, CedulaClienteInvitado)
VALUES ('ORD-SEED0004', DATEADD(DAY, -2, GETDATE()), 'Caja', 'Local', 4000.00, 520.00, 0, 4520.00,
        2, 2, NULL, @IdAdmin, 'Carlos Vindas', NULL);

DECLARE @Pedido4 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 4000.00, 4000.00, NULL, @Pedido4, NULL, 2);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (4520.00, 0.00, NULL, NULL, DATEADD(DAY, -2, GETDATE()), @Pedido4, 3);

/* ------------------------------------------------------------------
   Pedido 5: Cliente registrado, Procesando, hace 3 días, tarjeta de débito
------------------------------------------------------------------ */
INSERT INTO PEDIDO (CodigoOrden, FechaPedido, OrigenPedido, MetodoEntrega, Subtotal, Impuesto, CostoEnvio, Total,
                     IdEstado, IdEstacionActual, IdCliente, IdEmpleado)
VALUES ('ORD-SEED0005', DATEADD(DAY, -3, GETDATE()), 'Caja', 'Local', 7500.00, 975.00, 0, 8475.00,
        4, 2, @IdCliente, @IdAdmin);

DECLARE @Pedido5 INT = SCOPE_IDENTITY();

INSERT INTO DETALLE_PEDIDO (Cantidad, PrecioUnitario, Subtotal, Observaciones, IdPedido, IdProducto, IdCombo) VALUES
(1, 3500.00, 3500.00, NULL, @Pedido5, 1, NULL),
(1, 4000.00, 4000.00, 'Extra queso', @Pedido5, NULL, 2);

INSERT INTO PAGO (MontoPagado, Vuelto, TipoTarjeta, UltimosDigitos, FechaPago, IdPedido, IdMetodoPago)
VALUES (8475.00, 0.00, 'Débito', '1188', DATEADD(DAY, -3, GETDATE()), @Pedido5, 2);

GO
