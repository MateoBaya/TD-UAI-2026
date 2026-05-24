using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngSoft.Repository.Implementation
{
    public class CarritoMinoristaRepository : ICarritoMinoristaRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public CarritoMinoristaRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public CarritoMinorista ObtenerCarritoPendiente()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@Finalizado", false }
                };

                var carritos = _connection.EjecutarDataSet<CarritoMinorista>("ObtenerCarritoMinoristaPendiente", parametros);
                var carrito = carritos?.FirstOrDefault();

                if (carrito == null)
                    return null;

                var itemsParams = new Dictionary<string, object>
                {
                    { "@CarritoId", carrito.Id }
                };

                var items = _connection.EjecutarDataSet<CarritoItem>("ObtenerItemsCarritoMinorista", itemsParams);
                carrito.Items = items ?? new List<CarritoItem>();

                return carrito;
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void CrearCarrito(CarritoMinorista carrito)
        {
            if (carrito == null)
                throw new ArgumentNullException(nameof(carrito));

            if (carrito.Id == Guid.Empty)
                carrito.Id = Guid.NewGuid();

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    { "@Id", carrito.Id },
                    { "@Finalizado", carrito.Finalizado },
                    { "@FechaCreacion", carrito.FechaCreacion }
                };

                _connection.EjecutarSinResultado("CrearCarritoMinorista", parametros);
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

        public void AgregarItem(Guid carritoId, CarritoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    { "@Id", item.Id },
                    { "@CarritoId", carritoId },
                    { "@ProductoId", item.Producto.Id },
                    { "@Cantidad", item.Cantidad },
                    { "@PrecioUnitario", item.PrecioUnitario }
                };

                _connection.EjecutarSinResultado("AgregarItemCarritoMinorista", parametros);
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

        public void FinalizarCarrito(Guid carritoId)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    { "@Id", carritoId },
                    { "@Finalizado", true }
                };

                _connection.EjecutarSinResultado("FinalizarCarritoMinorista", parametros);
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
    }
}
