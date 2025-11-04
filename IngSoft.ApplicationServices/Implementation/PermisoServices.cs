using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.ApplicationServices.Implementation
{
    public class PermisoServices : GuardableEnBitacora, IPermisoServices
    {
        private readonly IPermisoRepository _permisoRepository;

        public PermisoServices(IPermisoRepository permisoRepository)
        {
            _permisoRepository = permisoRepository ?? FactoryRepository.CreatePermisoRepository();
        }

        public void GuardarPermiso(PermisoComponent permiso)
        {
            try
            {
                _permisoRepository.GuardarPermiso(permiso);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permiso '{permiso.Nombre}' creado exitosamente", "GuardarPermiso", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al crear permiso '{permiso.Nombre}'", "GuardarPermiso", TipoEvento.Error);
                throw;
            }
        }

        public void AsignarPermisoEnUsuario(PermisoComponent permiso, string userName)
        {
            try
            {
                _permisoRepository.AsignarPermisoEnUsuario(permiso, userName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permiso '{permiso.Nombre}' asignado al usuario '{userName}' exitosamente", "GuardarPermisoEnUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al asignar permiso '{permiso.Nombre}' al usuario '{userName}'", "GuardarPermisoEnUsuario", TipoEvento.Error);
                throw;
            }
        }
        public void EliminarPermisoDeUsuario(PermisoComponent permiso, string userName)
        {
            try
            {
                _permisoRepository.EliminarPermisosDeUsuario(permiso,userName);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, $"Permiso {permiso.Nombre} eliminado del usuario {userName} exitosamente", "EliminarPermisoDeUsuario", TipoEvento.Message);
            }
            catch(Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, $"Error al eliminar permiso {permiso.Nombre} del usuario {userName}", "EliminarPermisoDeUsuario", TipoEvento.Error);
                throw;
            }
        }

        public void ModificarPermiso(string permisoNombre, string permisoNombreNuevo, string permisoPadre=null)
        {
            try
            {
                _permisoRepository.ModificarPermiso(permisoNombre,permisoNombreNuevo,permisoPadre);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permiso '{permisoNombre}' modificado exitosamente", "ModificarPermiso", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al modificar permiso '{permisoNombre}'", "ModificarPermiso", TipoEvento.Error);
                throw;
            }
        }

        public void EliminarPermiso(string permisoNombre)
        {
            try
            {
                _permisoRepository.EliminarPermiso(permisoNombre);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permiso '{permisoNombre}' eliminado exitosamente", "EliminarPermiso", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al eliminar permiso '{permisoNombre}'", "EliminarPermiso", TipoEvento.Error);
                throw;
            }
        }
        public PermisoComponent ObtenerPermisosPorUsuario(string userName)
        {
            try
            {
                if(SessionManager.GetInstance().IsLoggedIn())
                {
                    _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permisos del usuario '{userName}' obtenidos exitosamente", "ObtenerPermisosPorUsuario", TipoEvento.Message);
                }
                return _permisoRepository.ObtenerPermisosPorUsuario(userName);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al obtener permisos del usuario '{userName}'", "ObtenerPermisosPorUsuario", TipoEvento.Error);
                throw;
            }
        }
        public PermisoComponent ObtenerTodosLosPermisos()
        {
            try
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Todos los permisos obtenidos exitosamente", "ObtenerTodosLosPermisos", TipoEvento.Message);
                return _permisoRepository.ObtenerTodosLosPermisos();
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al obtener todos los permisos", "ObtenerTodosLosPermisos", TipoEvento.Error);
                throw;
            }
        }
        
    }
}
