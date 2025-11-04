using System;
using System.Collections.Generic;
using System.Linq;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.DBConnection.Models;
using IngSoft.Domain;
using IngSoft.Repository.Factory;
using IngSoft.Services.Encriptadores;

namespace IngSoft.Repository.Implementation
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString= System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
        private IDigitoVerificadorRepository _digitoVerificadorRepository;

        internal UsuarioRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }
        private IDigitoVerificadorRepository DigitoVerificadorRepository
        {
            get
            {
                if (_digitoVerificadorRepository == null)
                {
                    _digitoVerificadorRepository = FactoryRepository.CreateDigitoVerificadorRepository();
                }
                return _digitoVerificadorRepository;
            }
        }

        public void ModificarUsuarioPrueba(Usuario usuario)
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
                    var usuarioExistente = ObtenerUsuarioInterno(usuario.UserName);
                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Apellido = usuario.Apellido;
                    usuarioExistente.Email = usuario.Email;
                    usuarioExistente.Contrasena = usuario.Contrasena;

                    var parametros = new Dictionary<string, object>
                    {
                        {"@UserName", usuario.UserName },
                        {"@Nombre", usuarioExistente.Nombre },
                        {"@Apellido", usuarioExistente.Apellido },
                        {"@Email", usuarioExistente.Email },
                        {"@Contrasena", usuarioExistente.Contrasena },
                        {"@Dvh", DigitoVerificadorRepository.CrearDVH(usuarioExistente)}
                    };
                    _connection.EjecutarSinResultado("ModificarUsuario", parametros);
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
                        {"@UserName", usuario.UserName },
                        {"@Dvh",  DigitoVerificadorRepository.CrearDVH(usuario)}
                    };

                    _connection.EjecutarSinResultado("CrearUsuario", parametros);
                }
                _connection.AceptarTransaccion();
                ActualizarDVV();
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

        public Usuario CrearUsuario(Usuario usuario)
        {
            EncriptadorExperto mEncritpador = new EncriptadorExperto();
            usuario.Contrasena = mEncritpador.EncriptadorSecuencial(usuario.Contrasena);
            _connection.NuevaConexion(connectionString);

            try
            {
                _connection.IniciarTransaccion();

                usuario.IdUsuario = _connection.ObtenerUltimoId("Usuario", "Id") + 1;
                var parametros = new Dictionary<string, object>
                    {
                        {"@Id", usuario.IdUsuario},
                        {"@Nombre", usuario.Nombre },
                        {"@Apellido", usuario.Apellido },
                        {"@Email", usuario.Email },
                        {"@Contrasena", usuario.Contrasena },
                        {"@UserName", usuario.UserName },
                        {"@Dvh",  DigitoVerificadorRepository.CrearDVH(usuario)}
                    };

                _connection.EjecutarSinResultado("CrearUsuario", parametros);

                _connection.AceptarTransaccion();
                ActualizarDVV();

                return usuario;
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
        public Usuario ModificarUsuario(Usuario usuario)
        {
            EncriptadorExperto mEncritpador = new EncriptadorExperto();
            
            _connection.NuevaConexion(connectionString);
            
            try
            {
                _connection.IniciarTransaccion();

                var usuarioExistente = ObtenerUsuarioInterno(usuario.UserName);
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Email = usuario.Email;

                if (usuario.Contrasena != null)
                {
                    usuarioExistente.Contrasena = mEncritpador.EncriptadorSecuencial(usuario.Contrasena);
                }

                var parametros = new Dictionary<string, object>
                    {
                        {"@UserName", usuario.UserName },
                        {"@Nombre", usuarioExistente.Nombre },
                        {"@Apellido", usuarioExistente.Apellido },
                        {"@Email", usuarioExistente.Email },
                        {"@Contrasena", usuarioExistente.Contrasena },
                        {"@Dvh", DigitoVerificadorRepository.CrearDVH(usuarioExistente)}
                    };
                _connection.EjecutarSinResultado("ModificarUsuario", parametros);
                _connection.AceptarTransaccion();
                ActualizarDVV();
                return usuarioExistente;
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

        public Usuario ObtenerUsuario(string username)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                return ObtenerUsuarioInterno(username);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        private Usuario ObtenerUsuarioInterno(string username)
        {
            var parametros = new Dictionary<string, object>
            {
                {"@UserName", username}
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
                //else
                //{
                //    throw new ArgumentException("Usuario no Encontrado");
                //}
            }
            return usuario;
        }

        public List<Usuario> ObtenerUsuarios()
        {
            _connection.NuevaConexion(connectionString);
            var resultado = _connection.EjecutarDataSet<UsuarioQuerySql>("ObtenerUsuarios", new Dictionary<string, object>());
            List<Usuario> usuarios = resultado.Select(u => new Usuario
            {
                IdUsuario = u.IdUsuario,
                Id = EncriptarId(u.Id),
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Contrasena = u.Contrasena,
                UserName = u.UserName,
                Bloqueado = u.Bloqueado,
                CantidadIntentos = u.CantidadIntentos,
                Dvh = u.Dvh
            }).ToList();
            _connection.FinalizarConexion();
            return usuarios;
        }
        internal Guid EncriptarId(Guid id)
        {
            return new Guid(new EncriptadorExperto().EncriptadorOnlyHash(id.ToString()));
        }
        public Usuario AumentarIntentosFallidos(Usuario usuario)
        {
            _connection.NuevaConexion(connectionString);

            try
            {
                _connection.IniciarTransaccion();
                var usuarioStored = ObtenerUsuarioInterno(usuario.UserName);

                if (usuarioStored == null)
                {
                    throw new UnauthorizedAccessException("Usuario no encontrado.");
                }

                var parametros = new Dictionary<string, object>
                {
                    {"@UserName", usuarioStored.UserName}
                };

                if (usuarioStored.CantidadIntentos > 2)
                {
                    usuarioStored.Bloqueado = true;
                    parametros.Add("@Dvh", DigitoVerificadorRepository.CrearDVH(usuarioStored));
                    _connection.EjecutarSinResultado("BloquearUsuario", parametros);
                }
                else
                {
                    usuarioStored.CantidadIntentos = usuarioStored.CantidadIntentos + 1;
                    parametros.Add("@Dvh", DigitoVerificadorRepository.CrearDVH(usuarioStored));
                    _connection.EjecutarSinResultado("AumentarIntentosUsuario", parametros);
                }

                _connection.AceptarTransaccion();
                ActualizarDVV();

                return usuarioStored;
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

        public void ResetearIntentosFallidos(Usuario usuario)
        {
            _connection.NuevaConexion(connectionString);
            var usuarioStored = ObtenerUsuarioInterno(usuario.UserName);
            usuarioStored.CantidadIntentos = usuario.CantidadIntentos;

            var parametros = new Dictionary<string, object>
            {
                {"@UserName", usuarioStored.UserName},
                {"@Dvh", DigitoVerificadorRepository.CrearDVH(usuarioStored)}
            };
            _connection.EjecutarSinResultado("ResetearIntentosUsuario", parametros);
            ActualizarDVV();
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

        private void ActualizarDVV() 
        {
            var dvvActualizado = DigitoVerificadorRepository.CrearDVV(nameof(Usuario));
            DigitoVerificadorRepository.ActualizarDVV(nameof(Usuario), dvvActualizado);
        }
    }
}
