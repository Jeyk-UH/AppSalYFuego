using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Aplication.Config;
using Sal_Fuego.Aplication.Profiles;
using Sal_Fuego.Aplication.Services.Implementations;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Implementations;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;
using SalYFuego.Infraestructure.Repository.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos
builder.Services.AddDbContext<SalYFuegoContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("SqlServerDataBase")));

// Configuración de la aplicación (llave secreta para encriptar contraseñas, etc.)
builder.Services.Configure<AppConfig>(builder.Configuration);

// Registro de Repositorios
builder.Services.AddScoped<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddScoped<IRepositoryCombo, RepositoryCombo>();
builder.Services.AddScoped<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddScoped<IRepositoryProceso, RepositoryProceso>();
builder.Services.AddScoped<IRepositoryIngrediente, RepositoryIngrediente>();
builder.Services.AddScoped<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddScoped<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddScoped<IRepositoryRol, RepositoryRol>();
builder.Services.AddScoped<IRepositoryPedido, RepositoryPedido>();
builder.Services.AddScoped<IRepositoryMetodoPago, RepositoryMetodoPago>();

// Registro de Servicios
builder.Services.AddScoped<IServiceProducto, ServiceProducto>();
builder.Services.AddScoped<IServiceMenu, ServiceMenu>();
builder.Services.AddScoped<IServiceProceso, ServiceProceso>();
builder.Services.AddScoped<IServiceIngrediente, ServiceIngrediente>();
builder.Services.AddScoped<IServiceCategoria, ServiceCategoria>();
builder.Services.AddScoped<IServiceCombo, ServiceCombo>();
builder.Services.AddScoped<IServiceUsuario, ServiceUsuario>();
builder.Services.AddScoped<IServiceRol, ServiceRol>();
builder.Services.AddScoped<IServicePedido, ServicePedido>();
builder.Services.AddScoped<IServiceMetodoPago, ServiceMetodoPago>();

// Registro de AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<ProductoProfile>();
    cfg.AddProfile<ComboProfile>();
    cfg.AddProfile<MenuProfile>();
    cfg.AddProfile<ProcesoProfile>();
    cfg.AddProfile<CategoriaProfile>();
    cfg.AddProfile<EstacionProfile>();
    cfg.AddProfile<UsuarioProfile>();
});

// Seguridad: autenticación basada en cookies (sin ASP.NET Core Identity)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/Forbidden";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None
        });
});

// Permite validar el token antiforgery también cuando llega por header
// (necesario para el fetch() en JSON del punto de venta de Caja)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();