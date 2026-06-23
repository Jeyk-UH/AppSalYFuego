using Microsoft.EntityFrameworkCore;
using Sal_Fuego.Aplication.Profiles;
using Sal_Fuego.Aplication.Services.Implementations;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Implementations;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos
builder.Services.AddDbContext<SalYFuegoContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("SqlServerDataBase")));

// Registro de Repositorios
builder.Services.AddScoped<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddScoped<IRepositoryCombo, RepositoryCombo>();
builder.Services.AddScoped<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddScoped<IRepositoryProceso, RepositoryProceso>();

// Registro de Servicios
builder.Services.AddScoped<IServiceProducto, ServiceProducto>();
builder.Services.AddScoped<IServiceCombo, ServiceCombo>();
builder.Services.AddScoped<IServiceMenu, ServiceMenu>();
builder.Services.AddScoped<IServiceProceso, ServiceProceso>();

// Registro de AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<ProductoProfile>();
    cfg.AddProfile<ComboProfile>();
    cfg.AddProfile<MenuProfile>();
    cfg.AddProfile<ProcesoProfile>();
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();