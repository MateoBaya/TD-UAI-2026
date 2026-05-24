using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Repository
{
    public interface IProductoRepository
    {
        void CrearProducto(Producto producto);
        void ModificarProducto(Producto producto);
        void EliminarProducto(Producto producto);
        Producto BuscarProducto(Producto producto);
        List<Producto> BuscarProductos(List<Producto> productos);
        List<Producto> BuscarProductosValidos(Producto producto);
    }
}
