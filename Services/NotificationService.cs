using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Notifications.Wpf;
using SysMarkModerno.Models;

namespace SysMarkModerno.Services
{
    public class NotificationService : IDisposable
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationManager _notificationManager;
        private readonly Timer _timer;

        public NotificationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _notificationManager = new NotificationManager();
            
            _timer = new Timer(3600000); // 1 hora
            _timer.Elapsed += async (s, e) => await VerificarLlamadasPendientesAsync();
            _timer.AutoReset = true;
        }

        public void IniciarMonitoreo()
        {
            _timer.Start();
            Task.Run(async () => await VerificarLlamadasPendientesAsync());
        }

        public void DetenerMonitoreo()
        {
            _timer.Stop();
        }

        private async Task VerificarLlamadasPendientesAsync()
        {
            try
            {
                var llamadasHoy = await _databaseService.ObtenerLlamadasProximasAsync(0);
                var llamadasProximas = await _databaseService.ObtenerLlamadasProximasAsync(3);

                foreach (var llamada in llamadasHoy)
                {
                    if (llamada.ProximaLlamada?.Date == DateTime.Today)
                    {
                        MostrarNotificacion(
                            "⚠️ Llamada Programada HOY",
                            $"{llamada.Empresa} - {llamada.Contacto}\n{llamada.ProximaLlamada:HH:mm}",
                            NotificationType.Warning
                        );
                    }
                }

                var llamadasProximosDias = llamadasProximas.Where(l => 
                    l.ProximaLlamada?.Date > DateTime.Today && 
                    l.ProximaLlamada?.Date <= DateTime.Today.AddDays(3));

                if (llamadasProximosDias.Any())
                {
                    var cantidad = llamadasProximosDias.Count();
                    MostrarNotificacion(
                        "📅 Llamadas Próximas",
                        $"Tienes {cantidad} llamada(s) programada(s) en los próximos 3 días",
                        NotificationType.Information
                    );
                }

                // --------- SEGUIMIENTOS STARTUP LOGIC ---------
                var seguimientos = await _databaseService.ObtenerSeguimientosProximosAsync(3); // Fetch seguimientos upcoming in 3 days or PAST
                var seguimientosAtrasados = seguimientos.Where(s => 
                    s.Seguimiento?.Date < DateTime.Today).ToList();
                var seguimientosProximos = seguimientos.Where(s => 
                    s.Seguimiento?.Date >= DateTime.Today && 
                    s.Seguimiento?.Date <= DateTime.Today.AddDays(3)).ToList();

                if (seguimientosAtrasados.Any())
                {
                    MostrarNotificacion(
                        "⚠️ Seguimientos Atrasados",
                        $"Tienes {seguimientosAtrasados.Count} seguimiento(s) atrasados pendientes",
                        NotificationType.Error
                    );
                }

                if (seguimientosProximos.Any())
                {
                    MostrarNotificacion(
                        "📅 Seguimientos Pendientes",
                        $"Tienes {seguimientosProximos.Count} seguimiento(s) pendientes en los próximos 3 días",
                        NotificationType.Information
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar llamadas: {ex.Message}");
            }
        }

        public void MostrarNotificacion(string titulo, string mensaje, NotificationType tipo = NotificationType.Information)
        {
            var notificationContent = new NotificationContent
            {
                Title = titulo,
                Message = mensaje,
                Type = tipo
            };

            _notificationManager.Show(notificationContent, expirationTime: TimeSpan.FromSeconds(10));
        }

        public void MostrarExito(string mensaje)
        {
            MostrarNotificacion("✅ Éxito", mensaje, NotificationType.Success);
        }

        public void MostrarError(string mensaje)
        {
            MostrarNotificacion("❌ Error", mensaje, NotificationType.Error);
        }

        public void MostrarAdvertencia(string mensaje)
        {
            MostrarNotificacion("⚠️ Advertencia", mensaje, NotificationType.Warning);
        }

        public async Task NotificarLlamadaInmediataAsync(Marketing marketing)
        {
            if (marketing.ProximaLlamada.HasValue)
            {
                var horasRestantes = (marketing.ProximaLlamada.Value - DateTime.Now).TotalHours;

                if (horasRestantes <= 2 && horasRestantes > 0)
                {
                    MostrarNotificacion(
                        "Llamada Próxima",
                        $"{marketing.Empresa} - {marketing.Contacto}\nEn {(int)horasRestantes} hora(s)",
                        NotificationType.Warning
                    );
                }
            }

            await Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
