using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Abstractions
{
    // Interfaz que contiene únicamente las propiedades/atributos
    public interface IComposible
    {
        string Nombre { get; set; }
        string ParentName { get; }

    }

    // Interfaz que contiene únicamente la metodología/operaciones
    public interface ICompositableMethodology
    {
        bool Operacion(string userAction);
        ICompositable AddCompositable(ICompositable compositable);
        ICompositable RemoveCompositable(ICompositable compositable);
        ICompositable ClearCompositable();
        void RaisePermisoEliminado();
        void RaisePermisoAsignado(ICompositable padre);
        bool EstaAsignadoPorComponent(ICompositable permisoComponent);
        ICompositable EncontrarRoot();
    }

    // Interfaz compuesta para compatibilidad: agrupa propiedades y métodos
    public interface ICompositable : IComposible, ICompositableMethodology
    {
    }
}
