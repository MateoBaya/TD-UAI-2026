using IngSoft.Domain;
using System.Collections.Generic;

namespace IngSoft.ApplicationServices
{
    public interface IProductoServices
    {
        void CrearProducto(Producto producto);
        void ModificarProducto(Producto producto);
        void EliminarProducto(Producto producto);
        Producto BuscarProducto(Producto producto);
        List<Producto> BuscarProductos(List<Producto> productos);
        List<Producto> BuscarProductosValidos(Producto producto);
    }
}
