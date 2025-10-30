using System;
using IngSoft.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Repository
{
    public interface IPermisoRepository
    {
        void GuardarPermiso(PermisoComponent permiso);
        void AsignarPermisoEnUsuario(PermisoComponent permisoComposite, string pUserName);
        void EliminarPermisosDeUsuario(PermisoComponent permisoComposite, string pUserName);
        void ModificarPermiso(string permisoNombre, string nuevoNombre);
        void EliminarPermiso(string permisoNombre);
        PermisoComponent ObtenerPermisosPorUsuario(string userName);
        PermisoComponent ObtenerTodosLosPermisos();

    }
}
