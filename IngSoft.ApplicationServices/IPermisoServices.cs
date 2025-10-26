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
        void GuardarPermisoEnUsuario(PermisoComponent permiso, string userName);
        void EliminarPermiso(string permisoNombre);
        PermisoComponent ObtenerPermisosPorUsuario(string userName);
        PermisoComponent ObtenerTodosLosPermisos();
    }
}
