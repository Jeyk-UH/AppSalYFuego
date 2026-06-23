using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalYFuego.Infraestructure.Models;

namespace SalYFuego.Infraestructure.Data;

public partial class SalYFuegoContext : DbContext
{
    public SalYFuegoContext(DbContextOptions<SalYFuegoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Combo> Combo { get; set; }

    public virtual DbSet<ComboProducto> ComboProducto { get; set; }

    public virtual DbSet<DetallePedido> DetallePedido { get; set; }

    public virtual DbSet<DireccionUsuario> DireccionUsuario { get; set; }

    public virtual DbSet<Estacion> Estacion { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedido { get; set; }

    public virtual DbSet<HistorialEstacion> HistorialEstacion { get; set; }

    public virtual DbSet<HistorialEstadoPedido> HistorialEstadoPedido { get; set; }

    public virtual DbSet<Ingrediente> Ingrediente { get; set; }

    public virtual DbSet<Menu> Menu { get; set; }

    public virtual DbSet<MenuDisponibilidad> MenuDisponibilidad { get; set; }

    public virtual DbSet<MenuItem> MenuItem { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<ProcesoPreparacion> ProcesoPreparacion { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<ProductoImagen> ProductoImagen { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__CATEGORI__A3C02A100EA63BA5");

            entity.ToTable("CATEGORIA");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.IdCombo).HasName("PK__COMBO__D65BF2C87C74DB1B");

            entity.ToTable("COMBO");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PrecioEspecial).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Combo)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__COMBO__IdCategor__656C112C");
        });

        modelBuilder.Entity<ComboProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdCombo, e.IdProducto }).HasName("PK__COMBO_PR__C6C37BE9D3901A77");

            entity.ToTable("COMBO_PRODUCTO");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.ComboProducto)
                .HasForeignKey(d => d.IdCombo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__COMBO_PRO__IdCom__68487DD7");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ComboProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__COMBO_PRO__IdPro__693CA210");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DETALLE___E43646A54DB4E87F");

            entity.ToTable("DETALLE_PEDIDO");

            entity.Property(e => e.Observaciones)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.DetallePedido)
                .HasForeignKey(d => d.IdCombo)
                .HasConstraintName("FK__DETALLE_P__IdCom__08B54D69");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.DetallePedido)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DETALLE_P__IdPed__06CD04F7");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallePedido)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__DETALLE_P__IdPro__07C12930");
        });

        modelBuilder.Entity<DireccionUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDireccion).HasName("PK__DIRECCIO__1F8E0C76CB9A2813");

            entity.ToTable("DIRECCION_USUARIO");

            entity.Property(e => e.Alias)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Canton)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Referencia)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DireccionUsuario)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DIRECCION__IdUsu__52593CB8");
        });

        modelBuilder.Entity<Estacion>(entity =>
        {
            entity.HasKey(e => e.IdEstacion).HasName("PK__ESTACION__F0C18C42C4C7538F");

            entity.ToTable("ESTACION");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__ESTADO_P__FBB0EDC1EA0FF0F6");

            entity.ToTable("ESTADO_PEDIDO");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialEstacion>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__HISTORIA__9CC7DBB471D3F9C8");

            entity.ToTable("HISTORIAL_ESTACION");

            entity.Property(e => e.HoraIngreso).HasColumnType("datetime");
            entity.Property(e => e.HoraSalida).HasColumnType("datetime");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.HistorialEstacion)
                .HasForeignKey(d => d.IdEstacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HISTORIAL__IdEst__0C85DE4D");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.HistorialEstacion)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HISTORIAL__IdPed__0B91BA14");
        });

        modelBuilder.Entity<HistorialEstadoPedido>(entity =>
        {
            entity.HasKey(e => e.IdHistorialEstado).HasName("PK__HISTORIA__D672E2A9920C203F");

            entity.ToTable("HISTORIAL_ESTADO_PEDIDO");

            entity.Property(e => e.FechaHora).HasColumnType("datetime");
            entity.Property(e => e.Observacion)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.HistorialEstadoPedido)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HISTORIAL__IdEst__10566F31");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.HistorialEstadoPedido)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HISTORIAL__IdPed__0F624AF8");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialEstadoPedido)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HISTORIAL__IdUsu__114A936A");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("PK__INGREDIE__3DA4DD60E8C9ABB4");

            entity.ToTable("INGREDIENTE");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__MENU__4D7EA8E15FC909AF");

            entity.ToTable("MENU");

            entity.Property(e => e.EstaActivo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MenuDisponibilidad>(entity =>
        {
            entity.HasKey(e => e.IdDisponibilidad).HasName("PK__MENU_DIS__AE82DB172087042D");

            entity.ToTable("MENU_DISPONIBILIDAD");

            entity.Property(e => e.DiaSemana)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MenuDisponibilidad)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENU_DISP__IdMen__6EF57B66");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.IdMenuItem).HasName("PK__MENU_ITE__77CE2ED6CBBF5462");

            entity.ToTable("MENU_ITEM");

            entity.HasOne(d => d.IdComboNavigation).WithMany(p => p.MenuItem)
                .HasForeignKey(d => d.IdCombo)
                .HasConstraintName("FK__MENU_ITEM__IdCom__73BA3083");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MenuItem)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MENU_ITEM__IdMen__71D1E811");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.MenuItem)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__MENU_ITEM__IdPro__72C60C4A");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__METODO_P__6F49A9BEA5829C50");

            entity.ToTable("METODO_PAGO");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreMetodo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__PAGO__FC851A3A8801E3C8");

            entity.ToTable("PAGO");

            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.MontoPagado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoTarjeta)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UltimosDigitos)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Vuelto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PAGO__IdMetodoPa__151B244E");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PAGO__IdPedido__14270015");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__PEDIDO__9D335DC33358290C");

            entity.ToTable("PEDIDO");

            entity.Property(e => e.CodigoOrden)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CostoEnvio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaPedido).HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoEntrega)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrigenPedido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.PedidoIdClienteNavigation)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__PEDIDO__IdClient__02084FDA");

            entity.HasOne(d => d.IdDireccionEntregaNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdDireccionEntrega)
                .HasConstraintName("FK__PEDIDO__IdDirecc__03F0984C");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.PedidoIdEmpleadoNavigation)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__PEDIDO__IdEmplea__02FC7413");

            entity.HasOne(d => d.IdEstacionActualNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdEstacionActual)
                .HasConstraintName("FK__PEDIDO__IdEstaci__01142BA1");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PEDIDO__IdEstado__00200768");
        });

        modelBuilder.Entity<ProcesoPreparacion>(entity =>
        {
            entity.HasKey(e => e.IdProceso).HasName("PK__PROCESO___036D0743770A5FD8");

            entity.ToTable("PROCESO_PREPARACION");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.ProcesoPreparacion)
                .HasForeignKey(d => d.IdEstacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROCESO_P__IdEst__2BFE89A6");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProcesoPreparacion)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROCESO_P__IdPro__2B0A656D");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__PRODUCTO__09889210EEC2DDF3");

            entity.ToTable("PRODUCTO");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCTO__IdCate__59FA5E80");

            entity.HasMany(d => d.IdIngrediente).WithMany(p => p.IdProducto)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoIngrediente",
                    r => r.HasOne<Ingrediente>().WithMany()
                        .HasForeignKey("IdIngrediente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PRODUCTO___IdIng__5DCAEF64"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PRODUCTO___IdPro__5CD6CB2B"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdIngrediente").HasName("PK__PRODUCTO__1A52DFC62149740E");
                        j.ToTable("PRODUCTO_INGREDIENTE");
                    });
        });

        modelBuilder.Entity<ProductoImagen>(entity =>
        {
            entity.HasKey(e => e.IdImagen).HasName("PK__PRODUCTO__B42D8F2AA6253FBA");

            entity.ToTable("PRODUCTO_IMAGEN");

            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoImagen)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCTO___IdPro__619B8048");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__ROL__2A49584CA0C68830");

            entity.ToTable("ROL");

            entity.Property(e => e.NombreRol)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__5B65BF97F15FF746");

            entity.ToTable("USUARIO");

            entity.HasIndex(e => e.Correo, "UQ__USUARIO__60695A1913D30B8A").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ContrasenaHash)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TokenExpiracion).HasColumnType("datetime");
            entity.Property(e => e.TokenRecuperacion)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USUARIO__IdRol__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
