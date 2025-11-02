using System;
using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class UsuarioServices: IUsuarioServices
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private Action<Usuario, string, string, TipoEvento> _registrarEnBitacora;

        public UsuarioServices(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository ?? FactoryRepository.CreateUsuarioRepository();
        }

        public void SetRegistradoBitacora(Action<Usuario, string, string, TipoEvento> registrarEnBitacora)
        {
            _registrarEnBitacora = registrarEnBitacora;
        }

        public void GuardarUsuario(Usuario usuario)
        {
            try
            {
               _usuarioRepository.GuardarUsuario(usuario);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado/modificado exitosamente", "GuardarUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear/modificar usuario", "GuardarUsuario", TipoEvento.Error);
                throw;
            }
        }
        public SessionManager LoginUser(Usuario usuario)
        {
            SessionManager session = SessionManager.GetInstance();
            Usuario usuarioStored = ObtenerUsuario(usuario.UserName);
            if(usuarioStored!= null && usuarioStored.Bloqueado)
            {
                _registrarEnBitacora(usuarioStored, "Intento de acceso con usuario bloqueado", "Login", TipoEvento.Error);
                throw new UnauthorizedAccessException("El usuario se encuentra bloqueado.");
            }
            try
            {
                session.LogIn(usuario, usuarioStored);
                _registrarEnBitacora(usuarioStored, "Acceso exitoso", "Login", TipoEvento.Message);
                usuario.CantidadIntentos = 0;
                _usuarioRepository.ResetearIntentosFallidos(usuarioStored);
            }
            catch(UnauthorizedAccessException)
            {
                _registrarEnBitacora(usuarioStored, "Intento de acceso fallido", "Login", TipoEvento.Error);
                _usuarioRepository.AumentarIntentosFallidos(usuarioStored);
                throw;
            }
            catch(Exception)
            {
                throw;
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
            return _usuarioRepository.ObtenerUsuario(username);
        }

        public List<Usuario> ObtenerUsuarioFiltrados(string filtro)
        {
            return _usuarioRepository.ObtenerUsuariosFiltrados(filtro);
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return _usuarioRepository.ObtenerUsuarios();
        }
    }
}

