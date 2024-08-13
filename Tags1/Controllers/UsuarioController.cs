using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tags1.Models;
using Tags1.Models.DTOs;

namespace Tags1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Tag1Context _context;
        public UsuarioController(Tag1Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Usuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ListaUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.RolNavigation)
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Rol = u.RolNavigation.Rol1
                })
                .ToListAsync();

            if (usuarios == null || !usuarios.Any())
            {
                return NotFound(new { message = "No se encontraron usuarios en la base de datos." });
            }

            foreach (var usuario in usuarios)
            {
                if (usuario.Rol == null)
                {
                    return StatusCode(500, new { message = "Error al recuperar la información del rol para uno o más usuarios." });
                }
            }

            return Ok(usuarios);
        }



        [HttpGet]
        [Route("UsuarioId")]
        public async Task<ActionResult<UsuarioDTO>> UsuariosId(int usuario_id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.RolNavigation)
                .FirstOrDefaultAsync(u => u.Id == usuario_id);

            if (usuario == null)
            {
                return NotFound(new { message = "No se encontraron usuarios en la base de datos." });
            }

            if (usuario.RolNavigation == null)
            {
                return StatusCode(500, new { message = "Error al recuperar la información del rol para el usuario." });
            }

            return Ok(new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.RolNavigation.Rol1
            });
        }

        [HttpPut]
        [Route("EditarUsuario")]
        public async Task<IActionResult> EditarUsuario(int user_id, [FromBody] UsuarioDTO usuarioDTO)
        {
            var usuarioExiste = await _context.Usuarios.FindAsync(user_id);
            if(usuarioExiste == null)
            {
                return NotFound(new { message = "No se encontraron usuarios en la base de datos." });
            }

            var CorreoExiste = await _context.Usuarios.AnyAsync(u => u.Correo == usuarioDTO.Correo && u.Id != user_id);
            if (CorreoExiste)
            {
                return NotFound(new { message = "El correo ya esta registrado." });
            }
            usuarioExiste.Correo = usuarioDTO.Correo;
            usuarioExiste.Nombre = usuarioDTO.Nombre;
            usuarioExiste.Rol = usuarioDTO.RolId;

            _context.Usuarios.Update(usuarioExiste);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });

        }

        [HttpDelete]
        [Route("EliminarUsuario")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Actualizar los activos asignados al supervisor para poner supervisor en null
            var activosSupervisados = await _context.Activos
                .Where(a => a.Supervisor == id)
                .ToListAsync();

            foreach (var activo in activosSupervisados)
            {
                activo.Supervisor = null; // Poner supervisor en null
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Eliminar el usuario
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }

}

