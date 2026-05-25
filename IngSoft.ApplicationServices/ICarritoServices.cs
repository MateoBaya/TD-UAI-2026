using IngSoft.Domain;
using System;
using System.Collections.Generic;

namespace IngSoft.ApplicationServices
{
    public interface ICarritoServices
    {
        bool AgregarCantidadesItem(CarritoItem item);
        float PrecioTotal();
        Carrito BuscarCarrito(Carrito carrito);
        List<Carrito> BuscarCarritosFiltrados(Usuario usuario, DateTime fechaDesde, DateTime fechaHasta);
        List<CarritoItem> MostrarDetalleCarrito();
        void CrearCarrito();
        void CrearCarrito(CarritoItem item);
        bool ValidarItem(CarritoItem item);
        bool AprobarCarrito(DateTime fechaEntrega);
        bool RechazarCarrito();
        bool FinalizarCarrito();
        List<Carrito> ObtenerCarritosPendientes();
        List<CarritoItem> MostrarDetalleCarrito(Guid carritoId);
        bool AprobarCarrito(Guid carritoId, DateTime fechaEntrega);
    }
}
