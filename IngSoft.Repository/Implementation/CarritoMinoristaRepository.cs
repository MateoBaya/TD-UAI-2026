using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;

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

        public List<CarritoItem> MostrarDetalleCarrito()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                return _connection.EjecutarDataSet<CarritoItem>("MostrarDetalleCarritoMinorista", new Dictionary<string, object>());
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
                    { "@UsuarioId", usuario.IdUsuario }
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
                    { "@UsuarioId", usuario.IdUsuario },
                    { "@ProductoId", item.Producto.Id },
                    { "@Cantidad", item.Cantidad },
                    { "@Precio", item.Precio }
                };
                _connection.EjecutarSinResultado("CrearCarritoMinoristaConItem", parametros);
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
                var resultado = _connection.EjecutarEscalar("ValidarItemMinorista", parametros);
                return resultado != null && Convert.ToBoolean(resultado);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public bool AceptarCarrito(DateTime fechaEntrega)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId",    usuario.IdUsuario },
                    { "@FechaEntrega", fechaEntrega }
                };
                var resultado = _connection.EjecutarEscalar("AceptarCarritoMinorista", parametros);
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
                    { "@UsuarioId", usuario.IdUsuario}
                };
                var resultado = _connection.EjecutarEscalar("RechazarCarritoMinorista", parametros);
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
                var resultado = _connection.EjecutarEscalar("FinalizarCarritoMinorista", new Dictionary<string, object>());
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

        public List<Carrito> ObtenerCarritosPendientes()
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                return _connection.EjecutarDataSet<Carrito>("ObtenerCarritosPendientesMinorista", new Dictionary<string, object>());
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public List<CarritoItem> MostrarDetalleCarritoPorId(Guid carritoId)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CarritoId", carritoId }
                };
                return _connection.EjecutarDataSet<CarritoItem>("MostrarDetalleCarritoPorId", parametros);
            }
            finally
            {
                _connection.FinalizarConexion();
            }
        }

        public bool RechazarCarritoPorId(Guid carritoId)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuario.IdUsuario },
                    { "@CarritoId", carritoId }
                };
                var resultado = _connection.EjecutarEscalar("RechazarCarritoMinorista", parametros);
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

        public bool AceptarCarritoPorId(Guid carritoId, DateTime fechaEntrega)
        {
            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();
                var usuario = SessionManager.GetUsuario() as Usuario;
                var parametros = new Dictionary<string, object>
                {
                    { "@CarritoId",    carritoId },
                    { "@UsuarioId",    usuario.IdUsuario },
                    { "@FechaEntrega", fechaEntrega }
                };
                var resultado = _connection.EjecutarEscalar("AceptarCarritoPorId", parametros);
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
