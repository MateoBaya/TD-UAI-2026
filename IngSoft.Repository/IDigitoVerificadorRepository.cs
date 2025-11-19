using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Repository.Dto;

namespace IngSoft.Repository
{
    public interface IDigitoVerificadorRepository
    {
        void ActualizarDVV(string tabla, string nuevoDv);
        List<DigitoVerificador> ObtenerDigitosVerificadores();
        string CrearDVH(object entity);
        string CrearDVV(string tabla);
        ResultadoIntegridad ValidarIntegridad();
        void RecalcularDigitosVerificadores();
    }
}
