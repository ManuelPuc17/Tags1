using System;
using System.Collections.Generic;

namespace Tags1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public int Rol { get; set; }

    public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();

    public virtual Rol RolNavigation { get; set; } = null!;
}
