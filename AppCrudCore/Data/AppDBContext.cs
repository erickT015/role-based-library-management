using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using AppCrudCore.Models;

namespace AppCrudCore.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Libro> Libro { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<TransaccionBiblioteca> TransaccionBiblioteca { get; set; }
        public DbSet<TransaccionDetalle> TransaccionDetalle { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //==PROPIEDADES DE LA TABLA EMPLEADO
            modelBuilder.Entity<Empleado>(tb =>
            {
                //propiedades para columna "col" idEmpleado
                tb.HasKey(col => col.IdEmpleado); //es primary
                tb.Property(col => col.IdEmpleado)
                .UseIdentityColumn() //incremental (1,1)
                .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                tb.Property(col => col.Cedula)
                .IsRequired()
                .HasMaxLength(20);
                tb.HasIndex(col => col.Cedula).IsUnique();

                tb.Property(col => col.NombreCompleto).HasMaxLength(50);

                tb.Property(col => col.Correo).HasMaxLength(50);

                tb.Property(col => col.PasswordHash)
                .IsRequired() //no nulo
                .HasMaxLength(255); //tamaño de 255

                tb.HasOne(col => col.Rol)
                .WithMany() //se relacinoa con muchos
                .HasForeignKey(col => col.RolId);
            });

            modelBuilder.Entity<Empleado>().ToTable("Empleado"); //le asignamos el nombre, a la fuerza no por convencion

            //==PROPIEDADES DE LA TABLA ROL
            modelBuilder.Entity<Rol>(tb =>
            {
                //propiedades para columna "col" IdRol
                tb.HasKey(col => col.IdRol); //es primary
                tb.Property(col => col.IdRol)
                .UseIdentityColumn() //incremental (1,1)
                .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                tb.Property(col => col.Nombre)
                .HasMaxLength(50)// maximo 50
                .IsRequired(); //no nulo

                tb.HasIndex(col => col.Nombre).IsUnique();// nombre unico

                tb.Property(col => col.Descripcion).HasMaxLength(100);

                tb.Property(col => col.Activo)
                .IsRequired()
                .HasDefaultValue(true);

                // ============================
                // SEED DE DATOS (ROLES INICIALES)
                // ============================
                tb.HasData(
                    new Rol
                    {
                        IdRol = 1,
                        Nombre = "Admin",
                        Descripcion = "Control completo del sistema",
                        Activo = true
                    },
                    new Rol
                    {
                        IdRol = 2,
                        Nombre = "Empleado",
                        Descripcion = "Gestión de libros y clientes",
                        Activo = true
                    },
                    new Rol
                    {
                        IdRol = 3,
                        Nombre = "Cliente",
                        Descripcion = "Rol lógico para filtro y sin acceso por el momento",
                        Activo = true
                    }
                );

            });

            //==PROPIEDADES DE LA TABLA CLIENTE
            modelBuilder.Entity<Cliente>(tb =>
            {
                //propiedaes para columnas
                tb.HasKey(col => col.IdCliente);
                tb.Property(col => col.IdCliente)
               .UseIdentityColumn() //incremental (1,1)
               .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                tb.HasIndex(col => col.Cedula).IsUnique();

                tb.HasOne(col => col.Rol)
                .WithMany()
                .HasForeignKey(col => col.RolId);

                tb.HasIndex(col => col.Correo).IsUnique();
            });

            //==PROPIEDADES DE LA TABLA LIBRO
            modelBuilder.Entity<Libro>(tb =>
            {
                tb.HasKey(col => col.IdLibro);
                tb.Property(col => col.IdLibro)
               .UseIdentityColumn() //incremental (1,1)
               .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                tb.HasIndex(col => col.ISBN).IsUnique();
                tb.HasIndex(col => col.Titulo);

                tb.HasOne(col => col.Categoria)
                .WithMany()
                .HasForeignKey(col => col.CategoriaId);

                tb.Property(col => col.PrecioVenta).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Libro>().ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Libro_Stock_Total",
                    "StockPrestamo + StockVenta <= StockTotal"
                );

                tb.HasCheckConstraint(
                    "CK_Libro_Stock_NoNegativo",
                    "StockTotal >= 0 AND StockPrestamo >= 0 AND StockVenta >= 0"
                );
            });

            //==PROPIEDADES DE LA TABLA CATEGORIA
            modelBuilder.Entity<Categoria>(tb =>
            {
                //propiedades para columna "col" Idcategoria
                tb.HasKey(col => col.IdCategoria); //es primary
                tb.Property(col => col.IdCategoria)
                .UseIdentityColumn() //incremental (1,1)
                .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                tb.Property(col => col.Nombre)
                .HasMaxLength(30)// maximo 50
                .IsRequired(); //no nulo
                tb.HasIndex(col => col.Nombre).IsUnique();// nombre unico

                tb.Property(col => col.Activo)
                .IsRequired()
                .HasDefaultValue(true);

                // ============================
                // SEED DE DATOS (CATEGORIAS INICIALES)
                // ============================
                tb.HasData(
                    new Categoria { IdCategoria = 1, Nombre = "Literatura", Activo = true},
                    new Categoria { IdCategoria = 2, Nombre = "Ciencia", Activo = true },
                    new Categoria { IdCategoria = 3, Nombre = "Tecnología", Activo = true },
                    new Categoria { IdCategoria = 4, Nombre = "Historia", Activo = true },
                    new Categoria { IdCategoria = 5, Nombre = "Fantasía", Activo = true },
                    new Categoria { IdCategoria = 6, Nombre = "Ciencia Ficción", Activo = true },
                    new Categoria { IdCategoria = 7, Nombre = "Educación", Activo = true },
                    new Categoria { IdCategoria = 8, Nombre = "Infantil", Activo = true }
                );

            });


            //==PROPIEDADES DE TABLA TRANSACCION BIBLIOTECA
            modelBuilder.Entity<TransaccionBiblioteca>(tb =>
            {
                //Primary
                tb.HasKey(col => col.IdTransaccionBiblioteca);
                tb.Property(col => col.IdTransaccionBiblioteca)
                .UseIdentityColumn() //incremental (1,1)
                .ValueGeneratedOnAdd(); //no envies valor, la db lo gesyiona

                //numero de transaccion
                tb.Property(col => col.NumeroTransaccion)
                .IsRequired()
                .HasMaxLength(50);
                tb.HasIndex(col => col.NumeroTransaccion)
                .IsUnique();


                // Formato correcto para decimales
                tb.Property(col => col.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                tb.Property(col => col.MontoPagado)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                // referencia de pago
                tb.Property(col => col.ReferenciaPago)
                    .HasMaxLength(100);


                // Observaaciones
                tb.Property(col => col.Observaciones)
                    .HasMaxLength(500);


                // Fechas
                tb.Property(col => col.FechaCreacion).IsRequired();
                tb.HasIndex(col => col.FechaCreacion);

                tb.Property(col => col.FechaDevolucion).IsRequired(false);

                tb.Property(col => col.FechaCompletada).IsRequired(false);

                //LLaves foraneas
                tb.HasOne(col => col.Cliente)//FK
               .WithMany()
               .HasForeignKey(col => col.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);


                tb.HasOne(col => col.Empleado)//FK
                .WithMany()
                .HasForeignKey(col => col.EmpleadoId)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            //==PROPIEDADES DE LA TABLA TRANSACCION DETALLE
            modelBuilder.Entity<TransaccionDetalle>(tb =>
            {
                tb.HasKey(col => col.IdTransaccionDetalle);
                tb.Property(col => col.IdTransaccionDetalle)
                .UseIdentityColumn() 
                .ValueGeneratedOnAdd();

                tb.Property(col => col.PrecioUnitario).HasColumnType("decimal(18,2)");

                tb.Property(col => col.Subtotal).HasColumnType("decimal(18,2)");

                tb.HasOne(col => col.Libro) //fk
                .WithMany()
                .HasForeignKey(col => col.LibroId)
                 .OnDelete(DeleteBehavior.Restrict);


                tb.HasOne(col => col.TransaccionBiblioteca) //fk
              .WithMany( t => t.Detalles)
              .HasForeignKey(col => col.TransaccionBibliotecaId)
               .OnDelete(DeleteBehavior.Cascade);

            });
        }

    }
}
