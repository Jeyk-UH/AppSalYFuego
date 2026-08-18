# Sal y Fuego — Publicar en IIS, paso a paso detallado

Guía pensada para seguir clic por clic en tu PC. Cada fase indica exactamente qué buscar, dónde hacer clic y qué deberías ver al terminar. Si algo no coincide con lo que ves, andá a la sección **Fase 8 — Errores comunes** al final.

---

## Fase 0 — Antes de empezar

- Confirmá que la app corre bien local (`dotnet run` desde Visual Studio con F5) y que podés iniciar sesión con `admin@salyfuego.com` / `Admin123*`. Si esto no funciona local, no va a funcionar en IIS tampoco — arreglalo primero.
- Confirmá que SQL Server está corriendo en tu PC y que ya ejecutaste, en este orden, los scripts: `SalYFuegoDB_Maestro.sql` → `SeedUsuarios.sql` → `SeedPedidos.sql` (y si ya habías probado el panel de Menús antes, también `LimpiarDuplicadosMenu.sql`).

---

## Fase 1 — Activar IIS en Windows

1. Presioná la tecla **Windows**, escribí `características de windows` y abrí **"Activar o desactivar las características de Windows"**.
2. En la ventana que aparece, buscá **"Internet Information Services"** en la lista (tiene un ícono de carpeta). Hacé clic en el **+** para expandirlo.
3. Dentro, expandí **"Herramientas de administración web"** y marcá la casilla **"Consola de administración de IIS"**.
4. Expandí **"Servicios World Wide Web"** y marcá **todas** las casillas que cuelgan de ahí (Características HTTP comunes, Desarrollo de aplicaciones, Estado y diagnóstico, Rendimiento, Seguridad). No hace daño dejarlas todas activadas.
5. Hacé clic en **Aceptar**. Windows va a instalar los componentes — aparece una barra de progreso, puede tardar unos minutos. Si al final te pide reiniciar la PC, reiniciá.
6. **Verificación:** abrí un navegador y entrá a `http://localhost`. Deberías ver la página azul por defecto de IIS ("IIS Windows Server"). Si la ves, IIS quedó bien instalado y activo.

---

## Fase 2 — Instalar el Hosting Bundle de .NET 10

Tu proyecto usa `.NET 10`, así que necesitás el Hosting Bundle de esa versión (no el 8 que aparece en el ejemplo de la clase).

1. Andá a `https://dotnet.microsoft.com/en-us/download/dotnet/10.0`.
2. Buscá la tabla de descargas para **Windows**. Ahí vas a ver varias opciones: SDK, Runtime, ASP.NET Core Runtime. Buscá específicamente el enlace que dice **"Hosting Bundle"** (suele estar junto a "ASP.NET Core Runtime 10.x.x"). Ese es el que necesitás — NO el SDK, NO el "Runtime" solo.
3. Descargá el instalador (algo como `dotnet-hosting-10.x.x-win.exe`) y ejecutalo.
4. En el instalador: **Next** → aceptar términos → **Install** → esperar → **Close**. Al final normalmente dice algo como "Please restart IIS after installation completes".
5. Abrí una terminal **como Administrador**: presioná Windows, escribí `cmd`, y en vez de Enter hacé clic derecho sobre "Símbolo del sistema" → **"Ejecutar como administrador"**.
6. En esa terminal escribí exactamente:
   ```
   iisreset
   ```
   y presioná Enter. Esperá a que diga `Internet services successfully restarted.`

---

## Fase 3 — Preparar el proyecto en Visual Studio

1. Si Visual Studio está abierto, cerralo.
2. Buscá el ícono de Visual Studio, hacé **clic derecho → "Ejecutar como administrador"**. Esto es obligatorio: si no lo abrís como administrador, más adelante el "Publish" a `C:\inetpub\wwwroot` va a fallar por permisos.
3. Abrí la solución del proyecto: el archivo `appSalYFuego.slnx`.
4. Arriba, en la barra de herramientas (donde están los botones de compilar/ejecutar), buscá el desplegable que dice **"Debug"** y cambialo a **"Release"**.
5. En el **Explorador de soluciones** (normalmente panel derecho), ubicá el proyecto llamado **"SalYFuego"** (el que tiene ícono de globo/mundo — es el proyecto web). Los otros dos, "SalYFuego.Application" y "SalYFuego.Infraestructure", NO se publican solos, son librerías que usa el proyecto web.
6. Clic derecho sobre **"SalYFuego"** (el proyecto, no toda la solución) → **"Build"**. Esperá a que en la barra de estado (abajo) diga **"Build succeeded"**. Si sale algún error, avisame antes de seguir.

---

## Fase 4 — Publicar desde Visual Studio

1. Clic derecho sobre el proyecto **"SalYFuego"** → **"Publish..."**.
2. Se abre el asistente de publicación. Como tipo de destino ("Target") elegí **"Folder"** → **Next**.
3. Te pide la ubicación ("Folder location"). Escribí exactamente:
   ```
   C:\inetpub\wwwroot\SalYFuego
   ```
   (tiene que quedar DENTRO de esa carpeta — es donde IIS busca los sitios por defecto).
4. Clic en **Finish**. Esto crea el perfil de publicación, pero todavía no publica nada.
5. Ahora ves la pantalla de resumen del perfil, con un botón **"Publish"** arriba a la derecha. Antes de tocarlo, podés hacer clic en **"Show all settings"** para confirmar: Configuration = **Release**, Target Framework = **net10.0**.
6. Hacé clic en **"Publish"**. Esperá — puede tardar entre 30 segundos y 2 minutos.
7. Al terminar, debería aparecer un mensaje verde: **"Publish succeeded"**, con un enlace **"Open folder"**. Hacé clic ahí.
8. **Verificación importante:** en esa carpeta (`C:\inetpub\wwwroot\SalYFuego`) tiene que estar el archivo **`appsettings.Production.json`** (junto a `SalYFuego.dll`, `web.config`, `appsettings.json`, la carpeta `wwwroot`, etc.). Si no está, el login no va a funcionar — avisame y lo revisamos.

---

## Fase 5 — Crear el Application Pool en IIS

1. Presioná Windows, escribí `IIS` y abrí **"Internet Information Services (IIS) Manager"**.
2. En el panel izquierdo ("Connections"), vas a ver el nombre de tu PC. Debajo, dos carpetas: **"Application Pools"** y **"Sites"**. Hacé clic en **"Application Pools"**.
3. En el panel del centro aparece la lista de pools existentes. Hacé **clic derecho** en cualquier espacio vacío de esa lista → **"Add Application Pool..."**.
4. En el diálogo que se abre:
   - **Name:** escribí `SalYFuegoPool`
   - **.NET CLR version:** cambiá el desplegable a **"No Managed Code"** ← este paso es el más importante de toda la guía; si lo dejás en una versión de .NET Framework clásico, el sitio no va a levantar.
   - **Managed pipeline mode:** dejalo en **"Integrated"**.
   - Dejá marcado **"Start Application Pool immediately"**.
5. Clic en **OK**. Deberías ver `SalYFuegoPool` en la lista, con Status **"Started"**.

---

## Fase 6 — Agregar la aplicación al sitio

1. En el panel izquierdo, expandí **"Sites"** → **"Default Web Site"**.
2. Hacé **clic derecho sobre "Default Web Site"** → **"Add Application..."** (ojo: NO es "Add Website", eso crearía un sitio aparte con otro puerto; "Add Application" la agrega dentro del sitio que ya existe, usando el mismo puerto 80).
3. En el diálogo:
   - **Alias:** escribí `SalYFuego` (esto va a formar parte de la dirección: `http://localhost/SalYFuego`).
   - **Application pool:** hacé clic en **"Select..."**, elegí `SalYFuegoPool` de la lista → OK.
   - **Physical path:** hacé clic en el botón **"..."** al lado del campo y navegá hasta `C:\inetpub\wwwroot\SalYFuego` (la misma carpeta donde publicaste en la Fase 4).
4. Clic en **OK** para cerrar el diálogo.
5. Deberías ver ahora, debajo de "Default Web Site", una carpetita con ícono de engranaje llamada **"SalYFuego"**.

---

## Fase 7 — Probar que el sitio abre

1. Hacé clic sobre **"SalYFuego"** (la aplicación recién creada) en el panel izquierdo.
2. En el panel derecho, buscá la sección **"Actions"** (la columna de la derecha del todo) y hacé clic en **"Browse *:80 (http)"** (el número de puerto puede variar según cómo tengas configurado tu Default Web Site).
3. Se abre el navegador en algo como `http://localhost/SalYFuego`. Si todo salió bien, ves la página de inicio de Sal y Fuego, igual que cuando corrés local con F5.
4. Probá iniciar sesión con `admin@salyfuego.com` / `Admin123*`. Si entra correctamente, confirmás que `appsettings.Production.json` quedó bien configurado.

---

## Fase 8 — Errores comunes y qué hacer

| Síntoma | Causa probable | Solución |
|---|---|---|
| Página en blanco o error **500.19** | Falta el Hosting Bundle, o no se reinició IIS después de instalarlo | Instalar el Hosting Bundle (Fase 2) y correr `iisreset` como administrador |
| Error **502.5 - Process Failure** | La app no pudo arrancar (falta config, o la base de datos no responde) | Ver el Visor de Eventos de Windows → Registros de Windows → Application, buscar el error más reciente de "IIS AspNetCore Module" |
| La página carga pero **el login da error 500** | Falta `appsettings.Production.json` en la carpeta publicada | Volver a publicar (Fase 4) y confirmar que el archivo esté en `C:\inetpub\wwwroot\SalYFuego` |
| **"No se puede conectar a la base de datos"** | El servicio de SQL Server no está corriendo, o el login `sa` no está habilitado | Abrir `services.msc`, confirmar que "SQL Server (MSSQLSERVER)" esté iniciado. Confirmar que la autenticación esté en modo mixto y que `sa` / `123456` funcione |
| Al crear la aplicación (Fase 6) dice que el alias ya existe | Quedó una aplicación anterior con el mismo nombre | En IIS Manager, clic derecho sobre esa aplicación vieja → Remove, y repetir la Fase 6 |
| Los estilos/imágenes no cargan (la página se ve "sin diseño") | La carpeta `wwwroot` no se copió bien al publicar | Volver a publicar (Fase 4); confirmar que dentro de `C:\inetpub\wwwroot\SalYFuego\wwwroot` estén las carpetas `css`, `js`, `lib`, `uploads` |
