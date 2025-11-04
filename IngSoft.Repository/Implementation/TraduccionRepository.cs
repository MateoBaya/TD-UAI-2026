using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain.Multidioma;

namespace IngSoft.Repository.Implementation
{
    public class TraduccionRepository : ITraduccionRepository
    {
        private readonly IConnection _connection;
        private string _connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;

        public TraduccionRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public void ActualizarTraduccion(Traduccion traduccion)
        {
            try
            {
                _connection.NuevaConexion(_connectionString);
                var parametros = new Dictionary<string, object>
                {
                    { "@IdIdioma", traduccion.IdIdioma },
                    { "@IdControlIdioma", traduccion.IdControlIdioma },
                    { "@TextoTraducido", traduccion.TextoTraducido }
                };
                _connection.EjecutarSinResultado("ActualizarTraduccion", parametros);
                _connection.FinalizarConexion();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CrearTraduccion(Traduccion traduccion)
        {
            try
            {
                _connection.NuevaConexion(_connectionString);
                var parametros = new Dictionary<string, object>
                {
                    { "@Id", traduccion.Id },
                    { "@IdIdioma", traduccion.IdIdioma },
                    { "@IdControlIdioma", traduccion.IdControlIdioma },
                    { "@TextoTraducido", traduccion.TextoTraducido }
                };
                _connection.EjecutarSinResultado("CrearTraduccion", parametros);
                _connection.FinalizarConexion();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Traduccion ObtenerTraduccionPorIdiomaYControlIdioma(Guid idIdioma, Guid idControlIdioma)
        {
            try
            {
                _connection.NuevaConexion(_connectionString);
                
                var parametros = new Dictionary<string, object>
                {
                    { "@IdIdioma", idIdioma },
                    { "@IdControlIdioma", idControlIdioma }
                };

                var resultado = _connection.EjecutarDataSet<Traduccion>("ObtenerTraduccionPorIdiomaYControlIdioma", parametros);
                _connection.FinalizarConexion();

                return resultado.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
