using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SysMarkModerno.Models;
using SysMarkModerno.Services;
using SysMarkModerno.Views;
using Microsoft.Extensions.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace SysMarkModerno.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableCollection<Marketing> _registrosMarketing = new();

        [ObservableProperty]
        private ObservableCollection<Marketing> _registrosFiltrados = new();

        [ObservableProperty]
        private Marketing _registroSeleccionado;

        [ObservableProperty]
        private string _textoBusqueda = string.Empty;

        [ObservableProperty]
        private string _estatusFiltro;

        [ObservableProperty]
        private bool _soloConLlamada;

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private ObservableCollection<string> _listaEstatus = new();

        [ObservableProperty]
        private ObservableCollection<string> _listaEmpleados = new();

        [ObservableProperty]
        private int _totalRegistros;

        [ObservableProperty]
        private int _registrosConLlamada;

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private bool _isEmployeeDrawerOpen;

        [ObservableProperty]
        private EmployeeViewModel _employeeViewModel;

        [ObservableProperty]
        private ObservableCollection<ContactoExtra> _listaContactosExtras = new();

        [ObservableProperty]
        private Helpers.ConfigurationHelper.EmailConfig _emailConfig;

        [ObservableProperty]
        private bool _isSettingsDrawerOpen;

        public MainViewModel(DatabaseService databaseService, NotificationService notificationService, IEmailService emailService, IServiceProvider serviceProvider)
        {
            _databaseService = databaseService;
            _notificationService = notificationService;
            _emailService = emailService;
            _serviceProvider = serviceProvider;

            RegistroSeleccionado = new Marketing
            {
                FechaContacto = DateTime.Now
            };

            // Inicializar estado del tema
            var config = Helpers.ConfigurationHelper.ObtenerConfiguracionAplicacion();
            IsDarkMode = config.Theme.Equals("Dark", StringComparison.OrdinalIgnoreCase);

            // Inicializar EmployeeViewModel
            EmployeeViewModel = serviceProvider.GetRequiredService<EmployeeViewModel>();

            _ = InicializarAsync();
            
            // Cargar configuración de email
            EmailConfig = Helpers.ConfigurationHelper.ObtenerConfiguracionEmail() ?? new Helpers.ConfigurationHelper.EmailConfig();
        }

        [RelayCommand]
        private void ToggleSettingsDrawer()
        {
            IsSettingsDrawerOpen = !IsSettingsDrawerOpen;
        }

        [RelayCommand]
        private void GuardarConfiguracionEmail()
        {
            try
            {
                Helpers.ConfigurationHelper.GuardarConfiguracionEmail(EmailConfig);
                _notificationService.MostrarNotificacion(
                    "Configuración Guardada",
                    "La configuración de correo se ha actualizado correctamente.",
                    Notifications.Wpf.NotificationType.Success
                );
                IsSettingsDrawerOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la configuración: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ToggleEmployeeDrawer()
        {
            IsEmployeeDrawerOpen = !IsEmployeeDrawerOpen;
        }

        private async Task InicializarAsync()
        {
            await CargarDatosAsync();
            await CargarCatalogosAsync();
            _notificationService.IniciarMonitoreo();
        }

        [RelayCommand]
        private async Task CargarDatosAsync()
        {
            EstaCargando = true;

            try
            {
                var registros = await _databaseService.ObtenerTodoMarketingAsync();
                RegistrosMarketing = new ObservableCollection<Marketing>(registros);
                AplicarFiltros();
                ActualizarEstadisticas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EstaCargando = false;
            }
        }

        private async Task CargarCatalogosAsync()
        {
            try
            {
                var estatus = await _databaseService.ObtenerEstatusAsync();
                ListaEstatus = new ObservableCollection<string>(estatus);

                var empleados = await _databaseService.ObtenerEmpleadosActivosAsync();
                ListaEmpleados = new ObservableCollection<string>(empleados);

                var contactosExtras = await _databaseService.ObtenerContactosExtrasAsync();
                ListaContactosExtras = new ObservableCollection<ContactoExtra>(contactosExtras);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar catálogos: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task BuscarAsync()
        {
            EstaCargando = true;

            try
            {
                var resultados = await _databaseService.BuscarMarketingAsync(
                    TextoBusqueda,
                    EstatusFiltro,
                    SoloConLlamada
                );

                RegistrosFiltrados = new ObservableCollection<Marketing>(resultados);
                ActualizarEstadisticas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la búsqueda: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EstaCargando = false;
            }
        }

        [RelayCommand]
        private void AplicarFiltros()
        {
            var query = RegistrosMarketing.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                var busqueda = TextoBusqueda.ToLower();
                query = query.Where(r =>
                    r.Empresa?.ToLower().Contains(busqueda) == true ||
                    r.Contacto?.ToLower().Contains(busqueda) == true ||
                    r.Email?.ToLower().Contains(busqueda) == true ||
                    r.Giro?.ToLower().Contains(busqueda) == true ||
                    r.Estado?.ToLower().Contains(busqueda) == true ||
                    r.Ciudad?.ToLower().Contains(busqueda) == true
                );
            }

            if (!string.IsNullOrWhiteSpace(EstatusFiltro))
            {
                query = query.Where(r => r.Estatus == EstatusFiltro);
            }

            if (SoloConLlamada)
            {
                query = query.Where(r => r.TieneLlamada);
            }

            RegistrosFiltrados = new ObservableCollection<Marketing>(query);
            ActualizarEstadisticas();
        }

        [RelayCommand]
        private void LimpiarFiltros()
        {
            TextoBusqueda = string.Empty;
            EstatusFiltro = null;
            SoloConLlamada = false;
            RegistrosFiltrados = new ObservableCollection<Marketing>(RegistrosMarketing);
            ActualizarEstadisticas();
        }

        [RelayCommand]
        private void NuevoRegistro()
        {
            RegistroSeleccionado = new Marketing
            {
                FechaContacto = DateTime.Now,
                Estatus = "Nuevo"
            };
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(RegistroSeleccionado?.Empresa))
            {
                MessageBox.Show("El nombre de la empresa es requerido", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            EstaCargando = true;
            try
            {
                var exito = await _databaseService.GuardarMarketingAsync(RegistroSeleccionado);
                if (exito)
                {
                    string mensajeExito = $"Registro de {RegistroSeleccionado.Empresa} guardado correctamente";
                    bool correoEnviado = false;
                    string errorCorreo = string.Empty;
                    var proyectosList = new System.Collections.Generic.List<string>();
                    if (RegistroSeleccionado.ProyectoEpicor) proyectosList.Add("Epicor");
                    if (RegistroSeleccionado.ProyectoOpera) proyectosList.Add("Opera");
                    string proyectoStr = proyectosList.Count > 0 ? string.Join(" / ", proyectosList) : "Ninguno";
                    var additionalEmails = new System.Collections.Generic.List<string>();
                    if (!string.IsNullOrWhiteSpace(RegistroSeleccionado.Email)) additionalEmails.Add(RegistroSeleccionado.Email);
                    if (!string.IsNullOrWhiteSpace(RegistroSeleccionado.Email2)) additionalEmails.Add(RegistroSeleccionado.Email2);
                    if (!string.IsNullOrWhiteSpace(RegistroSeleccionado.IngresadoPor))
                    {
                        var emailIngresadoPor = await _databaseService.ObtenerEmailPorNombreEmpleadoAsync(RegistroSeleccionado.IngresadoPor);
                        if (!string.IsNullOrWhiteSpace(emailIngresadoPor))
                        {
                            additionalEmails.Add(emailIngresadoPor);
                        }
                    }
                    if (ListaContactosExtras != null)
                    {
                        foreach (var contacto in ListaContactosExtras.Where(c => c.IsSelected && !string.IsNullOrWhiteSpace(c.Correo)))
                        {
                            additionalEmails.Add(contacto.Correo);
                        }
                    }
                    if (RegistroSeleccionado.TieneLlamada && RegistroSeleccionado.ProximaLlamada.HasValue)
                    {
                        string emailSubject = $"Recordatorio Llamada: {RegistroSeleccionado.Empresa} - {proyectoStr}";
                        
                        // Extraer el último comentario para incluirlo como mensaje en el correo
                        string ultimoComentario = string.Empty;
                        if (!string.IsNullOrWhiteSpace(RegistroSeleccionado.Comentarios))
                        {
                            var lineas = RegistroSeleccionado.Comentarios.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lineas.Length > 0) ultimoComentario = lineas.Last();
                        }

                        string emailBody = Helpers.EmailTemplateHelper.GenerarCuerpoEmailMarketing(RegistroSeleccionado, emailSubject, proyectoStr, ultimoComentario);

                        (correoEnviado, errorCorreo) = await _emailService.SendCalendarInviteAsync(
                            emailSubject,
                            emailBody,
                            RegistroSeleccionado.ProximaLlamada.Value,
                            RegistroSeleccionado.ProximaLlamada.Value.AddMinutes(30),
                            "Llamada Telefónica",
                            additionalEmails 
                        );
                    }
                    else
                    {
                        string emailSubject = $"Nuevo Registro Marketing: {RegistroSeleccionado.Empresa} - {proyectoStr}";
                        
                        // Extraer el último comentario para incluirlo como mensaje en el correo
                        string ultimoComentario = string.Empty;
                        if (!string.IsNullOrWhiteSpace(RegistroSeleccionado.Comentarios))
                        {
                            var lineas = RegistroSeleccionado.Comentarios.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lineas.Length > 0) ultimoComentario = lineas.Last();
                        }

                        string emailBody = Helpers.EmailTemplateHelper.GenerarCuerpoEmailMarketing(RegistroSeleccionado, emailSubject, proyectoStr, ultimoComentario);

                        (correoEnviado, errorCorreo) = await _emailService.SendEmailAsync(
                            emailSubject,
                            emailBody,
                            additionalEmails 
                        );
                    }

                    if (correoEnviado)
                    {
                        mensajeExito += "\n📧 Correo de notificación enviado.";
                        _notificationService.MostrarNotificacion(
                            "Guardado y Notificado",
                            mensajeExito,
                            Notifications.Wpf.NotificationType.Success
                        );
                    }
                    else
                    {
                        mensajeExito += $"\n⚠️ Error al enviar correo: {errorCorreo}";
                        _notificationService.MostrarNotificacion(
                            "Guardado (Sin Correo)",
                            mensajeExito,
                            Notifications.Wpf.NotificationType.Warning
                        );
                    }
                    await CargarDatosAsync();
                }
                else
                {
                    MessageBox.Show("Error al guardar el registro", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                EstaCargando = false;
            }
        }

        [RelayCommand]
        private async Task EliminarAsync()
        {
            if (RegistroSeleccionado?.IdEmpresa == 0)
            {
                MessageBox.Show("Selecciona un registro para eliminar", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Estás seguro de eliminar el registro de {RegistroSeleccionado.Empresa}?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado == MessageBoxResult.Yes)
            {
                EstaCargando = true;

                try
                {
                    var exito = await _databaseService.EliminarMarketingAsync(RegistroSeleccionado.IdEmpresa);

                    if (exito)
                    {
                        _notificationService.MostrarNotificacion(
                            "🗑️ Eliminado",
                            $"Registro eliminado correctamente",
                            Notifications.Wpf.NotificationType.Information
                        );

                        await CargarDatosAsync();
                        NuevoRegistro();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    EstaCargando = false;
                }
            }
        }

        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltros();
        }

        partial void OnEstatusFiltroChanged(string value)
        {
            AplicarFiltros();
        }

        partial void OnSoloConLlamadaChanged(bool value)
        {
            AplicarFiltros();
        }

        private void ActualizarEstadisticas()
        {
            TotalRegistros = RegistrosFiltrados.Count;
            RegistrosConLlamada = RegistrosFiltrados.Count(r => r.TieneLlamada);
        }

        [RelayCommand]
        private void VerLlamadasProximas()
        {
            var window = _serviceProvider.GetRequiredService<UpcomingCallsWindow>();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(IsDarkMode ? BaseTheme.Dark : BaseTheme.Light);
            paletteHelper.SetTheme(theme);

            // Guardar en config
            var config = Helpers.ConfigurationHelper.ObtenerConfiguracionAplicacion();
            Helpers.ConfigurationHelper.GuardarConfiguracionAplicacion(
                IsDarkMode ? "Dark" : "Light", 
                config.Primary, 
                config.Secondary
            );
        }
    }
}
