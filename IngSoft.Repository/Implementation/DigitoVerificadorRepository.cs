using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.DBConnection.Models;
using IngSoft.Domain;
using IngSoft.Repository.Dto;
using IngSoft.Repository.Factory;
using IngSoft.Services.Encriptadores;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IngSoft.Repository.Implementation
{
    /// <summary>
    /// Centraliza la configuración de una tabla para DVH/DVV.
    /// Para agregar una tabla nueva: agregar una entrada en la lista
    /// configs del constructor, sin tocar ningún otro método.
    /// </summary>
    public class TableDvConfig
    {
        public string NombreTabla { get; set; }

        // Nombre de la propiedad en la clase de dominio que contiene
        // el identificador entero (PK real en BD), usado para el SP ActualizarDVH.
        public string NombrePropiedadId { get; set; }

        // Tipos de dominio cuyos objetos corresponden a esta tabla.
        // Usado para resolver PropiedadesExcluidas por tipo en runtime.
        public HashSet<Type> TiposEntidad { get; set; }

        public HashSet<string> PropiedadesExcluidas { get; set; }

        // Ambos Funcs reciben _connection ya abierta y operan sobre ella
        // directamente, sin abrir una segunda conexión que pisaría _sqlConnection.
        public Func<IConnection, List<object>> ObtenerTodos { get; set; }
        public Func<IConnection, IEnumerable<string>, List<object>> ObtenerPorIds { get; set; }
    }

    public class DigitoVerificadorRepository : IDigitoVerificadorRepository
    {
        private readonly IConnection _connection;
        private readonly EncriptadorExperto _encriptador;
        private readonly string _connectionString;

        private readonly Dictionary<string, TableDvConfig> _tablasPorNombre;

        public DigitoVerificadorRepository(
            IBitacoraRepository bitacoraRepository,
            IUsuarioRepository usuarioRepository,
            IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
            _encriptador = new EncriptadorExperto();
            _connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

            var configs = new List<TableDvConfig>
            {
                new TableDvConfig
                {
                    NombreTabla          = "Usuario",
                    NombrePropiedadId    = "IdUsuario",   // propiedad int en clase Usuario
                    TiposEntidad         = new HashSet<Type> { typeof(Usuario) },
                    PropiedadesExcluidas = new HashSet<string> { "Dvh", "IdBitacora", "Id" },

                    ObtenerTodos = conn =>
                        conn.EjecutarDataTable<UsuarioQuerySql>(
                                "SELECT * FROM Usuario",
                                new Dictionary<string, object>())
                            .Select(u => (object)MapUsuario(u))
                            .ToList(),

                    // Filtra por Id (int PK en BD) — los ids vienen de IdUsuario
                    // del dominio, que es el mismo int
                    ObtenerPorIds = (conn, ids) =>
                        conn.EjecutarDataTable<UsuarioQuerySql>(
                                $"SELECT * FROM Usuario WHERE Id IN ({string.Join(",", ids)})",
                                new Dictionary<string, object>())
                            .Select(u => (object)MapUsuario(u))
                            .ToList()
                },
                new TableDvConfig
                {
                    NombreTabla          = "Bitacora",
                    NombrePropiedadId    = "Id",
                    TiposEntidad         = new HashSet<Type> { typeof(Bitacora) },
                    PropiedadesExcluidas = new HashSet<string> { "Dvh", "Usuario" },

                    ObtenerTodos = conn =>
                        conn.EjecutarDataTable<Bitacora>(
                                "SELECT * FROM Bitacora",
                                new Dictionary<string, object>())
                            .Cast<object>()
                            .ToList(),

                    ObtenerPorIds = (conn, ids) =>
                        conn.EjecutarDataTable<Bitacora>(
                                $"SELECT * FROM Bitacora WHERE Id IN ({string.Join(",", ids)})",
                                new Dictionary<string, object>())
                            .Cast<object>()
                            .ToList()
                }
            };

            _tablasPorNombre = configs.ToDictionary(c => c.NombreTabla, StringComparer.OrdinalIgnoreCase);
        }

        // ── Interfaz pública ────────────────────────────────────────────────────

        public List<DigitoVerificador> ObtenerDigitosVerificadores()
        {
            return EjecutarConConexion(() => ObtenerDigitosVerificadoresInterno());
        }

        public string CrearDVH(object entity)
        {
            var excluidas = ObtenerPropiedadesExcluidasPorTipo(entity.GetType());
            return CalcularDVH(entity, excluidas);
        }

        public string CrearDVV(string tabla)
        {
            return EjecutarConConexion(() =>
            {
                var registros = ObtenerConfigPorNombre(tabla).ObtenerTodos(_connection);
                return CalcularDVVDesdeRegistros(registros);
            });
        }

        public void ActualizarDVV(string tabla, string nuevoDv)
        {
            EjecutarConConexion<object>(() =>
            {
                ActualizarDVVInterno(tabla, nuevoDv);
                return null;
            });
        }

        public ResultadoIntegridad ValidarIntegridad()
        {
            try
            {
                return EjecutarConConexion(() => ValidarIntegridadInterno());
            }
            catch (Exception ex)
            {
                return new ResultadoIntegridad
                {
                    EsValida = false,
                    Errores = new List<MessageErrorIntegridad>(),
                    Mensaje = $"Error al validar integridad: {ex.Message}"
                };
            }
        }

        public void RecalcularDigitosVerificadores()
        {
            // Conexión abierta una sola vez — permanece activa durante
            // toda la operación: validación, fetches y transacción.
            _connection.NuevaConexion(_connectionString);
            try
            {
                var integridad = ValidarIntegridadInterno();
                if (integridad.EsValida) return;

                // IDs con error DVH agrupados por tabla
                var idsPorTabla = integridad.Errores
                    .Where(e => e.TipoDV == "DVH")
                    .GroupBy(e => e.Tabla)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToList());

                // Tablas con solo error DVV — necesitan todos los registros
                var tablasSoloDVV = integridad.Errores
                    .Where(e => e.TipoDV == "DVV" && !idsPorTabla.ContainsKey(e.Tabla))
                    .Select(e => e.Tabla)
                    .Distinct();

                // Una sola query por tabla, filtrada donde sea posible
                var registrosPorTabla = new Dictionary<string, List<object>>();

                foreach (var kv in idsPorTabla)
                    registrosPorTabla[kv.Key] = ObtenerConfigPorNombre(kv.Key)
                        .ObtenerPorIds(_connection, kv.Value);

                foreach (var tabla in tablasSoloDVV)
                    registrosPorTabla[tabla] = ObtenerConfigPorNombre(tabla)
                        .ObtenerTodos(_connection);

                _connection.IniciarTransaccion();
                try
                {
                    foreach (var kv in registrosPorTabla)
                    {
                        RecalcularYActualizarDVHBatch(kv.Key, kv.Value);
                        ActualizarDVVInterno(kv.Key, CalcularDVVDesdeRegistros(kv.Value));
                    }

                    _connection.AceptarTransaccion();
                }
                catch
                {
                    _connection.CancelarTransaccion();
                    throw;
                }
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        // ── Helpers privados ────────────────────────────────────────────────────

        // Mapeo idéntico al de ObtenerUsuarios() en UsuarioRepository.
        // Centralizado aquí para no duplicarlo en ObtenerTodos y ObtenerPorIds.
        private static Usuario MapUsuario(UsuarioQuerySql u)
        {
            return new Usuario
            {
                IdUsuario = u.IdUsuario,
                Id = new Guid(new EncriptadorExperto().EncriptadorOnlyHash(u.Id.ToString())),
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Contrasena = u.Contrasena,
                UserName = u.UserName,
                Bloqueado = u.Bloqueado,
                CantidadIntentos = u.CantidadIntentos,
                Dvh = u.Dvh,
                FechaEliminado = u.FechaEliminado
            };
        }

        private T EjecutarConConexion<T>(Func<T> accion)
        {
            _connection.NuevaConexion(_connectionString);
            try
            {
                return accion();
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        private List<DigitoVerificador> ObtenerDigitosVerificadoresInterno()
        {
            return _connection.EjecutarDataSet<DigitoVerificador>(
                "ObtenerDigitosVerificadores",
                new Dictionary<string, object>());
        }

        private ResultadoIntegridad ValidarIntegridadInterno()
        {
            var resultado = new ResultadoIntegridad
            {
                EsValida = true,
                Errores = new List<MessageErrorIntegridad>()
            };

            var digitosVerificadores = ObtenerDigitosVerificadoresInterno();

            // Una sola query por tabla — registros reutilizados para DVV y DVH
            var registrosPorTabla = digitosVerificadores
                .Where(dv => _tablasPorNombre.ContainsKey(dv.NombreTabla))
                .ToDictionary(
                    dv => dv.NombreTabla,
                    dv => ObtenerConfigPorNombre(dv.NombreTabla).ObtenerTodos(_connection));

            foreach (var dv in digitosVerificadores)
            {
                if (!registrosPorTabla.TryGetValue(dv.NombreTabla, out var registros))
                    continue;

                var dvvCalculado = CalcularDVVDesdeRegistros(registros);
                if (dvvCalculado != dv.DVV)
                {
                    resultado.EsValida = false;
                    resultado.Errores.Add(new MessageErrorIntegridad
                    {
                        Tabla = dv.NombreTabla,
                        DVEsperado = dv.DVV,
                        DVCalculado = dvvCalculado,
                        TipoDV = "DVV"
                    });
                }

                var erroresDVH = ValidarDVHDesdeRegistros(dv.NombreTabla, registros);
                if (erroresDVH.Any())
                {
                    resultado.EsValida = false;
                    resultado.Errores.AddRange(erroresDVH);
                }
            }

            resultado.Mensaje = resultado.EsValida
                ? "La integridad de la base de datos es válida."
                : $"Se encontraron {resultado.Errores.Count} error(es) de integridad.";

            return resultado;
        }

        private TableDvConfig ObtenerConfigPorNombre(string tabla)
        {
            if (_tablasPorNombre.TryGetValue(tabla, out var config))
                return config;

            throw new ArgumentException($"La tabla '{tabla}' no está registrada para DVH/DVV.");
        }

        // Replica el comportamiento de la versión estable: busca por tipo real
        // del objeto en runtime. Si no matchea, fallback a solo excluir "Dvh".
        private HashSet<string> ObtenerPropiedadesExcluidasPorTipo(Type tipo)
        {
            foreach (var config in _tablasPorNombre.Values)
            {
                if (config.TiposEntidad.Contains(tipo))
                    return config.PropiedadesExcluidas;
            }

            return new HashSet<string> { "Dvh" };
        }

        private string CalcularDVH(object entity, HashSet<string> propiedadesExcluidas)
        {
            var sb = new StringBuilder();

            foreach (var prop in entity.GetType()
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => !propiedadesExcluidas.Contains(p.Name))
                         .OrderBy(p => p.Name))
            {
                sb.Append(prop.GetValue(entity)?.ToString() ?? string.Empty);
            }

            return _encriptador.EncriptadorSecuencial(sb.ToString());
        }

        private string CalcularDVVDesdeRegistros(List<object> registros)
        {
            var sb = new StringBuilder();

            foreach (var registro in registros)
            {
                var propiedad = registro.GetType()
                                        .GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);
                if (propiedad != null)
                    sb.Append(propiedad.GetValue(registro)?.ToString() ?? string.Empty);
            }

            return _encriptador.EncriptadorSecuencial(sb.ToString());
        }

        private List<MessageErrorIntegridad> ValidarDVHDesdeRegistros(string tabla, List<object> registros)
        {
            var errores = new List<MessageErrorIntegridad>();
            var config = ObtenerConfigPorNombre(tabla);

            foreach (var registro in registros)
            {
                var tipo = registro.GetType();
                var dvhPi = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);
                var almacenado = dvhPi?.GetValue(registro)?.ToString();
                var excluidas = ObtenerPropiedadesExcluidasPorTipo(tipo);
                var calculado = CalcularDVH(registro, excluidas);

                if (almacenado == calculado) continue;

                errores.Add(new MessageErrorIntegridad
                {
                    Id = GetId(registro, config.NombrePropiedadId),
                    Tabla = tabla,
                    DVEsperado = almacenado,
                    DVCalculado = calculado,
                    TipoDV = "DVH"
                });
            }

            return errores;
        }

        private void RecalcularYActualizarDVHBatch(string tabla, List<object> registros)
        {
            var config = ObtenerConfigPorNombre(tabla);

            foreach (var registro in registros)
            {
                var tipo = registro.GetType();
                var excluidas = ObtenerPropiedadesExcluidasPorTipo(tipo);
                var dvhNuevo = CalcularDVH(registro, excluidas);

                _connection.EjecutarSinResultado("ActualizarDVH", new Dictionary<string, object>
                {
                    { "@tabla",   tabla },
                    { "@valorId", GetId(registro, config.NombrePropiedadId) },
                    { "@dvh",     dvhNuevo }
                });

                // Actualizar en memoria para que el DVV posterior sea correcto
                var dvhPi = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);
                if (dvhPi != null && dvhPi.CanWrite)
                    dvhPi.SetValue(registro, dvhNuevo);
            }
        }

        private void ActualizarDVVInterno(string tabla, string nuevoDv)
        {
            _connection.EjecutarSinResultado("ActualizarDVV", new Dictionary<string, object>
            {
                { "@NombreTabla", tabla   },
                { "@Dvv",         nuevoDv }
            });
        }

        private static string GetId(object registro, string nombrePropiedadId)
        {
            var pi = registro.GetType()
                             .GetProperty(nombrePropiedadId, BindingFlags.Public | BindingFlags.Instance);
            return pi?.GetValue(registro)?.ToString() ?? "Desconocido";
        }
    }
}