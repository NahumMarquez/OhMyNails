using OhMyNails.Data;
using OhMyNails.Models;

namespace OhMyNails.Services
{
    public class CitaService
    {
        private readonly ApplicationDbContext _context;

        public CitaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Obtiene todas las citas (para el panel Admin)
        public List<Cita> ObtenerCitas()
        {
            return _context.Citas.ToList();
        }

        // 🔹 Agrega una nueva cita
        public void AgregarCita(Cita cita)
        {
            _context.Citas.Add(cita);
            _context.SaveChanges();
        }

        // 🔹 Verifica si un horario está ocupado
        public bool EstaOcupada(DateTime fecha, string hora)
        {
            // Busca si ya existe una cita en esa fecha y hora
            return _context.Citas.Any(c => c.Fecha.Date == fecha.Date && c.Hora == hora);
        }

        // 🔹 Devuelve los horarios disponibles según el día de la semana
        public List<string> ObtenerHorarios(DateTime fecha)
        {
            var horarios = new List<string>();

            if (fecha.DayOfWeek == DayOfWeek.Sunday)
                return horarios; // domingo: sin horarios

            if (fecha.DayOfWeek == DayOfWeek.Saturday)
            {
                horarios.Add("14:00");
                horarios.Add("16:30");
            }
            else
            {
                horarios.Add("10:30");
                horarios.Add("14:00");
                horarios.Add("16:30");
            }

            return horarios;
        }
    }
}
