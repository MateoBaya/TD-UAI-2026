using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;

namespace IngSoft.ApplicationServices.Implementation
{
    public abstract class CarritoServices : GuardableEnBitacora, ICarritoServices
    {
        protected Carrito Carrito;
        private readonly ICarritoRepository _carritoRepository;

        protected CarritoServices(ICarritoRepository carritoRepository)
        {
            _carritoRepository = carritoRepository ?? FactoryRepository.CreateCarritoRepository();
        }

        public bool AgregarCantidadesItem(CarritoItem item)
        {
            try
            {
                if (!ValidarItem(item))
                    throw new InvalidOperationException("El item no es válido para este tipo de carrito.");

                if (Carrito == null)
                    CrearCarrito(item);

                var resultado = _carritoRepository.AgregarCantidadesItem(item);
                Carrito?.AgregarCantidadesItem(item);

                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    $"Item agregado al carrito: {item.Producto?.Nombre} x{item.Cantidad}",
                    "AgregarCantidadesItem", TipoEvento.Message);

                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al agregar item al carrito",
                    "AgregarCantidadesItem", TipoEvento.Error);
                throw;
            }
        }

        public float PrecioTotal()
        {
            var items = MostrarDetalleCarrito();
            float total = 0f;
            foreach (var item in items)
                total += item.Precio * item.Cantidad;
            return total;
        }

        public Carrito BuscarCarrito(Carrito carrito)
        {
            try
            {
                return _carritoRepository.BuscarCarrito(carrito);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al buscar carrito",
                    "BuscarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public List<Carrito> BuscarCarritosFiltrados(Usuario usuario, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                return _carritoRepository.BuscarCarritosFiltrados(usuario, fechaDesde, fechaHasta);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al buscar carritos filtrados",
                    "BuscarCarritosFiltrados", TipoEvento.Error);
                throw;
            }
        }

        public virtual List<CarritoItem> MostrarDetalleCarrito()
        {
            return Carrito?.MostrarDetalleCarrito() ?? new List<CarritoItem>();
        }

        public abstract void CrearCarrito();
        public abstract void CrearCarrito(CarritoItem item);
        public abstract bool ValidarItem(CarritoItem item);
        public abstract bool AprobarCarrito(DateTime fechaEntrega);
        public abstract bool RechazarCarrito();
        public abstract bool FinalizarCarrito();

        public virtual List<Carrito> ObtenerCarritosPendientes()
        {
            throw new NotSupportedException("Este tipo de carrito no soporta la operacion ObtenerCarritosPendientes.");
        }

        public virtual List<CarritoItem> MostrarDetalleCarrito(Guid carritoId)
        {
            throw new NotSupportedException("Este tipo de carrito no soporta la operacion MostrarDetalleCarrito por Id.");
        }

        public virtual bool AprobarCarrito(Guid carritoId, DateTime fechaEntrega)
        {
            throw new NotSupportedException("Este tipo de carrito no soporta la operacion AprobarCarrito por Id.");
        }
    }
}
