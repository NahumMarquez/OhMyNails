using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OhMyNails.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OhMyNails.Services
{
    public class RecordatorioService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecordatorioService> _logger;

        public RecordatorioService(IServiceProvider serviceProvider, ILogger<RecordatorioService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ Servicio de recordatorios iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Llamar al método principal de recordatorios
                    await EnviarRecordatoriosAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error enviando recordatorios");
                }

                // 🔁 Esperar solo 1 minuto entre verificaciones (para pruebas)
                _logger.LogInformation("⏱ Esperando 1 minuto para la siguiente verificación...");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task EnviarRecordatoriosAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // 🧪 Durante pruebas: buscar citas del día actual
                // 🟢 En producción: usar DateTime.Now.Date.AddDays(2)
                DateTime fechaObjetivo = DateTime.Now.Date;

                _logger.LogInformation($"🕒 Verificando citas para la fecha: {fechaObjetivo:dd/MM/yyyy}");

                var citas = _context.Citas
                    .Where(c => c.Fecha.Date == fechaObjetivo)
                    .ToList();

                if (citas.Any())
                {
                    foreach (var cita in citas)
                    {
                        string mensaje = $"💅 ¡Hola {cita.Nombre}! Recordatorio: tu cita en *Oh my Nails!* es el {cita.Fecha:dd/MM/yyyy} a las {cita.Hora} para {cita.Categoria}.";
                        string link = $"https://wa.me/{cita.Telefono}?text={Uri.EscapeDataString(mensaje)}";

                        _logger.LogInformation($"📅 Recordatorio generado para {cita.Nombre} ({cita.Telefono})");
                        _logger.LogInformation($"🔗 Enlace WhatsApp: {link}");
                    }
                }
                else
                {
                    _logger.LogInformation("🔕 No hay citas para recordar hoy.");
                }
            }

            await Task.CompletedTask;
        }
    }
}
