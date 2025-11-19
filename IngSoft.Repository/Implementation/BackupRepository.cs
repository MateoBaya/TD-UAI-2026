using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;

namespace IngSoft.Repository.Implementation
{
    public class BackupRepository : IBackupRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public BackupRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public string ObtenerRutaBackupPorDefecto()
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var parametros = new Dictionary<string, object>();
                var resultado = _connection.EjecutarEscalar("ObtenerRutaBackupPorDefecto", parametros);

                return resultado?.ToString() ?? @"C:\Backups";
            }
            catch (Exception)
            {
                return @"C:\Backups";
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void CrearBackup(string rutaDestino, string nombreArchivo)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                var nombreBD = builder.InitialCatalog;

                var rutaCompleta = Path.Combine(rutaDestino, nombreArchivo);

                var parametros = new Dictionary<string, object>
                {
                    {"@NombreBD", nombreBD},
                    {"@RutaCompleta", rutaCompleta}
                };

                _connection.EjecutarSinResultado("CrearBackup", parametros);
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

        public void RestaurarBackup(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"El archivo de backup no existe: {rutaArchivo}");
            }

            var builder = new SqlConnectionStringBuilder(connectionString);
            var nombreBD = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;

            _connection.NuevaConexion(masterConnectionString);

            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@NombreBD", nombreBD},
                    {"@RutaArchivo", rutaArchivo}
                };

                _connection.EjecutarSinResultado("RestaurarBackup", parametros);
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

        public List<Backups> ObtenerHistorialBackups()
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var resultado = _connection.EjecutarDataSet<Backups>("ObtenerHistorialBackups", new Dictionary<string, object>());
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
        public void RegistrarBackup(Backups backup)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@Id", backup.Id},
                    {"@NombreArchivo", backup.NombreArchivo},
                    {"@RutaCompleta", backup.RutaCompleta},
                    {"@FechaCreacion", backup.FechaCreacion},
                    {"@TamanoBytes", backup.TamanoBytes},
                    {"@UsuarioCreador", backup.UsuarioCreador}
                };

                _connection.EjecutarSinResultado("RegistrarBackup", parametros);
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
        public void ActualizarTamanoBackup(Guid id, long tamanoBytes)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var parametros = new Dictionary<string, object>
        {
            {"@Id", id},
            {"@TamanoBytes", tamanoBytes}
        };

                _connection.EjecutarSinResultado("ActualizarTamanoBackup", parametros);
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
        public void EliminarBackup(Guid id)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                var parametros = new Dictionary<string, object>
        {
            {"@Id", id}
        };

                _connection.EjecutarSinResultado("EliminarBackup", parametros);
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
    }
}
