using System;
using System.Collections.Generic;

namespace Tags1.Models;

public partial class Area
{
    public int Id { get; set; }

    public string Area1 { get; set; } = null!;

    public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
}
