using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;

namespace IngSoft.Repository.Implementation
{
    public class CarritoMayoristaRepository : ICarritoMayoristaRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public CarritoMayoristaRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public List<CarritoItem> MostrarDetalleCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                return _connection.EjecutarDataSet<CarritoItem>("MostrarDetalleCarritoMayorista", new Dictionary<string, object>());
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public void CrearCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@Id", Guid.NewGuid() },
                    { "@FechaInsert", DateTime.Now },
                    { "@UsuarioId", usuario?.Id ?? Guid.Empty }
                };
                _connection.EjecutarSinResultado("CrearCarritoMayorista", parametros);
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

        public void CrearCarrito(CarritoItem item)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@Id", Guid.NewGuid() },
                    { "@FechaInsert", DateTime.Now },
                    { "@UsuarioId", usuario?.Id ?? Guid.Empty },
                    { "@ProductoId", item.Producto.Id },
                    { "@Cantidad", item.Cantidad },
                    { "@Precio", item.Precio }
                };
                _connection.EjecutarSinResultado("CrearCarritoMayoristaConItem", parametros);
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

        public bool ValidarItem(CarritoItem item)
        {
            if (item?.Producto == null || item.Cantidad <= 0)
                return false;

            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@ProductoId", item.Producto.Id },
                    { "@Cantidad", item.Cantidad }
                };
                var resultado = _connection.EjecutarEscalar("ValidarItemMayorista", parametros);
                return resultado != null && Convert.ToBoolean(resultado);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public bool AceptarCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuario?.Id ?? Guid.Empty }
                };
                var resultado = _connection.EjecutarEscalar("AceptarCarritoMayorista", parametros);
                _connection.AceptarTransaccion();
                return resultado != null && Convert.ToBoolean(resultado);
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

        public bool RechazarCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuario?.Id ?? Guid.Empty }
                };
                var resultado = _connection.EjecutarEscalar("RechazarCarritoMayorista", parametros);
                _connection.AceptarTransaccion();
                return resultado != null && Convert.ToBoolean(resultado);
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

        public bool FinalizarCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var resultado = _connection.EjecutarEscalar("FinalizarCarritoMayorista", new Dictionary<string, object>());
                _connection.AceptarTransaccion();
                return resultado != null && Convert.ToBoolean(resultado);
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
