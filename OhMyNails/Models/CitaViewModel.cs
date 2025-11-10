using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OhMyNails.Models
{
    public class CitaViewModel
    {
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

        public List<string> HorasDisponibles
        {
            get
            {
                if (Fecha.DayOfWeek == DayOfWeek.Saturday)
                    return new List<string> { "2:00 PM", "4:30 PM" };
                else
                    return new List<string> { "10:30 AM", "2:00 PM", "4:30 PM" };
            }
        }

        public List<string> Categorias { get; } = new List<string>
        {
            "Softgel", "Rubber Gel", "Builder Gel"
        };

        public string? ImagenReferencia { get; set; }
        public string Estado { get; set; }

    }
}
