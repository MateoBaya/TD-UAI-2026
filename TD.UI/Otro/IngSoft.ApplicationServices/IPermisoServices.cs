using System;
using IngSoft.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.ApplicationServices
{
    public interface IPermisoServices
    {
        void GuardarPermiso(PermisoComponent permiso);
        void AsignarPermisoEnUsuario(PermisoComponent permiso, string userName);
        void EliminarPermisoDeUsuario(PermisoComponent permiso, string userName);
        void ModificarPermiso(string permisoNombre, string permisoNombreNuevo, string permisoPadre=null);
        void EliminarPermiso(string permisoNombre);
        PermisoComponent ObtenerPermisosPorUsuario(string userName);
        PermisoComponent ObtenerTodosLosPermisos();
    }
}
