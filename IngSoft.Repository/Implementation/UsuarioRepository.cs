using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.DBConnection.Models;
using IngSoft.Domain;
using IngSoft.Services.Encriptadores;

namespace IngSoft.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString= System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        internal UsuarioRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public void ModificarUsuario(Usuario usuario)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var existeUsuario = _connection.EjecutarEscalar("SELECT COUNT(*) FROM Usuario WHERE Username = @UserName", new Dictionary<string, object>
                {
                    {"@UserName", usuario.UserName }
                });
                if (Convert.ToInt32(existeUsuario) == 0)
                {
                    throw new ArgumentException("El usuario no existe");
                }
                else
                {
                    var parametros = new Dictionary<string, object>
                    {
                        {"@UserName", usuario.UserName },
                        {"@Nombre", usuario.Nombre },
                        {"@Apellido", usuario.Apellido },
                        {"@Email", usuario.Email },
                        {"@Bloqueado", usuario.Bloqueado }
                    };
                    _connection.EjecutarSinResultado("ModificarUsuario", parametros);

                }
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
        public void GuardarUsuario(Usuario usuario)
        {
            EncriptadorExperto mEncritpador = new EncriptadorExperto();
            usuario.Contrasena = mEncritpador.EncriptadorSecuencial(usuario.Contrasena);
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();               

                var existeUsuario = _connection.EjecutarEscalar("SELECT COUNT(*) FROM Usuario WHERE Username = @UserName", new Dictionary<string, object>
                {
                    {"@UserName", usuario.UserName }
                });

                if (Convert.ToInt32(existeUsuario) > 0)
                {
                    throw new ArgumentException("El usuario ya existe");
                }
                else
                {
                    usuario.IdUsuario = _connection.ObtenerUltimoId("Usuario", "Id") + 1;
                    var parametros = new Dictionary<string, object>
                    {
                        {"@Id", usuario.IdUsuario},
                        {"@Nombre", usuario.Nombre },
                        {"@Apellido", usuario.Apellido },
                        {"@Email", usuario.Email },
                        {"@Contrasena", usuario.Contrasena },
                        {"@UserName", usuario.UserName }
                    };
                    _connection.EjecutarSinResultado("CrearUsuario", parametros);
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

        public Usuario ObtenerUsuario(Usuario pUsuario)
        {
            _connection.NuevaConexion(connectionString);

            var parametros = new Dictionary<string, object>
            {
                {"@UserName", pUsuario.UserName}
            };
            var resultado = _connection.EjecutarDataSet<UsuarioQuerySql> ("ObtenerUsuarioPorUsername", parametros);
            Usuario usuario = null;

            if (resultado != null)
            {
                if(resultado.Count > 0)
                {
                    usuario = resultado.Select(u => new Usuario
                    {
                        IdUsuario = u.IdUsuario,
                        Id = EncriptarId(u.Id),
                        Nombre = u.Nombre,
                        Apellido = u.Apellido,
                        Email = u.Email,
                        Contrasena = u.Contrasena,
                        UserName = u.UserName,
                        Bloqueado = u.Bloqueado,
                        CantidadIntentos = u.CantidadIntentos
                    }).First<Usuario>();
                }
                else
                {
                    throw new ArgumentException("Usuario no Encontrado");
                }
            }
            return usuario;
        }

        public List<Usuario> ObtenerUsuarios()
        {
            _connection.NuevaConexion(connectionString);
            var resultado = _connection.EjecutarDataSet<UsuarioQuerySql>("ObtenerUsuarios", new Dictionary<string, object>());
            List<Usuario> usuarios = resultado.Select(u => new Usuario
            {
                Id = EncriptarId(u.Id),
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Contrasena = u.Contrasena,
                UserName = u.UserName,
                Bloqueado = u.Bloqueado,
                CantidadIntentos = u.CantidadIntentos
            }).ToList();
            _connection.FinalizarConexion();
            return usuarios;
        }
        internal Guid EncriptarId(Guid id)
        {
            return new Guid(new EncriptadorExperto().EncriptadorOnlyHash(id.ToString()));
        }
        public void AumentarIntentosFallidos(Usuario usuario)
        {
            _connection.NuevaConexion(connectionString);
            string query = "SELECT CantidadIntentos FROM Usuario WHERE UserName = @UserName";
            var parametros = new Dictionary<string, object>
            {
                {"@UserName", usuario.UserName}
            };
            int cantIntentos = (int)_connection.EjecutarEscalar(query, parametros);

            if (cantIntentos >= 2)
            {
                _connection.EjecutarSinResultado("AumentarIntentosUsuario", parametros);
                _connection.EjecutarSinResultado("BloquearUsuario", parametros);
            }
            else
            {
                _connection.EjecutarSinResultado("AumentarIntentosUsuario", parametros);
            }
            _connection.FinalizarConexion();
        }

        public void ResetearIntentosFallidos(Usuario usuario)
        {
            _connection.NuevaConexion(connectionString);
            var parametros = new Dictionary<string, object>
            {
                {"@UserName", usuario.UserName}
            };
            _connection.EjecutarSinResultado("ResetearIntentosUsuario", parametros);
            _connection.FinalizarConexion();
        }

        public List<Usuario> ObtenerUsuariosFiltrados(string filtro)
        {
            _connection.NuevaConexion(connectionString);
            var parametros = new Dictionary<string, object>
            {
                {"@Filtro", $"%{filtro}%"}
            };
            var resultado = _connection.EjecutarDataSet<UsuarioQuerySql>("ObtenerUsuariosFiltrados", parametros);
             List<Usuario> usuarios = resultado.Select(u => new Usuario
            {
                Id = EncriptarId(u.Id),
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Contrasena = u.Contrasena,
                 UserName = u.UserName,
                Bloqueado = u.Bloqueado,
                CantidadIntentos = u.CantidadIntentos
            }).ToList();
            _connection.FinalizarConexion();
            return usuarios;
        }
    }
}
