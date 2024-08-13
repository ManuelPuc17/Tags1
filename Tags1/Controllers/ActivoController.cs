using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tags1.Models;
using Tags1.Models.DTOs;

namespace Tags1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivoController : ControllerBase
    {
        private readonly Tag1Context _context;

        public ActivoController(Tag1Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Activos")]
        public async Task<ActionResult<IEnumerable<ActivoDTO>>> Activos()
        {
            var activos = await _context.Activos
                .Include(a => a.TipoActivoNavigation)
                .Include(a => a.AreaNavigation)
                .Include(a => a.SupervisorNavigation)
                .Select(a => new ActivoDTO
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    TipoActivo = a.TipoActivoNavigation.Tipo,  // tipo de activo
                    Area = a.AreaNavigation.Area1,  // Nombre del area
                    Supervisor = a.SupervisorNavigation != null ? a.SupervisorNavigation.Nombre : null  // Nombre del supervisor
                })
                .ToListAsync();

            if (activos == null || !activos.Any())
            {
                return Ok(new { message = "No se encontraron activos en la base de datos." });
            }

            return Ok(activos);
        }

        [HttpGet]
        [Route("ActivoId")]
        public async Task<ActionResult<ActivoDTO>> Activo(int id_activo)
        {
            // Buscar el activo con el ID 
            var activo = await _context.Activos
                .Include(a => a.TipoActivoNavigation)
                .Include(a => a.AreaNavigation)
                .Include(a => a.SupervisorNavigation)
                .FirstOrDefaultAsync(a => a.Id == id_activo);

            // Verificar si el activo existe
            if (activo == null)
            {
                return NotFound(new { message = "No se encontró el activo con el ID proporcionado." });
            }

            // Retornar el activo como un DTO
            return Ok(new ActivoDTO
            {
                Id = activo.Id,
                Nombre = activo.Nombre,
                TipoActivo = activo.TipoActivoNavigation.Tipo,
                Area = activo.AreaNavigation.Area1,
                Supervisor = activo.SupervisorNavigation?.Nombre  // Puede ser nulo si no hay supervisor
            });
        }


        [HttpPost]
        [Route("RegistrarActivo")]
        public async Task<ActionResult> RegistrarActivo([FromBody] ActivoDTO nuevoActivo)
        {
            // Convertir las propiedades del DTO a los tipos correctos
            int tipoActivoId;
            int areaId;
            int? supervisorId = null;

            if (!int.TryParse(nuevoActivo.TipoActivo, out tipoActivoId))
            {
                return BadRequest(new { message = "Tipo de activo inválido." });
            }

            if (!int.TryParse(nuevoActivo.Area, out areaId))
            {
                return BadRequest(new { message = "Área inválida." });
            }

            if (!string.IsNullOrEmpty(nuevoActivo.Supervisor))
            {
                if (!int.TryParse(nuevoActivo.Supervisor, out int parsedSupervisorId))
                {
                    return BadRequest(new { message = "Supervisor inválido." });
                }
                supervisorId = parsedSupervisorId;
            }

            // Crear el nuevo activo
            var activo = new Activo
            {
                Nombre = nuevoActivo.Nombre,
                TipoActivo = tipoActivoId,
                Area = areaId,
                Supervisor = supervisorId
            };

            // Agregar el activo a la base de datos
            _context.Activos.Add(activo);
            await _context.SaveChangesAsync();

            // Retornar una respuesta exitosa
            return Ok(new { message = "Activo registrado con éxito.", activo.Id });
        }



        [HttpPut]
        [Route("EditarActivo")]
        public async Task<ActionResult> EditarActivo(int id, [FromBody] ActivoDTO activoEditado)
        {
            // Buscar el activo existente por su ID
            var activoExistente = await _context.Activos
                .Include(a => a.TipoActivoNavigation)
                .Include(a => a.AreaNavigation)
                .Include(a => a.SupervisorNavigation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activoExistente == null)
            {
                return NotFound(new { message = "El activo no fue encontrado." });
            }

            // Verificar si el tipo de activo, área y supervisor existen
            var tipoActivo = await _context.Tipoactivos.FirstOrDefaultAsync(t => t.Tipo == activoEditado.TipoActivo);
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Area1 == activoEditado.Area);
            var supervisor = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == activoEditado.Supervisor);

            if (tipoActivo == null || area == null || supervisor == null)
            {
                return BadRequest(new { message = "Tipo de activo, área o supervisor no válidos." });
            }

            // Actualizar los campos del activo existente
            activoExistente.Nombre = activoEditado.Nombre;
            activoExistente.TipoActivo = tipoActivo.Id;
            activoExistente.Area = area.Id;
            activoExistente.Supervisor = supervisor?.Id;  // Puede ser nulo si no se asigna un supervisor

            // Guardar los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al actualizar el activo en la base de datos.", ex.InnerException?.Message });
            }

            // Retornar una respuesta exitosa
            return Ok(new { message = "Activo actualizado con éxito." });
        }


        [HttpDelete]
        [Route("EliminarActivo")]
        public async Task<ActionResult> EliminarActivo(int id)
        {
            // Buscar el activo existente por su ID
            var activo = await _context.Activos.FindAsync(id);

            if (activo == null)
            {
                return NotFound(new { message = "El activo no fue encontrado." });
            }

            // Eliminar el activo
            _context.Activos.Remove(activo);

            // Guardar los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al eliminar el activo de la base de datos.", ex.InnerException?.Message });
            }

            // Retornar una respuesta exitosa
            return Ok(new { message = "Activo eliminado con éxito." });
        }

    }
}
