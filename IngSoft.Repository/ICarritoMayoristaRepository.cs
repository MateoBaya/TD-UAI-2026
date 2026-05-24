using IngSoft.Domain;
using System.Collections.Generic;

namespace IngSoft.Repository
{
    public interface ICarritoMayoristaRepository
    {
        List<CarritoItem> MostrarDetalleCarrito();
        void CrearCarrito();
        bool AceptarCarrito();
        void CrearCarrito(CarritoItem item);
        bool ValidarItem(CarritoItem item);
        bool RechazarCarrito();
        bool FinalizarCarrito();
    }
}
