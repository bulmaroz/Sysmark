using System;
using System.Configuration;
using System.IO;
using Microsoft.Data.SqlClient;

namespace SysMarkModerno.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Verifica si la conexión a la base de datos es válida
        /// </summary>
        public static bool VerificarConexion(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection.State == System.Data.ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión con fallback automático:
        /// intenta primero 192.168.1.15 (red local) y si falla usa localhost.
        /// </summary>
        public static string ObtenerCadenaConexion()
        {
            // Lista de candidatos para intentar la conexión (en orden de prioridad)
            string[] candidatos = new[]
            {
                @"DESKTOP-RTH3RDL\EPICORSI", // Nombre de instancia
                @"192.168.1.13",           // IP Actual
                @"192.168.1.16"            // IP Anterior (por si acaso)
            };

            foreach (var server in candidatos)
            {
                string cs = $@"Server={server};Database=SysMark;User Id=sa1;Password=epicor;TrustServerCertificate=True;Connect Timeout=3;";
                
                if (VerificarConexion(cs))
                {
                    Console.WriteLine($@"✅ Conectado exitosamente a: {server}");
                    return cs;
                }
                
                Console.WriteLine($@"❌ Falló intento con: {server}");
            }

            // Si nada funcionó, devolvemos el principal por nombre para que el error en App.xaml.cs sea descriptivo
            return $@"Server=DESKTOP-RTH3RDL\EPICORSI;Database=SysMark;User Id=sa1;Password=epicor;TrustServerCertificate=True;";
        }

        /// <summary>
        /// Guarda la cadena de conexión en un archivo
        /// </summary>
        public static void GuardarCadenaConexion(string connectionString)
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            File.WriteAllText(configPath, connectionString);
        }

        /// <summary>
        /// Obtiene información del servidor SQL
        /// </summary>
        public static string ObtenerVersionSQLServer(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                using var command = new SqlCommand("SELECT @@VERSION", connection);
                return command.ExecuteScalar()?.ToString() ?? "Desconocida";
            }
            catch
            {
                return "No disponible";
            }
        }

        /// <summary>
        /// Constructor de cadena de conexión
        /// </summary>
        public static string ConstruirCadenaConexion(
            string servidor,
            string baseDatos,
            bool usarWindowsAuth,
            string usuario = null,
            string password = null)
        {
            if (usarWindowsAuth)
            {
                return $"Server={servidor};Database={baseDatos};Integrated Security=True;TrustServerCertificate=True;";
            }
            else
            {
                return $"Server={servidor};Database={baseDatos};User Id={usuario};Password={password};TrustServerCertificate=True;";
            }
        }
        /// <summary>
        /// Obtiene la configuración de la aplicación (Tema, Colores)
        /// </summary>
        public static (string Theme, string Primary, string Secondary) ObtenerConfiguracionAplicacion()
        {
            try
            {
                var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    string json = File.ReadAllText(appSettingsPath);
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("Application", out var appSettings))
                    {
                        string theme = appSettings.TryGetProperty("Theme", out var t) ? t.GetString() : "Light";
                        string primary = appSettings.TryGetProperty("PrimaryColor", out var p) ? p.GetString() : "Blue";
                        string secondary = appSettings.TryGetProperty("SecondaryColor", out var s) ? s.GetString() : "Cyan";
                        return (theme, primary, secondary);
                    }
                }
            }
            catch { }
            return ("Light", "Blue", "Cyan");
        }

        public class EmailConfig
        {
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
            public bool UseSsl { get; set; }
            public string FromEmail { get; set; }
            public string FromName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string[] ToEmails { get; set; }
        }

        public static EmailConfig ObtenerConfiguracionEmail()
        {
            try
            {
                var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    string json = File.ReadAllText(appSettingsPath);
                    // Usamos un deserializador flexible
                    var options = new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                    };
                    
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("Email", out var emailElement))
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<EmailConfig>(emailElement.GetRawText(), options);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo config email: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Guarda la configuración de correo en appsettings.json
        /// </summary>
        public static void GuardarConfiguracionEmail(EmailConfig config)
        {
            try
            {
                var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                string json = "{}";

                if (File.Exists(appSettingsPath))
                {
                    json = File.ReadAllText(appSettingsPath);
                }

                var options = new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                };

                var dict = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json, options);

                dict["Email"] = config;

                string updatedJson = System.Text.Json.JsonSerializer.Serialize(dict, options);
                File.WriteAllText(appSettingsPath, updatedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar configuración de email: {ex.Message}");
            }
        }

        /// <summary>
        /// Guarda la configuración de la aplicación (Tema, Colores)
        /// </summary>
        public static void GuardarConfiguracionAplicacion(string theme, string primary, string secondary)
        {
            try
            {
                var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                string json = "{}";
                
                if (File.Exists(appSettingsPath))
                {
                    json = File.ReadAllText(appSettingsPath);
                }

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement.Clone();
                
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var dict = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json);

                if (!dict.ContainsKey("Application"))
                {
                    dict["Application"] = new System.Collections.Generic.Dictionary<string, string>();
                }

                var appSettings = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(dict["Application"].ToString());
                appSettings["Theme"] = theme;
                appSettings["PrimaryColor"] = primary;
                appSettings["SecondaryColor"] = secondary;

                dict["Application"] = appSettings;

                string updatedJson = System.Text.Json.JsonSerializer.Serialize(dict, options);
                File.WriteAllText(appSettingsPath, updatedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar configuración: {ex.Message}");
            }
        }
    }
}
