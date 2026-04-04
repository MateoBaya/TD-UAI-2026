using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.Repository
{
    public interface IBitacoraRepository
    {
        void GuardarBitacora(Bitacora bitacora);
        List<Bitacora> ObtenerBitacoras();
        List<Bitacora> ObtenerBitacorasFiltradas(string filtro);
    }
}
