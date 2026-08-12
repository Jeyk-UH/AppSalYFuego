using AutoMapper;
using Sal_Fuego.Aplication.DTOs;
using Sal_Fuego.Aplication.Services.Interfaces;
using Sal_Fuego.Infraestructure.Repository.Interfaces;
using SalYFuego.Infraestructure.Models;

namespace Sal_Fuego.Aplication.Services.Implementations
{
    public class ServiceCombo : IServiceCombo
    {
        private readonly IRepositoryCombo _repository;
        private readonly IRepositoryProducto _repositoryProducto;
        private readonly IMapper _mapper;

        public ServiceCombo(
            IRepositoryCombo repository,
            IRepositoryProducto repositoryProducto,
            IMapper mapper)
        {
            _repository = repository;
            _repositoryProducto = repositoryProducto;
            _mapper = mapper;
        }

        public async Task<ICollection<ComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        public async Task<ComboDTO?> FindByIdAsync(int id)
        {
            var combo = await _repository.FindByIdAsync(id);
            if (combo == null) return null;
            return _mapper.Map<ComboDTO>(combo);
        }

        // Cargar datos del combo para el formulario de edición
        public async Task<ComboFormDTO?> FindFormByIdAsync(int id)
        {
            var combo = await _repository.FindByIdAsync(id);
            if (combo == null) return null;

            return new ComboFormDTO
            {
                IdCombo = combo.IdCombo,
                Nombre = combo.Nombre,
                Descripcion = combo.Descripcion,
                PrecioEspecial = combo.PrecioEspecial,
                Activo = combo.Activo,
                IdCategoria = combo.IdCategoria,
                UrlImagenActual = combo.UrlImagen,
                ProductosSeleccionados = combo.ComboProducto
                    .Select(cp => new ComboProductoFormDTO
                    {
                        IdProducto = cp.IdProducto,
                        NombreProducto = cp.IdProductoNavigation.Nombre,
                        Cantidad = cp.Cantidad
                    }).ToList()
            };
        }

        // Agregar nuevo combo con imagen y productos
        public async Task AddAsync(ComboFormDTO dto, string wwwrootPath)
        {
            var combo = new Combo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioEspecial = dto.PrecioEspecial,
                Activo = dto.Activo,
                IdCategoria = dto.IdCategoria
            };

            // Guardar imagen si se subió una
            if (dto.ImagenFile != null && dto.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(wwwrootPath, "uploads", "combos");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = $"{Guid.NewGuid()}" +
                    $"{Path.GetExtension(dto.ImagenFile.FileName)}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await dto.ImagenFile.CopyToAsync(stream);

                combo.UrlImagen = $"/uploads/combos/{nombreArchivo}";
            }

            // Agregar productos al combo
            foreach (var item in dto.ProductosSeleccionados)
            {
                combo.ComboProducto.Add(new ComboProducto
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                });
            }

            await _repository.AddAsync(combo);
        }

        // Actualizar combo existente
        public async Task UpdateAsync(ComboFormDTO dto, string wwwrootPath)
        {
            var combo = await _repository.FindByIdAsync(dto.IdCombo);
            if (combo == null) return;

            combo.Nombre = dto.Nombre;
            combo.Descripcion = dto.Descripcion;
            combo.PrecioEspecial = dto.PrecioEspecial;
            combo.Activo = dto.Activo;
            combo.IdCategoria = dto.IdCategoria;

            // Actualizar imagen si se subió una nueva
            if (dto.ImagenFile != null && dto.ImagenFile.Length > 0)
            {
                var carpeta = Path.Combine(wwwrootPath, "uploads", "combos");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = $"{Guid.NewGuid()}" +
                    $"{Path.GetExtension(dto.ImagenFile.FileName)}";
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    await dto.ImagenFile.CopyToAsync(stream);

                combo.UrlImagen = $"/uploads/combos/{nombreArchivo}";
            }

            // Actualizar productos del combo
            combo.ComboProducto.Clear();
            foreach (var item in dto.ProductosSeleccionados)
            {
                combo.ComboProducto.Add(new ComboProducto
                {
                    IdCombo = combo.IdCombo,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                });
            }

            await _repository.UpdateAsync(combo);
        }

        // Desactivar combo
        public async Task DesactivarAsync(int id)
        {
            var combo = await _repository.FindByIdAsync(id);
            if (combo != null)
                await _repository.DesactivarAsync(combo);
        }
    }
}