using System;
using System.Collections.Generic;
using System.Configuration;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;

namespace IngSoft.Repository.Implementation
{
    public class UsuarioHistoricoRepository : IUsuarioHistoricoRepository
    {
        private readonly IConnection _connection;

        public UsuarioHistoricoRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public void GuardarUsuarioHistorico(UsuarioHistorico usuarioHistorico)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
            _connection.NuevaConexion(connectionString);

            try
            {
                Dictionary<string, object> parametros;
                if(usuarioHistorico.FechaEliminado == DateTime.MinValue)
                {
                    parametros = new Dictionary<string, object>
                    {
                        {"@Id", usuarioHistorico.Id },
                        {"@IdUsuario", usuarioHistorico.IdUsuario },
                        {"@Nombre", usuarioHistorico.Nombre },
                        {"@Apellido", usuarioHistorico.Apellido },
                        {"@Email", usuarioHistorico.Email },
                        {"@UserName", usuarioHistorico.UserName },
                        {"@Bloqueado", usuarioHistorico.Bloqueado },
                        {"@CantidadIntentos", usuarioHistorico.CantidadIntentos },
                        {"@FechaModificacion", usuarioHistorico.FechaModificacion },
                        {"@TipoOperacion", usuarioHistorico.TipoOperacion },
                        {"@UsuarioModificador", usuarioHistorico.UsuarioModificador }
                    };
                }
                else
                {
                    parametros = new Dictionary<string, object>
                    {
                        {"@Id", usuarioHistorico.Id },
                        {"@IdUsuario", usuarioHistorico.IdUsuario },
                        {"@Nombre", usuarioHistorico.Nombre },
                        {"@Apellido", usuarioHistorico.Apellido },
                        {"@Email", usuarioHistorico.Email },
                        {"@UserName", usuarioHistorico.UserName },
                        {"@Bloqueado", usuarioHistorico.Bloqueado },
                        {"@CantidadIntentos", usuarioHistorico.CantidadIntentos },
                        { "@FechaEliminado",usuarioHistorico.FechaEliminado },
                        {"@FechaModificacion", usuarioHistorico.FechaModificacion },
                        {"@TipoOperacion", usuarioHistorico.TipoOperacion },
                        {"@UsuarioModificador", usuarioHistorico.UsuarioModificador }
                    };
                }

                    _connection.EjecutarSinResultado("GuardarUsuarioHistorico", parametros);
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

        public List<UsuarioHistorico> ObtenerUsuarioHistorico(string username)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
                _connection.NuevaConexion(connectionString);
                
                var parametros = new Dictionary<string, object>
                {
                    {"@UserName", username }
                };
                var resultado = _connection.EjecutarDataSet<UsuarioHistorico>("ObtenerUsuarioHistorico", parametros);

                _connection.FinalizarConexion();
                return resultado;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
