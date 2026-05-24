using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngSoft.Repository.Implementation
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public ProductoRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public Producto BuscarProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            _connection.NuevaConexion(connectionString);
            try
            {
                return ObtenerProductoInterno(producto.Id, producto.Nombre);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<Producto> BuscarProductos(List<Producto> productos)
        {
            if (productos == null || productos.Count == 0)
                return ObtenerTodosProductos();

            var resultado = new List<Producto>();
            foreach (var producto in productos)
            {
                var encontrado = BuscarProducto(producto);
                if (encontrado != null)
                    resultado.Add(encontrado);
            }
            return resultado;
        }

        public List<Producto> BuscarProductosValidos(Producto producto)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    {"@Id", producto?.Id ?? Guid.Empty},
                    {"@Nombre", producto?.Nombre ?? string.Empty},
                    {"@Marca", producto?.Marca ?? string.Empty},
                    {"@Modelo", producto?.Modelo ?? string.Empty},
                    {"@PrecioActual", producto?.PrecioActual ?? 0f},
                    {"@AceptaMayorista", producto != null && producto.AceptaMayorista},
                    {"@AceptaMinorista", producto != null && producto.AceptaMinorista},
                    {"@Stock", producto?.Stock ?? 0}
                };

                return _connection.EjecutarDataSet<Producto>("BuscarProductosValidos", parametros);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void CrearProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            if (producto.Id == Guid.Empty)
                producto.Id = Guid.NewGuid();

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    {"@Id", producto.Id},
                    {"@Nombre", producto.Nombre ?? string.Empty},
                    {"@PrecioActual", producto.PrecioActual},
                    {"@Marca", producto.Marca ?? string.Empty},
                    {"@Modelo", producto.Modelo ?? string.Empty},
                    {"@AceptaMayorista", producto.AceptaMayorista},
                    {"@AceptaMinorista", producto.AceptaMinorista},
                    {"@Stock", producto.Stock}
                };

                _connection.EjecutarSinResultado("CrearProducto", parametros);
                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void EliminarProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var productoExistente = ObtenerProductoInterno(producto.Id, producto.Nombre);
                if (productoExistente == null)
                    throw new ArgumentException("El producto no existe");

                var parametros = new Dictionary<string, object>
                {
                    {"@Id", productoExistente.Id}
                };
                _connection.EjecutarSinResultado("EliminarProducto", parametros);
                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void ModificarProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            if (producto.Id == Guid.Empty && string.IsNullOrEmpty(producto.Nombre))
                throw new ArgumentException("Debe especificar el producto a modificar");

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var productoExistente = ObtenerProductoInterno(producto.Id, producto.Nombre);
                if (productoExistente == null)
                    throw new ArgumentException("El producto no existe");

                productoExistente.Nombre = producto.Nombre ?? productoExistente.Nombre;
                productoExistente.Marca = producto.Marca ?? productoExistente.Marca;
                productoExistente.Modelo = producto.Modelo ?? productoExistente.Modelo;
                productoExistente.PrecioActual = producto.PrecioActual != 0f ? producto.PrecioActual : productoExistente.PrecioActual;
                productoExistente.Stock = producto.Stock != 0 ? producto.Stock : productoExistente.Stock;
                productoExistente.AceptaMayorista = producto.AceptaMayorista;
                productoExistente.AceptaMinorista = producto.AceptaMinorista;

                var parametros = new Dictionary<string, object>
                {
                    {"@Id", productoExistente.Id},
                    {"@Nombre", productoExistente.Nombre ?? string.Empty},
                    {"@PrecioActual", productoExistente.PrecioActual},
                    {"@Marca", productoExistente.Marca ?? string.Empty},
                    {"@Modelo", productoExistente.Modelo ?? string.Empty},
                    {"@AceptaMayorista", productoExistente.AceptaMayorista},
                    {"@AceptaMinorista", productoExistente.AceptaMinorista},
                    {"@Stock", productoExistente.Stock}
                };

                _connection.EjecutarSinResultado("ModificarProducto", parametros);
                _connection.AceptarTransaccion();
            }
            catch (Exception)
            {
                _connection.CancelarTransaccion();
                throw;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        private Producto ObtenerProductoInterno(Guid id, string nombre)
        {
            var parametros = new Dictionary<string, object>
            {
                {"@Id", id},
                {"@Nombre", nombre ?? string.Empty}
            };
            var resultado = _connection.EjecutarDataSet<Producto>("ObtenerProducto", parametros);
            return resultado?.FirstOrDefault();
        }

        private List<Producto> ObtenerTodosProductos()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                return _connection.EjecutarDataSet<Producto>("ObtenerProductos", new Dictionary<string, object>());
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }
    }
}
