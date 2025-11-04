using System;
using System.Collections.Generic;
using System.Configuration;
using IngSoft.DBConnection;
using IngSoft.DBConnection.Factory;
using IngSoft.Domain.Multidioma;

namespace IngSoft.Repository.Implementation
{
    public class ControlIdiomaRepository : IControlIdiomaRepository
    {
        private readonly IConnection _connection;
        private string _connectionString = ConfigurationManager.ConnectionStrings["IngSoftConnection"].ConnectionString;
        public ControlIdiomaRepository(IConnection connection)
        {
            _connection = connection ?? ConnectionFactory.CreateSqlServerConnection();
        }
        public List<ControlIdioma> ObtenerControlesPorIdioma(Guid idiomaId)
        {
            try
            {
                _connection.NuevaConexion(_connectionString);
                var parametros = new Dictionary<string, object>
                {
                    {"@IdIdioma", idiomaId }
                };

                var resultado = _connection.EjecutarDataSet<ControlIdioma>("ObtenerControlesPorIdioma", parametros);

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
