using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;

namespace IngSoft.ApplicationServices.Implementation
{
    public class CarritoMayoristaServices : CarritoServices
    {
        private readonly ICarritoMayoristaRepository _carritoMayoristaRepository;

        public CarritoMayoristaServices(ICarritoRepository carritoRepository, ICarritoMayoristaRepository carritoMayoristaRepository)
            : base(carritoRepository)
        {
            _carritoMayoristaRepository = carritoMayoristaRepository ?? FactoryRepository.CreateCarritoMayoristaRepository();
        }

        public override List<CarritoItem> MostrarDetalleCarrito()
        {
            try
            {
                return _carritoMayoristaRepository.MostrarDetalleCarrito();
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al mostrar detalle carrito mayorista",
                    "MostrarDetalleCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override void CrearCarrito()
        {
            try
            {
                _carritoMayoristaRepository.CrearCarrito();
                Carrito = new Carrito { FechaInsert = DateTime.Now };
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito mayorista creado",
                    "CrearCarrito", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al crear carrito mayorista",
                    "CrearCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override void CrearCarrito(CarritoItem item)
        {
            try
            {
                _carritoMayoristaRepository.CrearCarrito(item);
                Carrito = new Carrito { FechaInsert = DateTime.Now };
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito mayorista creado con item",
                    "CrearCarrito", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al crear carrito mayorista con item",
                    "CrearCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool ValidarItem(CarritoItem item)
        {
            try
            {
                return _carritoMayoristaRepository.ValidarItem(item);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al validar item mayorista",
                    "ValidarItem", TipoEvento.Error);
                throw;
            }
        }

        public override bool AprobarCarrito(DateTime fechaEntrega)
        {
            try
            {
                var resultado = _carritoMayoristaRepository.AceptarCarrito(fechaEntrega);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito mayorista aceptado",
                    "AprobarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al aceptar carrito mayorista",
                    "AprobarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool RechazarCarrito()
        {
            try
            {
                var resultado = _carritoMayoristaRepository.RechazarCarrito();
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito mayorista rechazado",
                    "RechazarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al rechazar carrito mayorista",
                    "RechazarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool FinalizarCarrito()
        {
            try
            {
                var resultado = _carritoMayoristaRepository.FinalizarCarrito();
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito mayorista finalizado",
                    "FinalizarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al finalizar carrito mayorista",
                    "FinalizarCarrito", TipoEvento.Error);
                throw;
            }
        }
    }
}
