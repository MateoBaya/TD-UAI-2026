using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.ApplicationServices
{
    public interface IBitacoraServices
    {
        void GuardarBitacora(Bitacora bitacora);
        List<Bitacora> ObtenerBitacoras();
        List<Bitacora> ObtenerBitacorasFiltradas(string filtro);
    }
}
