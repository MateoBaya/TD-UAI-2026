using System.Collections.Generic;

namespace IngSoft.DBConnection
{
    public interface IConnection
    {
        void NuevaConexion(string connectionString);
        void FinalizarConexion();
        void IniciarTransaccion();
        void AceptarTransaccion();
        void CancelarTransaccion();
        void EjecutarSinResultado(string query, Dictionary<string, object> parametros);
        object EjecutarEscalar(string query, Dictionary<string, object> parametros);
        int ObtenerUltimoId(string tabla, string columnaId);
        List<T> EjecutarDataTable<T>(string query, Dictionary<string, object> parametros) where T : new();
        List<T> EjecutarDataSet<T>(string storeProcedure, Dictionary<string, object> parametros) where T : new();
    }
}
