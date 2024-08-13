namespace Tags1.Models.DTOs
{
    public class ActivoDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string TipoActivo { get; set; }

        public string Area { get; set; }

        public string? Supervisor { get; set; }
    }
}
