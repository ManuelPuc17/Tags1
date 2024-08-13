using System;
using System.Collections.Generic;

namespace Tags1.Models;

public partial class Tipoactivo
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
}
