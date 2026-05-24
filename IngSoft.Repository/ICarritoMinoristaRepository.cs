using IngSoft.Domain;
using System;

namespace IngSoft.Repository
{
    public interface ICarritoMinoristaRepository
    {
        CarritoMinorista ObtenerCarritoPendiente();
        void CrearCarrito(CarritoMinorista carrito);
        void AgregarItem(Guid carritoId, CarritoItem item);
        void FinalizarCarrito(Guid carritoId);
    }
}
