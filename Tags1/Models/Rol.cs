using System;
using System.Collections.Generic;

namespace Tags1.Models;

public partial class Rol
{
    public int Id { get; set; }

    public string Rol1 { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
