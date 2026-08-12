/* ============================================================================================
   SAL Y FUEGO — Usuarios de prueba (uno por rol)
   Ejecutar sobre SalYFuegoDB. Requiere que la tabla ROL ya tenga los 6 roles
   (Administrador=1, Encargado=2, Cocina=3, Cliente=4, Salonero=5, Repartidor=6).
   Las contraseñas ya vienen encriptadas con el algoritmo AES de Cryptography.Encrypt,
   usando el secreto configurado en SalYFuego/appsettings.Development.json -> Crypto:Secret.
   Si ese secreto cambia, hay que recalcular estos hashes.
============================================================================================ */

USE SalYFuegoDB;
GO

INSERT INTO USUARIO (NombreCompleto, Correo, ContrasenaHash, Activo, IdRol) VALUES
('Administrador General', 'admin@salyfuego.com',       'iB3AD6Z2vtph5ZAYcOg85Q==', 1, 1), -- Admin123*
('Encargado de Caja',     'encargado@salyfuego.com',   '4VS7DiWQHYW7iqnqj09V9w==', 1, 2), -- Encargado123*
('Cocina Principal',      'cocina@salyfuego.com',      'K1FcLk2VFTTEaa7dXANUnw==', 1, 3), -- Cocina123*
('Cliente Demo',          'cliente@salyfuego.com',     'bRL/HyQL6iqThr/DV3dfEA==', 1, 4), -- Cliente123*
('Salonero Demo',         'salonero@salyfuego.com',    'f9eYwvpd/W6cjXOf7kMgpw==', 1, 5), -- Salonero123*
('Repartidor Demo',       'repartidor@salyfuego.com',  '5wtdizAKPKpbS3FOjpxnHQ==', 1, 6); -- Repartidor123*
GO

/* -----------------------------------------------------------------------
   Credenciales de prueba:
   admin@salyfuego.com       / Admin123*
   encargado@salyfuego.com   / Encargado123*
   cocina@salyfuego.com      / Cocina123*
   cliente@salyfuego.com     / Cliente123*
   salonero@salyfuego.com    / Salonero123*
   repartidor@salyfuego.com  / Repartidor123*
   ----------------------------------------------------------------------- */
