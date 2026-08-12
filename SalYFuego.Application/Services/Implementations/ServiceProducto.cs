using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IRepositoryIngrediente _repositoryIngrediente;
        private readonly IMapper _mapper;

        public ServiceProducto(
            IRepositoryProducto repository,
            IRepositoryIngrediente repositoryIngrediente,
            IMapper mapper)
        {
            _repository = repository;
            _repositoryIngrediente = repositoryIngrediente;
            _mapper = mapper;
        }

        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }

        public async Task<ProductoDTO?> FindByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto == null) return null;
            return _mapper.Map<ProductoDTO>(producto);
        }

        // Cargar datos del producto para el formulario de edición
        public async Task<ProductoFormDTO?> FindFormByIdAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto == null) return null;

            return new ProductoFormDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Activo = producto.Activo,
                IdCategoria = producto.IdCategoria,
                ImagenActual = producto.ProductoImagen
                    .FirstOrDefault(i => i.EsPrincipal == true)?.UrlImagen,
                IngredientesSeleccionados = producto.IdIngrediente
                    .Select(i => i.IdIngrediente)
                    .ToList()
            };
        }

        // Agregar nuevo producto con imagen e ingredientes
        public async Task AddAsync(ProductoFormDTO dto, string wwwrootPath)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Activo = dto.Activo,
                IdCategoria = dto.IdCategoria
            };

            // Guardar imagen si se subió una
            if (dto.ImagenFile != null && dto.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(wwwrootPath, "uploads", "productos");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImagenFile.FileName)}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await dto.ImagenFile.CopyToAsync(stream);

                producto.ProductoImagen.Add(new ProductoImagen
                {
                    UrlImagen = $"/uploads/productos/{nombreArchivo}",
                    EsPrincipal = true
                });
            }

            // Asignar ingredientes seleccionados
            if (dto.IngredientesSeleccionados.Any())
            {
                var ingredientes = await _repositoryIngrediente.ListAsync();
                foreach (var id in dto.IngredientesSeleccionados)
                {
                    var ingrediente = ingredientes.FirstOrDefault(i => i.IdIngrediente == id);
                    if (ingrediente != null)
                        producto.IdIngrediente.Add(ingrediente);
                }
            }

            await _repository.AddAsync(producto);
        }

        // Actualizar producto existente
        public async Task UpdateAsync(ProductoFormDTO dto, string wwwrootPath)
        {
            var producto = await _repository.FindByIdAsync(dto.IdProducto);
            if (producto == null) return;

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Precio = dto.Precio;
            producto.Activo = dto.Activo;
            producto.IdCategoria = dto.IdCategoria;

            // Actualizar imagen si se subió una nueva
            if (dto.ImagenFile != null && dto.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(wwwrootPath, "uploads", "productos");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImagenFile.FileName)}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await dto.ImagenFile.CopyToAsync(stream);

                // Reemplazar imagen principal
                var imagenActual = producto.ProductoImagen
                    .FirstOrDefault(i => i.EsPrincipal == true);
                if (imagenActual != null)
                    imagenActual.UrlImagen = $"/uploads/productos/{nombreArchivo}";
                else
                    producto.ProductoImagen.Add(new ProductoImagen
                    {
                        UrlImagen = $"/uploads/productos/{nombreArchivo}",
                        EsPrincipal = true
                    });
            }

            // Actualizar ingredientes
            producto.IdIngrediente.Clear();
            if (dto.IngredientesSeleccionados.Any())
            {
                var ingredientes = await _repositoryIngrediente.ListAsync();
                foreach (var id in dto.IngredientesSeleccionados)
                {
                    var ingrediente = ingredientes.FirstOrDefault(i => i.IdIngrediente == id);
                    if (ingrediente != null)
                        producto.IdIngrediente.Add(ingrediente);
                }
            }

            await _repository.UpdateAsync(producto);
        }

        // Desactivar producto por id
        public async Task DesactivarAsync(int id)
        {
            var producto = await _repository.FindByIdAsync(id);
            if (producto != null)
                await _repository.DesactivarAsync(producto);
        }
    }
}