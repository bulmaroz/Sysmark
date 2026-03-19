using System;
using SysMarkModerno.Models;

namespace SysMarkModerno.Helpers
{
    public static class EmailTemplateHelper
    {
        private const string PrimaryColor = "#2D5CF7"; // Azul del logo RDMS
        private const string PrimaryDark = "#1A3BB0";
        private const string GrayText = "#555555";
        private const string LightGray = "#F8F9FA";

        public static string GenerarCuerpoEmailMarketing(Marketing registro, string titulo, string proyectoStr, string ultimoComentario = null)
        {
            string fechaHora = registro.ProximaLlamada?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            bool esRecordatorio = titulo.Contains("Recordatorio");

            string seccionComentario = string.IsNullOrWhiteSpace(ultimoComentario) 
                ? "" 
                : $@"<div style='margin-bottom: 25px; padding: 15px; background-color: #fff9c4; border-radius: 4px; border: 1px solid #fff176; color: #555;'>
                        <strong style='color: {PrimaryColor}; display: block; margin-bottom: 5px;'>Mensaje:</strong>
                        {ultimoComentario}
                    </div>";

            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }}
        .header {{ background-color: {PrimaryColor}; padding: 30px; text-align: center; color: white; }}
        .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; text-transform: uppercase; letter-spacing: 1px; }}
        .content {{ padding: 30px; }}
        .card {{ background-color: {LightGray}; border-left: 4px solid {PrimaryColor}; padding: 20px; margin-bottom: 20px; border-radius: 0 4px 4px 0; }}
        .info-row {{ margin-bottom: 12px; border-bottom: 1px solid #eee; padding-bottom: 8px; }}
        .info-label {{ font-weight: bold; color: {PrimaryColor}; width: 140px; display: inline-block; vertical-align: top; }}
        .info-value {{ color: #333; display: inline-block; width: 380px; }}
        .footer {{ background-color: #333; color: #999; text-align: center; padding: 20px; font-size: 12px; }}
        .status-badge {{ display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: bold; background-color: {PrimaryColor}; color: white; margin-top: 10px; }}
        .highlight {{ color: {PrimaryColor}; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div style='font-size: 14px; opacity: 0.9; margin-bottom: 10px;'>SISTEMA SYSMARK</div>
            <h1>{(esRecordatorio ? "⏰ Recordatorio de Llamada" : "📌 Nuevo Registro de Marketing")}</h1>
        </div>
        <div class='content'>
            <p>Hola,</p>
            <p>Se ha generado una notificación para el siguiente registro en el sistema:</p>
            
            {seccionComentario}

            <div class='card'>
                <div class='info-row'>
                    <div class='info-label'>Empresa:</div>
                    <div class='info-value'><strong>{registro.Empresa}</strong></div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>Contacto:</div>
                    <div class='info-value'>{registro.Contacto}</div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>Proyecto:</div>
                    <div class='info-value'>{proyectoStr}</div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>Cliente de:</div>
                    <div class='info-value'>{registro.ClienteDe}</div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>Ingresado por:</div>
                    <div class='info-value'>{registro.IngresadoPor}</div>
                </div>
                <div class='info-row' style='border-bottom: none;'>
                    <div class='info-label'>Fecha y Hora:</div>
                    <div class='info-value highlight'>{fechaHora}</div>
                </div>
            </div>

            <p style='text-align: center;'>
                <span class='status-badge'>{(esRecordatorio ? "LLAMADA PROGRAMADA" : "NUEVO PROSPECTO")}</span>
            </p>

            <p style='margin-top: 30px; font-size: 13px; color: {GrayText};'>
                Este es un mensaje automático generado por <strong>RDMS Soluciones Empresariales</strong>. Por favor, no respondas a este correo.
            </p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Now.Year} RDMS Soluciones Empresariales | Sistema de Seguimiento de Marketing
        </div>
    </div>
</body>
</html>";
        }
    }
}
