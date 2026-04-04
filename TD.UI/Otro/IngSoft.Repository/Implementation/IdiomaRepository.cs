using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain.Multidioma;

namespace IngSoft.Repository.Implementation
{
    public class IdiomaRepository : IIdiomaRepository
    {
        private readonly IConnection _connection;
        private string _connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
        public IdiomaRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }

        public void CrearIdioma(Idioma idioma)
        {
            try
            {
                _connection.NuevaConexion(_connectionString);
                
                var parametros = new Dictionary<string, object>
                {
                    { "@Id", idioma.Id },
                    { "@Nombre", idioma.Nombre },
                    {"@Codigo", idioma.Codigo },
                    {"@isDefault", idioma.isDefault }
                };
                
                _connection.EjecutarSinResultado("CrearIdioma", parametros);
                _connection.FinalizarConexion();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Idioma ObtenerIdiomaPorDefecto()
        {
            try
            {
                _connection.NuevaConexion(_connectionString);

                var resultado = _connection.EjecutarDataSet<Idioma>("ObtenerIdiomaPorDefecto", new Dictionary<string, object>());

                _connection.FinalizarConexion();
                return resultado.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Idioma> ObtenerIdiomas()
        {
            try
            {
                _connection.NuevaConexion(_connectionString);

                var resultado = _connection.EjecutarDataSet<Idioma>("ObtenerIdiomas", new Dictionary<string, object>());

                _connection.FinalizarConexion();
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
