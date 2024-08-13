using System;
using System.Collections.Generic;

namespace Tags1.Models;

public partial class Activo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int TipoActivo { get; set; }

    public int Area { get; set; }

    public int? Supervisor { get; set; }

    public virtual Area AreaNavigation { get; set; } = null!;

    public virtual Usuario SupervisorNavigation { get; set; } = null!;

    public virtual Tipoactivo TipoActivoNavigation { get; set; } = null!;
}
