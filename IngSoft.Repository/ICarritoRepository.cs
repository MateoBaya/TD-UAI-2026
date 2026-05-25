using IngSoft.Domain;
using System;
using System.Collections.Generic;

namespace IngSoft.Repository
{
    public interface ICarritoRepository
    {
        bool AgregarCantidadesItem(CarritoItem item);
        Carrito BuscarCarrito(Carrito carrito);
        List<Carrito> BuscarCarritosFiltrados(Usuario usuario, DateTime fechaDesde, DateTime fechaHasta);
        List<PedidoResumen> ObtenerPedidosPorUsuario(Guid usuarioId);
        List<PedidoItem>    ObtenerDetallePedido(Guid carritoId);
    }
}
