
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class UsuarioServices : GuardableEnBitacora, IUsuarioServices
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioHistoricoServices _usuarioHistoricoServices;

        public UsuarioServices(IUsuarioRepository usuarioRepository, IUsuarioHistoricoServices usuarioHistoricoServices)
        {
            _usuarioRepository = usuarioRepository ?? FactoryRepository.CreateUsuarioRepository();
            _usuarioHistoricoServices = usuarioHistoricoServices ?? ServicesFactory.CreateUsuarioHistoricoServices();
        }

        public void CrearUsuario(Usuario usuario)
        {
            try
            {
                var usuarioStored = _usuarioRepository.CrearUsuario(usuario);
                GuardarUsuarioHistorico(usuarioStored, TipoOperacion.CREATE, SessionManager.GetUsuario().UserName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado exitosamente", "CrearUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear usuario", "CrearUsuario", TipoEvento.Error);
                throw;
            }
        }
        public void EliminarUsuario(Usuario usuario)
        {
            try
            {
                var usuarioStored = _usuarioRepository.EliminarUsuario(usuario);
                GuardarUsuarioHistorico(usuarioStored, TipoOperacion.DELETE, SessionManager.GetUsuario().UserName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario eliminado exitosamente", "EliminarUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al eliminar usuario", "EliminarUsuario", TipoEvento.Error);
                throw;
            }
        }

        public void ModificarUsuario(Usuario usuario)
        {
            try
            {
                var usuarioStored = _usuarioRepository.ModificarUsuario(usuario);
                GuardarUsuarioHistorico(usuarioStored, TipoOperacion.UPDATE, SessionManager.GetUsuario().UserName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario modificado exitosamente", "ModificarUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al modificar usuario", "ModificarUsuario", TipoEvento.Error);
                throw;
            }
        }

        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
                var usuarioStored = ObtenerUsuario(usuario.UserName);
                if (usuarioStored == null)
                {
                    CrearUsuario(usuario);
                }
                else
                {
                    ModificarUsuario(usuario);
                    GuardarUsuarioHistorico(usuario, TipoOperacion.UPDATE, SessionManager.GetUsuario().UserName);
                }
                //_usuarioRepository.GuardarUsuario(usuario);
                //_registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado/modificado exitosamente", "GuardarUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                //_registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear/modificar usuario", "GuardarUsuario", TipoEvento.Error);
                throw;
            }
        }
        public SessionManager LoginUser(Usuario usuario)
        {
            SessionManager session = SessionManager.GetInstance();
            Usuario usuarioStored = ObtenerUsuario(usuario.UserName);
            if(usuarioStored != null)    
            { 
                if (usuarioStored.Bloqueado)
                {
                    _registrarEnBitacora(usuarioStored, "Intento de acceso con usuario bloqueado", "Login", TipoEvento.Warning);
                    throw new UnauthorizedAccessException("El usuario se encuentra bloqueado.");
                }
                try
                {
                    IPermisoServices _permisoServices = ServicesFactory.CreatePermisoServices();
                    PermisoComponent permisoRoot = _permisoServices.ObtenerPermisosPorUsuario(usuarioStored.UserName);
                    session.LogIn(usuario, usuarioStored, permisoRoot);
                    _registrarEnBitacora(usuarioStored, "Acceso exitoso", "Login", TipoEvento.Message);
                    if(usuarioStored.CantidadIntentos>0)
                    {
                        usuarioStored.CantidadIntentos = 0;
                        _usuarioRepository.ResetearIntentosFallidos(usuarioStored);
                        GuardarUsuarioHistorico(usuarioStored, TipoOperacion.UPDATE, SessionManager.GetUsuario().UserName);
                    }
                }

                catch (InvalidCredentialException)
                {
                    _registrarEnBitacora(usuarioStored, "Intento de acceso fallido", "Login", TipoEvento.Error);
                    var usuarioConIntentos = _usuarioRepository.AumentarIntentosFallidos(usuarioStored);
                    GuardarUsuarioHistorico(usuarioConIntentos, TipoOperacion.UPDATE, usuarioConIntentos.UserName);
                    throw;
                }
                catch (Exception e)
                {
                    _registrarEnBitacora(usuarioStored, "Error inesperado: " + e.Message, "Login", TipoEvento.Error);
                    throw;
                }
            }
            else
            {
                throw new InvalidCredentialException("Credenciales incorrectas");
            }

                return session;
        }
        public void LogOutUser()
        {
            _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Cierre de sesión exitoso", "LogOut", TipoEvento.Message);
            SessionManager.GetInstance().LogOut();
        }
        public Usuario ObtenerUsuario(string username)
        {
            try
            {
                Usuario mUsuario = _usuarioRepository.ObtenerUsuario(username);
                if (!(SessionManager.GetUsuario() is null))
                {
                    _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Buscado Usuario " + username, "ObtenerUsuario", TipoEvento.Message);
                }
                return mUsuario;
            }
            /*catch(ArgumentException)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario ?? usuario, "Usuario no Encontrado", "Login", TipoEvento.Warning);
                throw new InvalidCredentialException();
            }
            catch (InvalidCredentialException e)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario ?? usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Warning);
                throw;
            }*/
            catch (Exception e)
            {
                //_registrarEnBitacora(SessionManager.GetUsuario() as Usuario ?? usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Error);
                throw;
            }
        }

        public List<Usuario> ObtenerUsuarioFiltrados(string filtro)
        {
            try
            {
                List<Usuario> mUsuarios = _usuarioRepository.ObtenerUsuariosFiltrados(filtro);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Buscados todos los Usuarios con filtro " + filtro, "ObtenerUsuariosFiltrados", TipoEvento.Message);
                return mUsuarios;
            }
            catch (Exception e)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Error);
                throw;
            }
        }

        public List<Usuario> ObtenerUsuarios()
        {
            try
            {
                List<Usuario> mUsuarios = _usuarioRepository.ObtenerUsuarios();
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Buscados todos los Usuarios", "ObtenerUsuariosFiltrados", TipoEvento.Message);
                return mUsuarios;
            }
            catch (Exception e)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Error);
                throw;
            }
        }

        private void GuardarUsuarioHistorico(Usuario usuario, TipoOperacion tipoOperacion, string usuarioModificador)
        {
            _usuarioHistoricoServices.GuardarUsuarioHistorico(new UsuarioHistorico
            {
                Id = Guid.NewGuid(),
                IdUsuario = usuario.IdUsuario,
                UserName = usuario.UserName,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Bloqueado = usuario.Bloqueado,
                CantidadIntentos = usuario.CantidadIntentos,
                FechaModificacion = DateTime.Now,
                TipoOperacion = tipoOperacion,
                UsuarioModificador = usuarioModificador
            });
        }
    }
}