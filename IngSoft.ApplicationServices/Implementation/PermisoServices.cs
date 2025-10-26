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

        public void GuardarPermisoEnUsuario(PermisoComponent permiso, string userName)
        {
            try
            {
                _permisoRepository.GuardarPermisoEnUsuario(permiso, userName);
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Permiso '{permiso.Nombre}' asignado al usuario '{userName}' exitosamente", "GuardarPermisoEnUsuario", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(new Usuario { IdUsuario = SessionManager.GetUsuario().IdUsuario }, $"Error al asignar permiso '{permiso.Nombre}' al usuario '{userName}'", "GuardarPermisoEnUsuario", TipoEvento.Error);
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
                return _permisoRepository.ObtenerPermisosPorUsuario(userName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PermisoComponent ObtenerTodosLosPermisos()
        {
            try
            {
                return _permisoRepository.ObtenerTodosLosPermisos();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
