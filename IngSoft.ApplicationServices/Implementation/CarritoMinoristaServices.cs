using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;

namespace IngSoft.ApplicationServices.Implementation
{
    public class CarritoMinoristaServices : GuardableEnBitacora, ICarritoMinoristaServices
    {
        private readonly ICarritoMinoristaRepository _carritoRepository;
        private readonly IProductoRepository _productoRepository;

        public CarritoMinoristaServices(ICarritoMinoristaRepository carritoRepository, IProductoRepository productoRepository)
        {
            _carritoRepository = carritoRepository ?? FactoryRepository.CreateCarritoMinoristaRepository();
            _productoRepository = productoRepository ?? FactoryRepository.CreateProductoRepository();
        }

        public bool AgregarCantidadesItem(CarritoItem item)
        {
            if (item == null || item.Producto == null || item.Cantidad <= 0)
                throw new ArgumentException("Datos del item inválidos.");

            try
            {
                var producto = _productoRepository.BuscarProducto(item.Producto);

                if (producto == null || !producto.AceptaMinorista)
                    throw new InvalidOperationException("El producto no puede ser comprado en Minorista.");

                item.PrecioUnitario = producto.PrecioActual;

                var carrito = _carritoRepository.ObtenerCarritoPendiente();

                if (carrito == null)
                {
                    carrito = new CarritoMinorista
                    {
                        Id = Guid.NewGuid(),
                        Finalizado = false,
                        FechaCreacion = DateTime.Now,
                        Items = new System.Collections.Generic.List<CarritoItem>()
                    };
                    _carritoRepository.CrearCarrito(carrito);
                }

                _carritoRepository.AgregarItem(carrito.Id, item);

                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    $"Item agregado al carrito minorista: {producto.Nombre} x{item.Cantidad}",
                    "AgregarCantidadesItem", TipoEvento.Message);

                return true;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al agregar item al carrito minorista",
                    "AgregarCantidadesItem", TipoEvento.Error);
                throw;
            }
        }

        public bool FinalizarCarrito()
        {
            try
            {
                var carrito = _carritoRepository.ObtenerCarritoPendiente();

                if (carrito == null || carrito.Finalizado)
                    throw new InvalidOperationException("Carro ya estaba finalizado.");

                _carritoRepository.FinalizarCarrito(carrito.Id);

                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista finalizado correctamente",
                    "FinalizarCarrito", TipoEvento.Message);

                return true;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al finalizar el carrito minorista",
                    "FinalizarCarrito", TipoEvento.Error);
                throw;
            }
        }
    }
}
