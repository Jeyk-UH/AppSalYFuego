/* ============================================================================================
   SAL Y FUEGO — SCRIPT MAESTRO DEFINITIVO
   Incluye: Creación de BD, Tablas, y todos los datos de prueba
   Orden: Tablas → Catálogos → Ingredientes → Productos → Imágenes → Combos → Menús → Procesos
============================================================================================ */

/* ============================
   PASO 1: CREACIÓN DE BASE DE DATOS
============================ */
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SalYFuegoDB')
    CREATE DATABASE SalYFuegoDB;
GO

USE SalYFuegoDB;
GO

/* ============================
   PASO 2: CREACIÓN DE TABLAS
   (en orden según dependencias de FK)
============================ */

-- ROL
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ROL]'))
CREATE TABLE ROL (
    IdRol        INT PRIMARY KEY IDENTITY(1,1),
    NombreRol    VARCHAR(100) NOT NULL
);

-- USUARIO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USUARIO]'))
CREATE TABLE USUARIO (
    IdUsuario         INT PRIMARY KEY IDENTITY(1,1),
    NombreCompleto    VARCHAR(200) NOT NULL,
    Correo            VARCHAR(150) NOT NULL UNIQUE,
    ContrasenaHash    VARCHAR(500) NOT NULL,
    TokenRecuperacion VARCHAR(500) NULL,
    TokenExpiracion   DATETIME NULL,
    Activo            BIT NOT NULL DEFAULT 1,
    IdRol             INT NOT NULL,
    Cedula            VARCHAR(20) NULL,
    FOREIGN KEY (IdRol) REFERENCES ROL(IdRol)
);

-- Actualización idempotente: agrega Cedula si la tabla ya existía sin esa columna
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[USUARIO]') AND name = 'Cedula'
)
    ALTER TABLE USUARIO ADD Cedula VARCHAR(20) NULL;
GO

-- DIRECCION_USUARIO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DIRECCION_USUARIO]'))
CREATE TABLE DIRECCION_USUARIO (
    IdDireccion      INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario        INT NOT NULL,
    Alias            VARCHAR(100),
    Provincia        VARCHAR(100),
    Canton           VARCHAR(100),
    Distrito         VARCHAR(100),
    DireccionExacta  VARCHAR(500),
    Referencia       VARCHAR(300),
    EsPredeterminada BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (IdUsuario) REFERENCES USUARIO(IdUsuario)
);

-- CATEGORIA
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CATEGORIA]'))
CREATE TABLE CATEGORIA (
    IdCategoria INT PRIMARY KEY IDENTITY(1,1),
    Nombre      VARCHAR(150) NOT NULL
);

-- INGREDIENTE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[INGREDIENTE]'))
CREATE TABLE INGREDIENTE (
    IdIngrediente INT PRIMARY KEY IDENTITY(1,1),
    Nombre        VARCHAR(150) NOT NULL
);

-- PRODUCTO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRODUCTO]'))
CREATE TABLE PRODUCTO (
    IdProducto  INT PRIMARY KEY IDENTITY(1,1),
    Nombre      VARCHAR(200) NOT NULL,
    Descripcion VARCHAR(500),
    Precio      DECIMAL(10,2) NOT NULL,
    Activo      BIT NOT NULL DEFAULT 1,
    IdCategoria INT NOT NULL,
    FOREIGN KEY (IdCategoria) REFERENCES CATEGORIA(IdCategoria)
);

-- PRODUCTO_INGREDIENTE (muchos a muchos)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRODUCTO_INGREDIENTE]'))
CREATE TABLE PRODUCTO_INGREDIENTE (
    IdProducto    INT NOT NULL,
    IdIngrediente INT NOT NULL,
    PRIMARY KEY (IdProducto, IdIngrediente),
    FOREIGN KEY (IdProducto)    REFERENCES PRODUCTO(IdProducto),
    FOREIGN KEY (IdIngrediente) REFERENCES INGREDIENTE(IdIngrediente)
);

-- PRODUCTO_IMAGEN
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRODUCTO_IMAGEN]'))
CREATE TABLE PRODUCTO_IMAGEN (
    IdImagen    INT PRIMARY KEY IDENTITY(1,1),
    IdProducto  INT NOT NULL,
    UrlImagen   VARCHAR(500) NOT NULL,
    EsPrincipal BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (IdProducto) REFERENCES PRODUCTO(IdProducto)
);

-- COMBO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[COMBO]'))
CREATE TABLE COMBO (
    IdCombo        INT PRIMARY KEY IDENTITY(1,1),
    Nombre         VARCHAR(200) NOT NULL,
    Descripcion    VARCHAR(500),
    PrecioEspecial DECIMAL(10,2) NOT NULL,
    Activo         BIT NOT NULL DEFAULT 1,
    IdCategoria    INT NOT NULL,
    UrlImagen      VARCHAR(255) NULL,
    FOREIGN KEY (IdCategoria) REFERENCES CATEGORIA(IdCategoria)
);

-- COMBO_PRODUCTO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[COMBO_PRODUCTO]'))
CREATE TABLE COMBO_PRODUCTO (
    IdCombo    INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad   INT NOT NULL,
    PRIMARY KEY (IdCombo, IdProducto),
    FOREIGN KEY (IdCombo)    REFERENCES COMBO(IdCombo),
    FOREIGN KEY (IdProducto) REFERENCES PRODUCTO(IdProducto)
);

-- MENU
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MENU]'))
CREATE TABLE MENU (
    IdMenu     INT PRIMARY KEY IDENTITY(1,1),
    Nombre     VARCHAR(150) NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFin    TIME NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- MENU_DISPONIBILIDAD
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MENU_DISPONIBILIDAD]'))
CREATE TABLE MENU_DISPONIBILIDAD (
    IdDisponibilidad INT PRIMARY KEY IDENTITY(1,1),
    IdMenu           INT NOT NULL,
    FechaInicio      DATE NULL,
    FechaFin         DATE NULL,
    DiaSemana        VARCHAR(20) NULL,
    FOREIGN KEY (IdMenu) REFERENCES MENU(IdMenu)
);

-- MENU_ITEM
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MENU_ITEM]'))
CREATE TABLE MENU_ITEM (
    IdMenuItem INT PRIMARY KEY IDENTITY(1,1),
    IdMenu     INT NOT NULL,
    IdProducto INT NULL,
    IdCombo    INT NULL,
    FOREIGN KEY (IdMenu)     REFERENCES MENU(IdMenu),
    FOREIGN KEY (IdProducto) REFERENCES PRODUCTO(IdProducto),
    FOREIGN KEY (IdCombo)    REFERENCES COMBO(IdCombo)
);

-- ESTACION
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ESTACION]'))
CREATE TABLE ESTACION (
    IdEstacion  INT PRIMARY KEY IDENTITY(1,1),
    Nombre      VARCHAR(150) NOT NULL,
    Descripcion VARCHAR(300)
);

-- PROCESO_PREPARACION
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PROCESO_PREPARACION]'))
CREATE TABLE PROCESO_PREPARACION (
    IdProceso             INT PRIMARY KEY IDENTITY(1,1),
    OrdenPaso             INT NOT NULL,
    TiempoEstimadoMinutos INT NOT NULL,
    IdProducto            INT NOT NULL,
    IdEstacion            INT NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES PRODUCTO(IdProducto),
    FOREIGN KEY (IdEstacion) REFERENCES ESTACION(IdEstacion)
);

-- ESTADO_PEDIDO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ESTADO_PEDIDO]'))
CREATE TABLE ESTADO_PEDIDO (
    IdEstado INT PRIMARY KEY IDENTITY(1,1),
    Nombre   VARCHAR(100) NOT NULL,
    Orden    INT NOT NULL
);

-- METODO_PAGO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[METODO_PAGO]'))
CREATE TABLE METODO_PAGO (
    IdMetodoPago INT PRIMARY KEY IDENTITY(1,1),
    Nombre       VARCHAR(100) NOT NULL
);

-- PEDIDO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PEDIDO]'))
CREATE TABLE PEDIDO (
    IdPedido           INT PRIMARY KEY IDENTITY(1,1),
    CodigoOrden        VARCHAR(50) NOT NULL,
    FechaPedido        DATETIME NOT NULL,
    OrigenPedido       VARCHAR(50) NOT NULL,
    MetodoEntrega      VARCHAR(50) NOT NULL,
    Subtotal           DECIMAL(10,2) NOT NULL,
    Impuesto           DECIMAL(10,2) NOT NULL,
    CostoEnvio         DECIMAL(10,2) NOT NULL,
    Total              DECIMAL(10,2) NOT NULL,
    IdEstado           INT NOT NULL,
    IdEstacionActual   INT NULL,
    IdCliente          INT NULL,
    IdEmpleado         INT NULL,
    IdDireccionEntrega INT NULL,
    -- Datos de un cliente sin cuenta (venta anónima desde Caja). Se usan solo
    -- cuando IdCliente es NULL; no crean una fila real en USUARIO.
    NombreClienteInvitado VARCHAR(200) NULL,
    CedulaClienteInvitado VARCHAR(20) NULL,
    FOREIGN KEY (IdEstado)           REFERENCES ESTADO_PEDIDO(IdEstado),
    FOREIGN KEY (IdEstacionActual)   REFERENCES ESTACION(IdEstacion),
    FOREIGN KEY (IdCliente)          REFERENCES USUARIO(IdUsuario),
    FOREIGN KEY (IdEmpleado)         REFERENCES USUARIO(IdUsuario),
    FOREIGN KEY (IdDireccionEntrega) REFERENCES DIRECCION_USUARIO(IdDireccion)
);

-- Actualización idempotente: agrega las columnas de cliente invitado si el pedido ya existía
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[PEDIDO]') AND name = 'NombreClienteInvitado'
)
    ALTER TABLE PEDIDO ADD NombreClienteInvitado VARCHAR(200) NULL;
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[PEDIDO]') AND name = 'CedulaClienteInvitado'
)
    ALTER TABLE PEDIDO ADD CedulaClienteInvitado VARCHAR(20) NULL;
GO

-- DETALLE_PEDIDO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DETALLE_PEDIDO]'))
CREATE TABLE DETALLE_PEDIDO (
    IdDetalle      INT PRIMARY KEY IDENTITY(1,1),
    Cantidad       INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal       DECIMAL(10,2) NOT NULL,
    Observaciones  VARCHAR(300),
    IdPedido       INT NOT NULL,
    IdProducto     INT NULL,
    IdCombo        INT NULL,
    FOREIGN KEY (IdPedido)     REFERENCES PEDIDO(IdPedido),
    FOREIGN KEY (IdProducto)   REFERENCES PRODUCTO(IdProducto),
    FOREIGN KEY (IdCombo)      REFERENCES COMBO(IdCombo)
);

-- HISTORIAL_ESTACION
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HISTORIAL_ESTACION]'))
CREATE TABLE HISTORIAL_ESTACION (
    IdHistorial INT PRIMARY KEY IDENTITY(1,1),
    IdPedido    INT NOT NULL,
    IdEstacion  INT NOT NULL,
    HoraIngreso DATETIME NOT NULL,
    HoraSalida  DATETIME NULL,
    FOREIGN KEY (IdPedido)   REFERENCES PEDIDO(IdPedido),
    FOREIGN KEY (IdEstacion) REFERENCES ESTACION(IdEstacion)
);

-- HISTORIAL_ESTADO_PEDIDO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HISTORIAL_ESTADO_PEDIDO]'))
CREATE TABLE HISTORIAL_ESTADO_PEDIDO (
    IdHistorialEstado INT PRIMARY KEY IDENTITY(1,1),
    IdPedido          INT NOT NULL,
    IdEstado          INT NOT NULL,
    FechaHora         DATETIME NOT NULL,
    IdUsuario         INT NOT NULL,
    Observacion       VARCHAR(500),
    FOREIGN KEY (IdPedido)  REFERENCES PEDIDO(IdPedido),
    FOREIGN KEY (IdEstado)  REFERENCES ESTADO_PEDIDO(IdEstado),
    FOREIGN KEY (IdUsuario) REFERENCES USUARIO(IdUsuario)
);

-- PAGO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PAGO]'))
CREATE TABLE PAGO (
    IdPago         INT PRIMARY KEY IDENTITY(1,1),
    MontoPagado    DECIMAL(10,2) NOT NULL,
    Vuelto         DECIMAL(10,2) NOT NULL,
    TipoTarjeta    VARCHAR(50) NULL,
    UltimosDigitos VARCHAR(4) NULL,
    FechaPago      DATETIME NOT NULL,
    IdPedido       INT NOT NULL,
    IdMetodoPago   INT NOT NULL,
    FOREIGN KEY (IdPedido)     REFERENCES PEDIDO(IdPedido),
    FOREIGN KEY (IdMetodoPago) REFERENCES METODO_PAGO(IdMetodoPago)
);
GO

/* ============================
   PASO 3: ROLES
============================ */
SET IDENTITY_INSERT ROL ON;
INSERT INTO ROL (IdRol, NombreRol) VALUES
(1, 'Administrador'),
(2, 'Encargado'),
(3, 'Cocina'),
(4, 'Cliente'),
(5, 'Salonero'),
(6, 'Repartidor');
SET IDENTITY_INSERT ROL OFF;
GO

/* ============================
   PASO 3B: USUARIOS DE PRUEBA (uno por rol)
   Las contraseñas ya vienen encriptadas con el algoritmo AES de
   Cryptography.Encrypt, usando el secreto configurado en
   SalYFuego/appsettings.Development.json -> Crypto:Secret.
   Si ese secreto cambia, hay que recalcular estos hashes.

   Credenciales de prueba:
   admin@salyfuego.com       / Admin123*
   encargado@salyfuego.com   / Encargado123*
   cocina@salyfuego.com      / Cocina123*
   cliente@salyfuego.com     / Cliente123*
   salonero@salyfuego.com    / Salonero123*
   repartidor@salyfuego.com  / Repartidor123*
============================ */
INSERT INTO USUARIO (NombreCompleto, Correo, ContrasenaHash, Activo, IdRol) VALUES
('Administrador General', 'admin@salyfuego.com',       'iB3AD6Z2vtph5ZAYcOg85Q==', 1, 1),
('Encargado de Caja',     'encargado@salyfuego.com',   '4VS7DiWQHYW7iqnqj09V9w==', 1, 2),
('Cocina Principal',      'cocina@salyfuego.com',      'K1FcLk2VFTTEaa7dXANUnw==', 1, 3),
('Cliente Demo',          'cliente@salyfuego.com',     'bRL/HyQL6iqThr/DV3dfEA==', 1, 4),
('Salonero Demo',         'salonero@salyfuego.com',    'f9eYwvpd/W6cjXOf7kMgpw==', 1, 5),
('Repartidor Demo',       'repartidor@salyfuego.com',  '5wtdizAKPKpbS3FOjpxnHQ==', 1, 6);
GO

/* ============================
   PASO 4: CATEGORÍAS
============================ */
SET IDENTITY_INSERT CATEGORIA ON;
INSERT INTO CATEGORIA (IdCategoria, Nombre) VALUES
(1,  'Comida Rápida'),
(2,  'Tacos'),
(3,  'Postres'),
(4,  'Corte de Carnes'),
(5,  'Almuerzos'),
(6,  'Ensaladas'),
(7,  'Pastas'),
(8,  'Frescos Naturales'),
(9,  'Batidos'),
(10, 'Gaseosas'),
(11, 'Cervezas'),
(12, 'Vinos'),
(13, 'Cócteles');
SET IDENTITY_INSERT CATEGORIA OFF;
GO

/* ============================
   PASO 5: ESTACIONES
============================ */
SET IDENTITY_INSERT ESTACION ON;
INSERT INTO ESTACION (IdEstacion, Nombre, Descripcion) VALUES
(1, 'Caja',     'Punto de recepción y registro del pedido.'),
(2, 'Cocina',   'Área de preparación de todos los productos del pedido.'),
(3, 'Despacho', 'Pedido listo para entregar al cliente o repartidor.');
SET IDENTITY_INSERT ESTACION OFF;
GO

/* ============================
   PASO 6: ESTADOS DEL PEDIDO
============================ */
SET IDENTITY_INSERT ESTADO_PEDIDO ON;
INSERT INTO ESTADO_PEDIDO (IdEstado, Nombre, Orden) VALUES
(1, 'Pendiente de Pago', 1),
(2, 'Aceptada',          2),
(3, 'Preparación',       3),
(4, 'Procesando',        4),
(5, 'Entregada',         5);
SET IDENTITY_INSERT ESTADO_PEDIDO OFF;
GO

/* ============================
   PASO 7: MÉTODOS DE PAGO
============================ */
SET IDENTITY_INSERT METODO_PAGO ON;
INSERT INTO METODO_PAGO (IdMetodoPago, Nombre) VALUES
(1, 'Efectivo'),
(2, 'Tarjeta'),
(3, 'SINPE');
SET IDENTITY_INSERT METODO_PAGO OFF;
GO

/* ============================
   PASO 8: INGREDIENTES
============================ */
SET IDENTITY_INSERT INGREDIENTE ON;
INSERT INTO INGREDIENTE (IdIngrediente, Nombre) VALUES
(1,  'Pollo arreglado'),
(2,  'Lechuga fresca'),
(3,  'Tomate'),
(4,  'Queso Mozzarella'),
(5,  'Pan Artesanal'),
(6,  'Salsa BBQ'),
(7,  'Ensalada Coleslaw'),
(8,  'Torta de punta de solomo'),
(9,  'Tocineta'),
(10, 'Vegetales frescos'),
(11, 'Queso Cheddar'),
(12, 'Tiras de pollo a la parrilla'),
(13, 'Tortilla de harina'),
(14, 'Aderezo César'),
(15, 'Tomate deshidratado'),
(16, 'Croutones'),
(17, 'Canela en polvo'),
(18, 'Dulce de leche'),
(19, 'Masa dulce'),
(20, 'Corte Ribeye'),
(21, 'Salsa Holandesa'),
(22, 'Espárragos frescos'),
(23, 'Ensalada tropical'),
(24, 'Tomate Cherry'),
(25, 'Pasta natural'),
(26, 'Zanahoria'),
(27, 'Hongos frescos'),
(28, 'Costillar de cerdo'),
(29, 'Piña a la parrilla'),
(30, 'Perejil fresco'),
(31, 'Pollo en salsa caribeña'),
(32, 'Arroz blanco'),
(33, 'Frijoles en leche de coco'),
(34, 'Patacones'),
(35, 'Pechuga de pollo en trozos'),
(36, 'Carne de res en trozos'),
(37, 'Chile dulce'),
(38, 'Cebollino fresco'),
(39, 'Papas fritas'),
(40, 'Ensalada verde'),
(41, 'Pollo a la plancha'),
(42, 'Mango maduro'),
(43, 'Aguacate fresco'),
(44, 'Queso blanco fresco'),
(45, 'Pasta tradicional'),
(46, 'Salsa Pomodoro'),
(47, 'Hierbas frescas'),
(48, 'Pulpa de Guanábana'),
(49, 'Piña fresca'),
(50, 'Moras frescas'),
(51, 'Fresas frescas'),
(52, 'Pulpa de Mango'),
(53, 'Pulpa de Maracuyá'),
(54, 'Leche'),
(55, 'Azúcar'),
(56, 'Hielo');
SET IDENTITY_INSERT INGREDIENTE OFF;
GO

/* ============================
   PASO 9: PRODUCTOS
============================ */
SET IDENTITY_INSERT PRODUCTO ON;
INSERT INTO PRODUCTO (IdProducto, Nombre, Descripcion, Precio, Activo, IdCategoria) VALUES
-- Comida Rápida
(1,  'Panini Artesanal',           'Panini relleno de pollo arreglado con lechuga, tomate y queso mozzarella, en pan artesanal tostado.',      3500.00, 1, 1),
(2,  'Hamburguesa Fusión',         'Hamburguesa con costilla BBQ y ensalada coleslaw en pan artesanal. Fusión de sabores únicos.',               3000.00, 1, 1),
(3,  'Hamburguesa Blend',          'Hamburguesa con torta de punta de solomo, tocineta, vegetales frescos y queso cheddar.',                    3500.00, 1, 1),
(4,  'Papas Fritas',               'Porción de papas fritas crujientes.',                                                                        800.00, 1, 1),
(5,  'Refresco',                   'Refresco natural de la casa.',                                                                               600.00, 1, 8),
(6,  'Gaseosa',                    'Gaseosa de lata 354ml.',                                                                                     700.00, 1, 10),
-- Tacos
(7,  'Tacos al César',             'Tiras de pollo a la parrilla, lechuga fresca, tortilla con queso, aderezo césar y tocineta.',               2800.00, 1, 2),
-- Postres
(8,  'Arrollado de Canela',        'Suave arrollado casero con relleno de canela y azúcar horneado al momento.',                                1500.00, 1, 3),
(9,  'Arrollado de Dulce de Leche','Arrollado esponjoso relleno de dulce de leche.',                                                            1500.00, 1, 3),
(10, 'Torta Chilena',              'Clásica torta chilena de hojaldre con manjar y azúcar impalpable.',                                         2000.00, 1, 3),
-- Corte de Carnes
(11, 'Ribeye en Salsa Holandesa',  'Corte Ribeye con espárragos, ensalada tropical y salsa holandesa artesanal.',                              9500.00, 1, 4),
(12, 'Ribeye Corte Abanico',       'Corte Ribeye en presentación abanico con pasta de vegetales: tomate cherry, hongos, zanahoria y albahaca.',9000.00, 1, 4),
(13, 'Costilla BBQ',               'Costilla de cerdo bañada en salsa BBQ, con tomate cherry, piña y perejil.',                                7500.00, 1, 4),
-- Almuerzos
(14, 'Rice and Beans',             'Pollo en salsa caribeña con arroz y frijoles en leche de coco, patacones y ensalada.',                     4500.00, 1, 5),
(15, 'Arroz Mixto',                'Arroz con pollo, carne de res, vegetales, papas fritas y ensalada.',                                        4000.00, 1, 5),
-- Ensaladas
(16, 'Ensalada Tropical César',    'Pollo a la plancha, tocineta, mango, aguacate, tomate cherry, lechuga, queso fresco y aderezo césar.',     3800.00, 1, 6),
-- Pastas
(17, 'Pasta a la Burra',           'Pasta con hierbas frescas, queso mozzarella fundido y salsa pomodoro artesanal.',                           3500.00, 1, 7),
-- Frescos Naturales
(18, 'Fresco de Guanábana',        'Refrescante bebida natural de guanábana, endulzada al gusto, servida con hielo.',                          1200.00, 1, 8),
(19, 'Fresco de Piña',             'Fresco natural de piña recién preparado.',                                                                  1200.00, 1, 8),
(22, 'Fresco de Maracuyá',         'Fresco natural de maracuyá con balance perfecto entre dulce y ácido.',                                     1300.00, 1, 8),
-- Batidos
(20, 'Batido Mora con Fresa',      'Batido cremoso de mora y fresa con leche.',                                                                 1500.00, 1, 9),
(21, 'Batido de Mango',            'Batido natural de mango maduro con leche y hielo.',                                                         1500.00, 1, 9),
(23, 'Batido Tropical',            'Batido de maracuyá, mango y piña con leche de coco.',                                                       1800.00, 1, 9),
-- Cervezas
(24, 'Cerveza Imperial',           'Cerveza lager tradicional costarricense, servida bien fría.',                                               1500.00, 1, 11),
(25, 'Cerveza Pilsen',             'Cerveza nacional de sabor con carácter.',                                                                   1500.00, 1, 11),
-- Vinos
(26, 'Copa de Vino Tinto',         'Copa de Cabernet Sauvignon de la casa.',                                                                    2500.00, 1, 12),
-- Cócteles
(27, 'Mojito Clásico',             'Cóctel refrescante a base de ron blanco y hierbabuena.',                                                    3200.00, 1, 13),
-- Gaseosas
(28, 'Coca-Cola Original',         'Gaseosa Coca-Cola sabor original en lata de 354ml.',                                                        1200.00, 1, 10),
(29, 'Coca-Cola Sin Azúcar',       'Gaseosa Coca-Cola Zero en lata de 354ml.',                                                                  1200.00, 1, 10),
(30, 'Fanta Naranja',              'Gaseosa Fanta sabor naranja en lata de 354ml.',                                                             1200.00, 1, 10),
(31, 'Fanta Kolita',               'La clásica gaseosa sabor Kolita en lata de 354ml.',                                                         1200.00, 1, 10),
(32, 'Sprite',                     'Gaseosa Sprite sabor lima-limón en lata de 354ml.',                                                         1200.00, 1, 10),
(33, 'Canada Dry Ginger Ale',      'Gaseosa Ginger Ale en lata de 354ml.',                                                                      1200.00, 1, 10);
SET IDENTITY_INSERT PRODUCTO OFF;
GO

/* ============================
   PASO 10: PRODUCTO_INGREDIENTE
============================ */
INSERT INTO PRODUCTO_INGREDIENTE (IdProducto, IdIngrediente) VALUES
(1,1),(1,2),(1,3),(1,4),(1,5),
(2,6),(2,7),(2,5),
(3,8),(3,9),(3,10),(3,11),(3,5),
(4,39),
(7,12),(7,2),(7,13),(7,4),(7,14),(7,15),(7,9),(7,16),
(8,17),(8,19),
(9,18),(9,19),
(10,18),(10,19),
(11,20),(11,21),(11,22),(11,23),
(12,20),(12,24),(12,25),(12,26),(12,27),
(13,28),(13,6),(13,24),(13,29),(13,30),
(14,31),(14,32),(14,33),(14,34),(14,40),
(15,32),(15,35),(15,36),(15,26),(15,37),(15,38),(15,39),(15,40),
(16,41),(16,9),(16,42),(16,43),(16,24),(16,2),(16,44),(16,14),
(17,45),(17,46),(17,4),(17,47),
(18,48),(18,55),(18,56),
(19,49),(19,55),(19,56),
(20,50),(20,51),(20,54),(20,55),
(21,52),(21,54),(21,55),
(22,53),(22,55),(22,56),
(23,53),(23,52),(23,49),(23,54);
GO

/* ============================
   PASO 11: IMÁGENES DE PRODUCTOS
============================ */
INSERT INTO PRODUCTO_IMAGEN (IdProducto, UrlImagen, EsPrincipal) VALUES
(1,  '/uploads/productos/panini-artesanal.jpg',        1),
(2,  '/uploads/productos/hamburguesa-fusion.jpg',      1),
(3,  '/uploads/productos/hamburguesa-blend.jpg',       1),
(4,  '/uploads/productos/papas-fritas.jpg',            1),
(5,  '/uploads/productos/refresco.jpg',                1),
(6,  '/uploads/productos/gaseosa.jpg',                 1),
(7,  '/uploads/productos/tacos-cesar.jpg',             1),
(8,  '/uploads/productos/arrollado-canela.jpg',        1),
(9,  '/uploads/productos/arrollado-dulce-leche.jpg',   1),
(10, '/uploads/productos/torta-chilena.jpg',           1),
(11, '/uploads/productos/ribeye-holandesa.jpg',        1),
(12, '/uploads/productos/ribeye-abanico.jpg',          1),
(13, '/uploads/productos/costilla-bbq.jpg',            1),
(14, '/uploads/productos/rice-and-beans.jpg',          1),
(15, '/uploads/productos/arroz-mixto.jpg',             1),
(16, '/uploads/productos/ensalada-tropical.jpg',       1),
(17, '/uploads/productos/pasta-burra.jpg',             1),
(18, '/uploads/productos/fresco-guanabana.jpg',        1),
(19, '/uploads/productos/fresco-pina.jpg',             1),
(20, '/uploads/productos/batido-mora-fresa.jpg',       1),
(21, '/uploads/productos/batido-mango.jpg',            1),
(22, '/uploads/productos/fresco-maracuya.jpg',         1),
(23, '/uploads/productos/batido-tropical.jpg',         1),
(24, '/uploads/productos/cerveza-imperial.jpg',        1),
(25, '/uploads/productos/cerveza-pilsen.jpg',          1),
(26, '/uploads/productos/copa-vino-tinto.jpg',         1),
(27, '/uploads/productos/mojito-clasico.jpg',          1),
(28, '/uploads/productos/coca-cola-original.jpg',      1),
(29, '/uploads/productos/coca-cola-sin-azucar.jpg',    1),
(30, '/uploads/productos/fanta-naranja.jpg',           1),
(31, '/uploads/productos/fanta-kolita.jpg',            1),
(32, '/uploads/productos/sprite.jpg',                  1),
(33, '/uploads/productos/ginger-ale.jpg',              1);
GO

/* ============================
   PASO 12: COMBOS
============================ */
SET IDENTITY_INSERT COMBO ON;
INSERT INTO COMBO (IdCombo, Nombre, Descripcion, PrecioEspecial, Activo, IdCategoria, UrlImagen) VALUES
(1, 'Combo Panini',            'Panini Artesanal con papas fritas y refresco natural.',          4500.00, 1, 1, '/uploads/combos/combo-panini.jpg'),
(2, 'Combo Hamburguesa Fusión','Hamburguesa Fusión con papas y gaseosa.',                        4000.00, 1, 1, '/uploads/combos/combo-hamburguesa-fusion.jpg'),
(3, 'Combo Hamburguesa Blend', 'Hamburguesa Blend con papas y gaseosa.',                         4500.00, 1, 1, '/uploads/combos/combo-hamburguesa-blend.jpg'),
(4, 'Combo Taco y Postre',     'Tacos al César con Arrollado de Canela.',                        3800.00, 1, 2, '/uploads/combos/combo-taco-postre.jpg'),
(5, 'Combo Almuerzo Caribeño', 'Rice and Beans con Fresco de Guanábana.',                        5500.00, 1, 5, '/uploads/combos/combo-almuerzo-caribeno.jpg'),
(6, 'Combo Parrilla Completa', 'Costilla BBQ con Ensalada César y Pasta a la Burra.',            9500.00, 1, 4, '/uploads/combos/combo-parrilla-completa.jpg'),
(7, 'Combo Noche Italiana',    'Pasta a la Burra con Copa de Vino Tinto.',                       5200.00, 1, 7, '/uploads/combos/combo-noche-italiana.jpg'),
(8, 'Combo Tico Grill',        'Costilla BBQ con Cerveza Imperial bien fría.',                   8200.00, 1, 4, '/uploads/combos/combo-tico-grill.jpg');
SET IDENTITY_INSERT COMBO OFF;

INSERT INTO COMBO_PRODUCTO (IdCombo, IdProducto, Cantidad) VALUES
(1,1,1),(1,4,1),(1,5,1),
(2,2,1),(2,4,1),(2,6,1),
(3,3,1),(3,4,1),(3,6,1),
(4,7,1),(4,8,1),
(5,14,1),(5,18,1),
(6,13,1),(6,16,1),(6,17,1),
(7,17,1),(7,26,1),
(8,13,1),(8,24,1);
GO

/* ============================
   PASO 13: MENÚS Y DISPONIBILIDAD
============================ */
SET IDENTITY_INSERT MENU ON;
INSERT INTO MENU (IdMenu, Nombre, HoraInicio, HoraFin, EstaActivo) VALUES
(1, 'Menú Almuerzo',         '11:00:00', '15:00:00', 1),
(2, 'Menú Cena',             '18:00:00', '22:00:00', 1),
(3, 'Menú Desayuno',         '07:00:00', '10:30:00', 0),
(4, 'Menú Fin de Semana',    '12:00:00', '21:00:00', 0),
(5, 'Menú Especial Navidad', '12:00:00', '23:59:00', 0);
SET IDENTITY_INSERT MENU OFF;

-- Disponibilidad por días de semana
INSERT INTO MENU_DISPONIBILIDAD (IdMenu, FechaInicio, FechaFin, DiaSemana) VALUES
(1, NULL, NULL, 'Lunes'),
(1, NULL, NULL, 'Martes'),
(1, NULL, NULL, 'Miércoles'),
(1, NULL, NULL, 'Jueves'),
(1, NULL, NULL, 'Viernes'),
(2, NULL, NULL, 'Viernes'),
(2, NULL, NULL, 'Sábado'),
(3, NULL, NULL, 'Lunes'),
(3, NULL, NULL, 'Martes'),
(3, NULL, NULL, 'Miércoles'),
(3, NULL, NULL, 'Jueves'),
(3, NULL, NULL, 'Viernes'),
(3, NULL, NULL, 'Sábado'),
(3, NULL, NULL, 'Domingo'),
(4, NULL, NULL, 'Sábado'),
(4, NULL, NULL, 'Domingo');

-- Disponibilidad por rango de fechas (Navidad)
INSERT INTO MENU_DISPONIBILIDAD (IdMenu, FechaInicio, FechaFin, DiaSemana) VALUES
(5, '2026-12-01', '2026-12-31', NULL);

-- Items del Menú Almuerzo
INSERT INTO MENU_ITEM (IdMenu, IdProducto, IdCombo) VALUES
(1,1,NULL),(1,2,NULL),(1,4,NULL),(1,5,NULL),(1,6,NULL),
(1,14,NULL),(1,15,NULL),(1,16,NULL),(1,17,NULL),
(1,18,NULL),(1,19,NULL),(1,20,NULL),(1,21,NULL),(1,22,NULL),(1,23,NULL),
(1,28,NULL),(1,29,NULL),(1,30,NULL),(1,31,NULL),(1,32,NULL),(1,33,NULL),
(1,NULL,1),(1,NULL,2),(1,NULL,3),(1,NULL,5);

-- Items del Menú Cena
INSERT INTO MENU_ITEM (IdMenu, IdProducto, IdCombo) VALUES
(2,2,NULL),(2,3,NULL),(2,4,NULL),(2,7,NULL),
(2,8,NULL),(2,9,NULL),(2,10,NULL),
(2,11,NULL),(2,12,NULL),(2,13,NULL),
(2,16,NULL),(2,17,NULL),
(2,24,NULL),(2,25,NULL),(2,26,NULL),(2,27,NULL),
(2,28,NULL),(2,29,NULL),(2,30,NULL),(2,31,NULL),(2,32,NULL),(2,33,NULL),
(2,NULL,3),(2,NULL,4),(2,NULL,6),(2,NULL,7),(2,NULL,8);

-- Items del Menú Desayuno
INSERT INTO MENU_ITEM (IdMenu, IdProducto, IdCombo) VALUES
(3,1,NULL),(3,8,NULL),(3,9,NULL),
(3,18,NULL),(3,19,NULL),(3,20,NULL),(3,21,NULL),(3,22,NULL),(3,23,NULL);

-- Items del Menú Navidad
INSERT INTO MENU_ITEM (IdMenu, IdProducto, IdCombo) VALUES
(5,11,NULL),(5,13,NULL),(5,10,NULL),(5,9,NULL),(5,23,NULL),
(5,NULL,6),(5,NULL,7),(5,NULL,8);
GO

/* ============================
   PASO 14: PROCESOS DE PREPARACIÓN
   1 estación  → Bebidas simples
   2 estaciones → Preparación + Despacho
   3 estaciones → Caja + Cocina + Despacho
============================ */

-- 1 ESTACIÓN → Solo Cocina
INSERT INTO PROCESO_PREPARACION (OrdenPaso, TiempoEstimadoMinutos, IdProducto, IdEstacion) VALUES
(1, 5,  18, 2),  -- Fresco de Guanábana
(1, 5,  19, 2),  -- Fresco de Piña
(1, 5,  22, 2),  -- Fresco de Maracuyá
(1, 7,  20, 2),  -- Batido Mora con Fresa
(1, 7,  21, 2),  -- Batido de Mango
(1, 7,  23, 2),  -- Batido Tropical
(1, 2,  6,  2),  -- Gaseosa
(1, 2,  28, 2),  -- Coca-Cola Original
(1, 2,  29, 2),  -- Coca-Cola Sin Azúcar
(1, 2,  30, 2),  -- Fanta Naranja
(1, 2,  31, 2),  -- Fanta Kolita
(1, 2,  32, 2),  -- Sprite
(1, 2,  33, 2),  -- Canada Dry Ginger Ale
(1, 3,  24, 2),  -- Cerveza Imperial
(1, 3,  25, 2),  -- Cerveza Pilsen
(1, 3,  26, 2);  -- Copa de Vino Tinto

-- 2 ESTACIONES → Cocina → Despacho
INSERT INTO PROCESO_PREPARACION (OrdenPaso, TiempoEstimadoMinutos, IdProducto, IdEstacion) VALUES
(1, 5,  8,  2),(2, 2,  8,  3),  -- Arrollado de Canela
(1, 5,  9,  2),(2, 2,  9,  3),  -- Arrollado de Dulce de Leche
(1, 5,  10, 2),(2, 2,  10, 3),  -- Torta Chilena
(1, 10, 7,  2),(2, 3,  7,  3),  -- Tacos al César
(1, 8,  16, 2),(2, 3,  16, 3),  -- Ensalada Tropical César
(1, 12, 17, 2),(2, 3,  17, 3),  -- Pasta a la Burra
(1, 15, 14, 2),(2, 3,  14, 3),  -- Rice and Beans
(1, 12, 15, 2),(2, 3,  15, 3),  -- Arroz Mixto
(1, 5,  27, 2),(2, 2,  27, 3),  -- Mojito Clásico
(1, 8,  4,  2),(2, 2,  4,  3),  -- Papas Fritas
(1, 3,  5,  2),(2, 2,  5,  3);  -- Refresco

-- 3 ESTACIONES → Caja → Cocina → Despacho
INSERT INTO PROCESO_PREPARACION (OrdenPaso, TiempoEstimadoMinutos, IdProducto, IdEstacion) VALUES
(1, 2,  1,  1),(2, 10, 1,  2),(3, 2,  1,  3),  -- Panini Artesanal
(1, 2,  2,  1),(2, 12, 2,  2),(3, 3,  2,  3),  -- Hamburguesa Fusión
(1, 2,  3,  1),(2, 12, 3,  2),(3, 3,  3,  3),  -- Hamburguesa Blend
(1, 2,  11, 1),(2, 20, 11, 2),(3, 3,  11, 3),  -- Ribeye en Salsa Holandesa
(1, 2,  12, 1),(2, 18, 12, 2),(3, 3,  12, 3),  -- Ribeye Corte Abanico
(1, 2,  13, 1),(2, 25, 13, 2),(3, 3,  13, 3);  -- Costilla BBQ
GO

PRINT '============================================================';
PRINT '  SAL Y FUEGO — BASE DE DATOS LISTA CORRECTAMENTE';
PRINT '============================================================';
