using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain;
using System;
using System.Collections.Generic;

namespace IngSoft.Repository.Implementation
{
    public class VentaRepository : IVentaRepository
    {
        private readonly IConnection _connection;
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public VentaRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public void GenerarVenta(Venta venta)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta));

            if (venta.Id == Guid.Empty)
                venta.Id = Guid.NewGuid();

            _connection.NuevaConexion(connectionString);
            try
            {
                _connection.IniciarTransaccion();

                var parametros = new Dictionary<string, object>
                {
                    { "@Id", venta.Id },
                    { "@CarritoId", venta.Carrito?.Id ?? Guid.Empty },
                    { "@UsuarioAprobadorId", venta.UsuarioAprobador?.Id ?? Guid.Empty },
                    { "@EstadoId", venta.Estado?.Id ?? Guid.Empty },
                    { "@FechaUpdate", venta.FechaUpdate },
                    { "@FechaEntrega", venta.FechaEntrega }
                };

                _connection.EjecutarSinResultado("GenerarVenta", parametros);
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
