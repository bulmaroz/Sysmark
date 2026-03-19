using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using SysMarkModerno.Models;

namespace SysMarkModerno.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        private DatabaseService()
        {

            _connectionString = Helpers.ConfigurationHelper.ObtenerCadenaConexion();
        }
        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<Marketing>> ObtenerTodoMarketingAsync()
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT 
                    IDEMPRESA as IdEmpresa,
                    EMPRESA as Empresa,
                    [PAGINA WEB] as PaginaWeb,
                    [ERP QUE MANEJAN] as ErpQueManeja,
                    CONTACTO as Contacto,
                    PUESTO as Puesto,
                    TEL as Telefono,
                    EXTENCION as Extension,
                    [E-MAIL] as Email,
                    GIRO as Giro,
                    ESTADO as Estado,
                    CIUDAD as Ciudad,
                    DIRECCION as Direccion,
                    CP as CodigoPostal,
                    [CONTACTO 2] as Contacto2,
                    [PUESTO 2] as Puesto2,
                    TEL2 as Telefono2,
                    [E-MAIL2] as Email2,
                    [PROXIMA LLAMADA] as ProximaLlamada,
                    [FECHA DEL CONTACTO] as FechaContacto,
                    ESTATUS as Estatus,
                    Epicor as ProyectoEpicor,
                    Opera as ProyectoOpera,
                    COMENTARIOS as Comentarios,
                    Ingresado as IngresadoPor,
                    Clientede as ClienteDe,
                    CELULAR as Celular,
                    CELULARCONTACTO2 as CelularContacto2,
                    TieneLlamada
                FROM MARKETING 
                ORDER BY IDEMPRESA DESC";

            return await connection.QueryAsync<Marketing>(query);
        }

        public async Task<IEnumerable<Marketing>> BuscarMarketingAsync(string textoBusqueda, string estatus = null, bool? soloConLlamada = null)
        {
            using var connection = CreateConnection();
            
            var parametros = new DynamicParameters();
            var condiciones = new List<string>();

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                condiciones.Add(@"(
                    EMPRESA LIKE @Texto OR 
                    CONTACTO LIKE @Texto OR 
                    EMAIL LIKE @Texto OR 
                    GIRO LIKE @Texto OR
                    ESTADO LIKE @Texto OR
                    CIUDAD LIKE @Texto
                )");
                parametros.Add("Texto", $"%{textoBusqueda}%");
            }

            if (!string.IsNullOrWhiteSpace(estatus))
            {
                condiciones.Add("ESTATUS = @Estatus");
                parametros.Add("Estatus", estatus);
            }

            if (soloConLlamada.HasValue && soloConLlamada.Value)
            {
                condiciones.Add("TieneLlamada = 1");
            }

            var whereClause = condiciones.Any() ? "WHERE " + string.Join(" AND ", condiciones) : "";

            var query = $@"
                SELECT 
                    IDEMPRESA as IdEmpresa,
                    EMPRESA as Empresa,
                    [PAGINA WEB] as PaginaWeb,
                    [ERP QUE MANEJAN] as ErpQueManeja,
                    CONTACTO as Contacto,
                    PUESTO as Puesto,
                    TEL as Telefono,
                    EXTENCION as Extension,
                    [E-MAIL] as Email,
                    GIRO as Giro,
                    ESTADO as Estado,
                    CIUDAD as Ciudad,
                    DIRECCION as Direccion,
                    CP as CodigoPostal,
                    [CONTACTO 2] as Contacto2,
                    [PUESTO 2] as Puesto2,
                    TEL2 as Telefono2,
                    [E-MAIL2] as Email2,
                    [PROXIMA LLAMADA] as ProximaLlamada,
                    [FECHA DEL CONTACTO] as FechaContacto,
                    ESTATUS as Estatus,
                    Epicor as ProyectoEpicor,
                    Opera as ProyectoOpera,
                    COMENTARIOS as Comentarios,
                    Ingresado as IngresadoPor,
                    Clientede as ClienteDe,
                    CELULAR as Celular,
                    CELULARCONTACTO2 as CelularContacto2,
                    TieneLlamada
                FROM MARKETING 
                {whereClause}
                ORDER BY IDEMPRESA DESC";

            return await connection.QueryAsync<Marketing>(query, parametros);
        }

        public async Task<IEnumerable<Marketing>> ObtenerLlamadasProximasAsync(int diasAdelante = 7)
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT 
                    IDEMPRESA as IdEmpresa,
                    EMPRESA as Empresa,
                    CONTACTO as Contacto,
                    TEL as Telefono,
                    [PROXIMA LLAMADA] as ProximaLlamada,
                    ESTATUS as Estatus,
                    COMENTARIOS as Comentarios
                FROM MARKETING 
                WHERE TieneLlamada = 1 
                    AND [PROXIMA LLAMADA] IS NOT NULL
                    AND [PROXIMA LLAMADA] >= @FechaInicio
                    AND [PROXIMA LLAMADA] <= @FechaFin
                ORDER BY [PROXIMA LLAMADA] ASC";

            return await connection.QueryAsync<Marketing>(query, new
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(diasAdelante)
            });
        }

        public async Task<bool> GuardarMarketingAsync(Marketing marketing)
        {
            using var connection = CreateConnection();

            try
            {
                if (marketing.IdEmpresa == 0)
                {
                    // Insertar nuevo
                    const string insertQuery = @"
                        INSERT INTO MARKETING (
                            EMPRESA, [PAGINA WEB], [ERP QUE MANEJAN], CONTACTO, PUESTO, TEL, 
                            EXTENCION, [E-MAIL], GIRO, ESTADO, CIUDAD, DIRECCION, CP, 
                            [CONTACTO 2], [PUESTO 2], TEL2, [E-MAIL2], [PROXIMA LLAMADA], 
                            [FECHA DEL CONTACTO], ESTATUS, Epicor, Opera, COMENTARIOS, 
                            Ingresado, Clientede, CELULAR, CELULARCONTACTO2, TieneLlamada
                        ) VALUES (
                            @Empresa, @PaginaWeb, @ErpQueManeja, @Contacto, @Puesto, @Telefono,
                            @Extension, @Email, @Giro, @Estado, @Ciudad, @Direccion, @CodigoPostal,
                            @Contacto2, @Puesto2, @Telefono2, @Email2, @ProximaLlamada,
                            @FechaContacto, @Estatus, @ProyectoEpicor, @ProyectoOpera, @Comentarios,
                            @IngresadoPor, @ClienteDe, @Celular, @CelularContacto2, @TieneLlamada
                        )";

                    await connection.ExecuteAsync(insertQuery, marketing);
                }
                else
                {
                    // Actualizar existente
                    const string updateQuery = @"
                        UPDATE MARKETING SET
                            EMPRESA = @Empresa,
                            [PAGINA WEB] = @PaginaWeb,
                            [ERP QUE MANEJAN] = @ErpQueManeja,
                            CONTACTO = @Contacto,
                            PUESTO = @Puesto,
                            TEL = @Telefono,
                            EXTENCION = @Extension,
                            [E-MAIL] = @Email,
                            GIRO = @Giro,
                            ESTADO = @Estado,
                            CIUDAD = @Ciudad,
                            DIRECCION = @Direccion,
                            CP = @CodigoPostal,
                            [CONTACTO 2] = @Contacto2,
                            [PUESTO 2] = @Puesto2,
                            TEL2 = @Telefono2,
                            [E-MAIL2] = @Email2,
                            [PROXIMA LLAMADA] = @ProximaLlamada,
                            [FECHA DEL CONTACTO] = @FechaContacto,
                            ESTATUS = @Estatus,
                            Epicor = @ProyectoEpicor,
                            Opera = @ProyectoOpera,
                            COMENTARIOS = @Comentarios,
                            Ingresado = @IngresadoPor,
                            Clientede = @ClienteDe,
                            CELULAR = @Celular,
                            CELULARCONTACTO2 = @CelularContacto2,
                            TieneLlamada = @TieneLlamada
                        WHERE IDEMPRESA = @IdEmpresa";

                    await connection.ExecuteAsync(updateQuery, marketing);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Eliminar registro
        public async Task<bool> EliminarMarketingAsync(int idEmpresa)
        {
            using var connection = CreateConnection();

            try
            {
                const string query = "DELETE FROM MARKETING WHERE IDEMPRESA = @IdEmpresa";
                await connection.ExecuteAsync(query, new { IdEmpresa = idEmpresa });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Obtener empleados activos
        public async Task<IEnumerable<string>> ObtenerEmpleadosActivosAsync()
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT CONCAT(NombreCompleto, ' ', ApellidoPaterno) as Nombre
                FROM Empleados 
                WHERE IdEstatus = 1
                ORDER BY NombreCompleto";

            return await connection.QueryAsync<string>(query);
        }

        // Obtener lista de estatus únicos
        public async Task<IEnumerable<string>> ObtenerEstatusAsync()
        {
            using var connection = CreateConnection();
            const string query = "SELECT DISTINCT ESTATUS FROM MARKETING WHERE ESTATUS IS NOT NULL ORDER BY ESTATUS";
            return await connection.QueryAsync<string>(query);
        }

        public async Task<string> ObtenerEmailPorNombreEmpleadoAsync(string nombre)
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT CorreoElectronico 
                FROM Empleados 
                WHERE CONCAT(NombreCompleto, ' ', ApellidoPaterno) = @Nombre";
            
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { Nombre = nombre });
        }

        public async Task<IEnumerable<ContactoExtra>> ObtenerContactosExtrasAsync()
        {
            using var connection = CreateConnection();
            const string query = "SELECT ID, Nombre, Correo FROM Contactos ORDER BY Nombre";
            return await connection.QueryAsync<ContactoExtra>(query);
        }

        // ===== Employee CRUD Methods =====

        public async Task<IEnumerable<Empleado>> ObtenerTodosEmpleadosAsync()
        {
            using var connection = CreateConnection();
            const string query = @"
                SELECT e.IdEmpleado, e.NombreCompleto, e.ApellidoPaterno, e.ApellidoMaterno, 
                       e.CorreoElectronico, e.IdEstatus, 
                       ISNULL(es.Descripcion, '') as EstatusDescripcion
                FROM Empleados e
                LEFT JOIN EstatusEmpleado es ON e.IdEstatus = es.IdEstatus
                ORDER BY e.NombreCompleto";
            return await connection.QueryAsync<Empleado>(query);
        }

        public async Task<IEnumerable<EstatusEmpleado>> ObtenerEstatusEmpleadosAsync()
        {
            using var connection = CreateConnection();
            const string query = "SELECT IdEstatus, Descripcion FROM EstatusEmpleado ORDER BY IdEstatus";
            return await connection.QueryAsync<EstatusEmpleado>(query);
        }

        public async Task<bool> GuardarEmpleadoAsync(Empleado empleado)
        {
            using var connection = CreateConnection();
            try
            {
                if (empleado.IdEmpleado == 0)
                {
                    const string insertQuery = @"
                        INSERT INTO Empleados (NombreCompleto, ApellidoPaterno, ApellidoMaterno, CorreoElectronico, IdEstatus)
                        VALUES (@NombreCompleto, @ApellidoPaterno, @ApellidoMaterno, @CorreoElectronico, @IdEstatus)";
                    await connection.ExecuteAsync(insertQuery, empleado);
                }
                else
                {
                    const string updateQuery = @"
                        UPDATE Empleados SET
                            NombreCompleto = @NombreCompleto,
                            ApellidoPaterno = @ApellidoPaterno,
                            ApellidoMaterno = @ApellidoMaterno,
                            CorreoElectronico = @CorreoElectronico,
                            IdEstatus = @IdEstatus
                        WHERE IdEmpleado = @IdEmpleado";
                    await connection.ExecuteAsync(updateQuery, empleado);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarEmpleadoAsync(int idEmpleado)
        {
            using var connection = CreateConnection();
            try
            {
                const string query = "DELETE FROM Empleados WHERE IdEmpleado = @IdEmpleado";
                await connection.ExecuteAsync(query, new { IdEmpleado = idEmpleado });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
