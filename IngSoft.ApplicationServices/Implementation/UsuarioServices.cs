using IngSoft.ApplicationServices.Factory;
using IngSoft.ApplicationServices.Implementation;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace IngSoft.ApplicationServices
{
    public class UsuarioServices: GuardableEnBitacora, IUsuarioServices
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioServices(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? FactoryRepository.CreateUsuarioRepository();
        }



        public void ModificarUsuario(Usuario usuario)
        {
            try
            {
                _usuarioRepository.ModificarUsuario(usuario);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario modificado exitosamente", "ModificarUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al modificar usuario", "ModificarUsuario", TipoEvento.Error);
                throw;
            }
        }

        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
               _usuarioRepository.GuardarUsuario(usuario);
               _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado exitosamente", "GuardarUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear usuario", "GuardarUsuario", TipoEvento.Error);
                throw;
            }
        }
        public SessionManager LoginUser(Usuario usuario)
        {
            SessionManager session = SessionManager.GetInstance();
            Usuario usuarioStored = ObtenerUsuario(usuario);
            if(usuarioStored!= null && usuarioStored.Bloqueado)
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
                _usuarioRepository.ResetearIntentosFallidos(usuario);
            }

            catch (InvalidCredentialException)
            {
                _registrarEnBitacora(usuarioStored, "Intento de acceso fallido", "Login", TipoEvento.Warning);
                _usuarioRepository.AumentarIntentosFallidos(usuario);
                throw;
            }
            catch(Exception e)
            {
                _registrarEnBitacora(usuarioStored, "Error inesperado: "+e.Message, "Login", TipoEvento.Error);
                throw;
            }
            return session;
        }
        public void LogOutUser()
        {
            _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Cierre de sesión exitoso", "LogOut", TipoEvento.Message);
            SessionManager.GetInstance().LogOut();
        }
        public Usuario ObtenerUsuario(Usuario usuario)
        {
            try
            {
                Usuario mUsuario = _usuarioRepository.ObtenerUsuario(usuario);
                if (!(SessionManager.GetUsuario() is null))
                {
                    _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Buscado Usuario "+usuario.UserName, "ObtenerUsuario", TipoEvento.Message);
                }
                return mUsuario;
            }
            catch(ArgumentException)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario ?? usuario, "Usuario no Encontrado", "Login", TipoEvento.Warning);
                throw new InvalidCredentialException();
            }
            catch (InvalidCredentialException e)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario ?? usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Warning);
                throw;
            }
            catch(Exception e)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario??usuario, "Error inesperado: " + e.Message, "Login", TipoEvento.Error);
                throw;
            }
        }

        public List<Usuario> ObtenerUsuarioFiltrados(string filtro)
        {
            try
            {
                List<Usuario> mUsuarios = _usuarioRepository.ObtenerUsuariosFiltrados(filtro);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Buscados todos los Usuarios con filtro " +filtro , "ObtenerUsuariosFiltrados", TipoEvento.Message);
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
    }
}

