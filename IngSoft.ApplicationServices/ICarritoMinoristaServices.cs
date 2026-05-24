using IngSoft.Domain;

namespace IngSoft.ApplicationServices
{
    public interface ICarritoMinoristaServices
    {
        bool AgregarCantidadesItem(CarritoItem item);
        bool FinalizarCarrito();
    }
}
