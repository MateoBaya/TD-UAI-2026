using IngSoft.Domain;
using System;
using System.Collections.Generic;

namespace IngSoft.Repository
{
    public interface ICarritoMinoristaRepository
    {
        List<CarritoItem> MostrarDetalleCarrito();
        void CrearCarrito();
        bool AceptarCarrito();
        void CrearCarrito(CarritoItem item);
        bool ValidarItem(CarritoItem item);
        bool RechazarCarrito();
        bool FinalizarCarrito();
        List<Carrito> ObtenerCarritosPendientes();
        List<CarritoItem> MostrarDetalleCarritoPorId(Guid carritoId);
        bool AceptarCarritoPorId(Guid carritoId);
    }
}
