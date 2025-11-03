using System;
using System.Collections.Generic;
using IngSoft.ApplicationServices.Factory;
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
        private readonly IUsuarioHistoricoServices _usuarioHistoricoServices;

        public UsuarioServices(IUsuarioRepository usuarioRepository, IUsuarioHistoricoServices usuarioHistoricoServices)
        {
            _usuarioRepository = usuarioRepository ?? FactoryRepository.CreateUsuarioRepository();
            _usuarioHistoricoServices = usuarioHistoricoServices ?? ServicesFactory.CreateUsuarioHistoricoServices();
        }

        public void SetRegistradoBitacora(Action<Usuario, string, string, TipoEvento> registrarEnBitacora)
        {
            _registrarEnBitacora = registrarEnBitacora;
        }

        public void CrearUsuario(Usuario usuario)
        {
            try
            {
               var usuarioStored = _usuarioRepository.CrearUsuario(usuario);
               GuardarUsuarioHistorico(usuarioStored, TipoOperacion.CREATE, SessionManager.GetUsuario().UserName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado exitosamente", "CrearUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear usuario", "CrearUsuario", TipoEvento.Error);
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
                var usuarioStored = ObtenerUsuario(usuario.UserName);
                if(usuarioStored == null)
                {
                    CrearUsuario(usuario);
                }
                else
                {
                    ModificarUsuario(usuario);
                }
                //_usuarioRepository.GuardarUsuario(usuario);
               //_registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Usuario creado/modificado exitosamente", "GuardarUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                //_registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, "Error al crear/modificar usuario", "GuardarUsuario", TipoEvento.Error);
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
                usuarioStored.CantidadIntentos = 0;
                _usuarioRepository.ResetearIntentosFallidos(usuarioStored);
                GuardarUsuarioHistorico(usuarioStored, TipoOperacion.UPDATE, SessionManager.GetUsuario().UserName);
            }
            catch(UnauthorizedAccessException)
            {
                _registrarEnBitacora(usuarioStored, "Intento de acceso fallido", "Login", TipoEvento.Error);
                var usuarioConIntentos = _usuarioRepository.AumentarIntentosFallidos(usuarioStored);
                GuardarUsuarioHistorico(usuarioConIntentos, TipoOperacion.UPDATE, usuarioConIntentos.UserName);
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

