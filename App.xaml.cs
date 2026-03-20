using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SysMarkModerno.Services;
using SysMarkModerno.ViewModels;
using SysMarkModerno.Views;
using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace SysMarkModerno
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            try 
            {
                base.OnStartup(e);

                // Configurar servicios
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Aplicar Tema
                AplicarTema();

                // Mostrar ventana principal
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Fatal al Iniciar: {ex.Message}\n\n{ex.StackTrace}", "Error de Sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Obtener cadena de conexión con fallback automático
            string connectionString = Helpers.ConfigurationHelper.ObtenerCadenaConexion();

            // Mostrar mensaje indicando a qué servidor se conectó
            bool conectadoExitoso = Helpers.ConfigurationHelper.VerificarConexion(connectionString);
            
            // Extraer el nombre del servidor de la cadena para mostrarlo
            string servidorDetectado = "Desconocido";
            try {
                var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                servidorDetectado = builder.DataSource;
            } catch { }

            if (!conectadoExitoso)
            {
                MessageBox.Show(
                    $"❌ ERROR CRÍTICO DE CONEXIÓN:\n\nNo se pudo encontrar el servidor en ninguna de las rutas conocidas (DESKTOP-RTH3RDL, .13, .16).\n\nVerifica que el servidor esté encendido y el usuario 'sa1' sea válido.",
                    "SysMark — Error de Red",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            
            // Servicios
            services.AddSingleton(new DatabaseService(connectionString));
            services.AddSingleton<NotificationService>();
            services.AddSingleton<IEmailService, EmailService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<UpcomingCallsViewModel>();
            services.AddTransient<UpcomingFollowUpsViewModel>();
            services.AddTransient<EmployeeViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<UpcomingCallsWindow>();
            services.AddTransient<UpcomingFollowUpsWindow>();
        }

        private void AplicarTema()
        {
            try
            {
                var config = Helpers.ConfigurationHelper.ObtenerConfiguracionAplicacion();
                var paletteHelper = new PaletteHelper();
                Theme theme = (Theme)paletteHelper.GetTheme();

                // Base Theme (Light/Dark)
                theme.SetBaseTheme(config.Theme.Equals("Dark", StringComparison.OrdinalIgnoreCase) 
                    ? BaseTheme.Dark 
                    : BaseTheme.Light);

                // Colors using Reflection on System.Windows.Media.Colors for robustness
                if (!string.IsNullOrEmpty(config.Primary))
                {
                    var prop = typeof(Colors).GetProperty(config.Primary, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                    if (prop != null) theme.SetPrimaryColor((Color)prop.GetValue(null));
                }

                if (!string.IsNullOrEmpty(config.Secondary))
                {
                    var prop = typeof(Colors).GetProperty(config.Secondary, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                    if (prop != null) theme.SetSecondaryColor((Color)prop.GetValue(null));
                }

                paletteHelper.SetTheme(theme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aplicar tema: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try 
            {
                _serviceProvider?.Dispose();
            }
            finally 
            {
                base.OnExit(e);
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }
    }
}
