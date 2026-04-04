using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using IngSoft.Repository.Dto;
using IngSoft.Repository.Factory;
using IngSoft.Services.Encriptadores;
using Microsoft.Win32;

namespace IngSoft.Repository.Implementation
{
    public class DigitoVerificadorRepository : IDigitoVerificadorRepository
    {
        private readonly IBitacoraRepository _bitacoraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConnection _connection;
        public DigitoVerificadorRepository(IBitacoraRepository bitacoraRepository, IUsuarioRepository usuarioRepository, IConnection connection)
        {
            _bitacoraRepository = bitacoraRepository ?? FactoryRepository.CreateBitacoraRepository();
            _usuarioRepository = usuarioRepository ?? FactoryRepository.CreateUsuarioRepository(); ;
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }
        public void ActualizarDVV(string tabla, string nuevoDv)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
            _connection.NuevaConexion(connectionString);

            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@NombreTabla", tabla},
                    {"@Dvv", nuevoDv }
                };

                _connection.EjecutarSinResultado("ActualizarDVV", parametros);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<DigitoVerificador> ObtenerDigitosVerificadores()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
            _connection.NuevaConexion(connectionString);

            try
            {
                var resultado = _connection.EjecutarDataSet<DigitoVerificador>("ObtenerDigitosVerificadores", new Dictionary<string, object>());
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public string CrearDVV(string tabla)
        {
            var encriptador = new EncriptadorExperto();
            var registros = ObtenerRegistrosDeTabla(tabla);
            var stringBuilder = new StringBuilder();

            foreach (var registro in registros)
            {
                var tipo = registro.GetType();
                var propiedad = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);

                if (propiedad != null)
                {
                    var dvhValue = propiedad.GetValue(registro);
                    stringBuilder.Append(dvhValue?.ToString() ?? string.Empty);
                }
            }

            var input = stringBuilder.ToString();
            return encriptador.EncriptadorSecuencial(input);
        }

        public string CrearDVH(object entity)
        {
            var encriptador = new EncriptadorExperto();

            var tipo = entity.GetType();

            var propiedadesExcluidas = ObtenerPropiedadesExcluidas(tipo);

            var propiedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => !propiedadesExcluidas.Contains(p.Name))
                                  .OrderBy(p => p.Name);

            var stringBuilder = new StringBuilder();

            foreach (var propiedad in propiedades)
            {
                var valor = propiedad.GetValue(entity);
                stringBuilder.Append(valor?.ToString() ?? string.Empty);
            }

            var input = stringBuilder.ToString();
            return encriptador.EncriptadorSecuencial(input);

        }
        public ResultadoIntegridad ValidarIntegridad()
        {
            var resultado = new ResultadoIntegridad
            {
                EsValida = true,
                Errores = new List<MessageErrorIntegridad>()
            };

            try
            {
                // Obtener todos los dígitos verificadores almacenados
                var digitosVerificadores = ObtenerDigitosVerificadores();

                foreach (var dv in digitosVerificadores)
                {
                    // Validar DVV (Dígito Verificador Vertical)
                    var dvvCalculado = CrearDVV(dv.NombreTabla);

                    if (dvvCalculado != dv.DVV)
                    {
                        resultado.EsValida = false;
                        resultado.Errores.Add(new MessageErrorIntegridad { Tabla = dv.NombreTabla, DVEsperado = dv.DVV, DVCalculado = dvvCalculado, TipoDV = "DVV" });
                    }

                    // Validar DVH (Dígito Verificador Horizontal) de cada registro
                    var erroresDVH = ValidarDVHPorTabla(dv.NombreTabla);

                    if (erroresDVH.Any())
                    {
                        resultado.EsValida = false;
                        resultado.Errores.AddRange(erroresDVH);
                    }
                }

                if (resultado.EsValida)
                {
                    resultado.Mensaje = "La integridad de la base de datos es válida.";
                }
                else
                {
                    resultado.Mensaje = $"Se encontraron {resultado.Errores.Count} error(es) de integridad.";
                }
            }
            catch (Exception ex)
            {
                resultado.EsValida = false;
                resultado.Mensaje = $"Error al validar integridad: {ex.Message}";
            }

            return resultado;
        }

        public void RecalcularDigitosVerificadores()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
            _connection.NuevaConexion(connectionString);
            try
            {
                var digitosVerificadores = ObtenerDigitosVerificadoresSinConexion();

                var datosTablas = new Dictionary<string, List<object>>();
                foreach (var dv in digitosVerificadores)
                {
                    datosTablas[dv.NombreTabla] = ObtenerRegistrosDeTabla(dv.NombreTabla);
                }

                //_connection.IniciarTransaccion();

                foreach (var dv in digitosVerificadores) 
                {
                    var registros = datosTablas[dv.NombreTabla];
                    RecalcularDVHPorTablaYActualizarMemoria(dv.NombreTabla, registros);

                    var nuevoDvv = CrearDVVDesdeRegistros(registros);
                    ActualizarDVVSinConexion(dv.NombreTabla, nuevoDvv);
                }

                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        private List<MessageErrorIntegridad> ValidarDVHPorTabla(string tabla)
        {
            var errores = new List<MessageErrorIntegridad>();
            var registros = ObtenerRegistrosDeTabla(tabla);

            foreach (var registro in registros)
            {
                var tipo = registro.GetType();
                var propiedadDvh = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);

                if (propiedadDvh != null)
                {
                    var dvhAlmacenado = propiedadDvh.GetValue(registro)?.ToString();
                    var dvhCalculado = CrearDVH(registro);

                    if (dvhAlmacenado != dvhCalculado)
                    {
                        string nombrePropiedadId = ObtenerNombrePropiedadId(tabla);
                        var propiedadId = tipo.GetProperty(nombrePropiedadId, BindingFlags.Public | BindingFlags.Instance);

                        var identificador = propiedadId?.GetValue(registro)?.ToString() ?? "Desconocido";

                        errores.Add(new MessageErrorIntegridad { Id = identificador, Tabla = tabla, DVEsperado = dvhAlmacenado, DVCalculado = dvhCalculado, TipoDV = "DVH" });
                    }
                }
            }

            return errores;
        }

        private List<object> ObtenerRegistrosDeTabla(string tabla)
        {
            switch (tabla)
            {
                case "Usuario":
                    return _usuarioRepository.ObtenerUsuarios().Cast<object>().ToList();
                case "Bitacora":
                    return _bitacoraRepository.ObtenerBitacoras().Cast<object>().ToList();
                default:
                    throw new ArgumentException($"La tabla '{tabla}' no es soportada para el cálculo de DVV.");
            }
        }
        private string ObtenerNombrePropiedadId(string tabla)
        {
            switch (tabla)
            {
                case "Usuario":
                    return "IdUsuario";
                case "Bitacora":
                    return "Id";
                default:
                    return "Id";
            }
        }

        private HashSet<string> ObtenerPropiedadesExcluidas(Type tipo)
        {
            var excluidas = new HashSet<string> { "Dvh" };

            if (tipo == typeof(Usuario))
            {
                excluidas.Add("IdBitacora");
                excluidas.Add("Id");
            }
            else if (tipo == typeof(Bitacora))
            {
                excluidas.Add("Usuario");
            }

            return excluidas;
        }

        private void RecalcularDVHPorTablaYActualizarMemoria(string tabla, List<object> registros)
        {
            foreach (var registro in registros)
            {
                var dvhNuevo = CrearDVH(registro);

                ActualizarDVHEnBD(tabla, registro, dvhNuevo);

                ActualizarDVHEnMemoria(registro, dvhNuevo);
            }
        }
        private void ActualizarDVHEnMemoria(object registro, string dvhNuevo)
        {
            var tipo = registro.GetType();
            var propiedadDvh = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);

            if (propiedadDvh != null && propiedadDvh.CanWrite)
            {
                propiedadDvh.SetValue(registro, dvhNuevo);
            }
        }

        private void ActualizarDVHEnBD(string tabla, object registro, string dvhNuevo)
        {
            var tipo = registro.GetType();
            string nombrePropiedadId = ObtenerNombrePropiedadId(tabla);
            var propiedadId = tipo.GetProperty(nombrePropiedadId, BindingFlags.Public | BindingFlags.Instance);
            var identificador = propiedadId?.GetValue(registro);

            if (identificador == null)
            {
                throw new InvalidOperationException($"No se pudo obtener el identificador del registro en la tabla {tabla}");
            }

            var parametros = new Dictionary<string, object>
            {
                { "@tabla", tabla },
                {"@valorId", identificador},
                {"@dvh", dvhNuevo}
            };

            var storedProcedure = $"ActualizarDVH{tabla}";
            var storedProcedure = $"ActualizarDVH";
            _connection.EjecutarSinResultado(storedProcedure, parametros);
        }
        private List<DigitoVerificador> ObtenerDigitosVerificadoresSinConexion()
        {
            try
            {
                var resultado = _connection.EjecutarDataSet<DigitoVerificador>("ObtenerDigitosVerificadores", new Dictionary<string, object>());
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ActualizarDVVSinConexion(string tabla, string nuevoDv)
        {
            try
            {
                var parametros = new Dictionary<string, object>
        {
            {"@NombreTabla", tabla},
            {"@Dvv", nuevoDv }
        };

                _connection.EjecutarSinResultado("ActualizarDVV", parametros);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string CrearDVVDesdeRegistros(List<object> registros)
        {
            var encriptador = new EncriptadorExperto();
            var stringBuilder = new StringBuilder();

            foreach (var registro in registros)
            {
                var tipo = registro.GetType();
                var propiedad = tipo.GetProperty("Dvh", BindingFlags.Public | BindingFlags.Instance);

                if (propiedad != null)
                {
                    var dvhValue = propiedad.GetValue(registro);
                    stringBuilder.Append(dvhValue?.ToString() ?? string.Empty);
                }
            }

            var input = stringBuilder.ToString();
            return encriptador.EncriptadorSecuencial(input);
        }
    }
}
