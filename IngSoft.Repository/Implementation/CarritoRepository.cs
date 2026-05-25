using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngSoft.Repository.Implementation
{
    public class CarritoRepository : ICarritoRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public CarritoRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public bool AgregarCantidadesItem(CarritoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    { "@Id", item.Id == Guid.Empty ? Guid.NewGuid() : item.Id },
                    { "@ProductoId", item.Producto.Id },
                    { "@Cantidad", item.Cantidad },
                    { "@Precio", item.Precio }
                };

                _connection.EjecutarSinResultado("AgregarCantidadesItemCarrito", parametros);
                _connection.AceptarTransaccion();
                return true;
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

        public Carrito BuscarCarrito(Carrito carrito)
        {
            if (carrito == null)
                throw new ArgumentNullException(nameof(carrito));

            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@Id", carrito.Id },
                    { "@NroCarrito", carrito.NroCarrito }
                };

                var resultado = _connection.EjecutarDataSet<Carrito>("BuscarCarrito", parametros);
                return resultado?.FirstOrDefault();
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<Carrito> BuscarCarritosFiltrados(Usuario usuario, DateTime fechaDesde, DateTime fechaHasta)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuario?.Id ?? Guid.Empty },
                    { "@FechaDesde", fechaDesde },
                    { "@FechaHasta", fechaHasta }
                };

                return _connection.EjecutarDataSet<Carrito>("BuscarCarritosFiltrados", parametros);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<PedidoResumen> ObtenerPedidosPorUsuario(Guid usuarioId)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId }
                };
                return _connection.EjecutarDataSet<PedidoResumen>("ObtenerPedidosPorUsuario", parametros);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<PedidoItem> ObtenerDetallePedido(Guid carritoId)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CarritoId", carritoId }
                };
                return _connection.EjecutarDataSet<PedidoItem>("ObtenerDetallePedidoPorId", parametros);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }
    }
}
