using System;
using System.ComponentModel.DataAnnotations;

namespace OhMyNails.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }  

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public string Hora { get; set; }

        [Required]
        public string Categoria { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? Imagen { get; set; }
        public string Estado { get; set; } = "Activa";

        public string? ImagenReferencia { get; set; }
    }
}
