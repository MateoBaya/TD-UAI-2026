using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;
using System.Collections.Generic;

namespace IngSoft.ApplicationServices.Implementation
{
    public class CarritoMinoristaServices : CarritoServices
    {
        private readonly ICarritoMinoristaRepository _carritoMinoristaRepository;

        public CarritoMinoristaServices(ICarritoRepository carritoRepository, ICarritoMinoristaRepository carritoMinoristaRepository)
            : base(carritoRepository)
        {
            _carritoMinoristaRepository = carritoMinoristaRepository ?? FactoryRepository.CreateCarritoMinoristaRepository();
        }

        public override List<CarritoItem> MostrarDetalleCarrito()
        {
            try
            {
                return _carritoMinoristaRepository.MostrarDetalleCarrito();
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al mostrar detalle carrito minorista",
                    "MostrarDetalleCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override void CrearCarrito()
        {
            try
            {
                _carritoMinoristaRepository.CrearCarrito();
                Carrito = new Carrito { FechaInsert = DateTime.Now };
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista creado",
                    "CrearCarrito", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al crear carrito minorista",
                    "CrearCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override void CrearCarrito(CarritoItem item)
        {
            try
            {
                _carritoMinoristaRepository.CrearCarrito(item);
                Carrito = new Carrito { FechaInsert = DateTime.Now };
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista creado con item",
                    "CrearCarrito", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al crear carrito minorista con item",
                    "CrearCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool ValidarItem(CarritoItem item)
        {
            try
            {
                return _carritoMinoristaRepository.ValidarItem(item);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al validar item minorista",
                    "ValidarItem", TipoEvento.Error);
                throw;
            }
        }

        public override bool AprobarCarrito(DateTime fechaEntrega)
        {
            try
            {
                var resultado = _carritoMinoristaRepository.AceptarCarrito(fechaEntrega);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista aceptado",
                    "AprobarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al aceptar carrito minorista",
                    "AprobarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool RechazarCarrito()
        {
            try
            {
                var resultado = _carritoMinoristaRepository.RechazarCarrito();
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista rechazado",
                    "RechazarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al rechazar carrito minorista",
                    "RechazarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool RechazarCarrito(Guid carritoId)
        {
            try
            {
                var resultado = _carritoMinoristaRepository.RechazarCarritoPorId(carritoId);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    $"Carrito minorista rechazado por Id: {carritoId}",
                    "RechazarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al rechazar carrito minorista por Id",
                    "RechazarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool FinalizarCarrito()
        {
            try
            {
                var resultado = _carritoMinoristaRepository.FinalizarCarrito();
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Carrito minorista finalizado correctamente",
                    "FinalizarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al finalizar carrito minorista",
                    "FinalizarCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override List<Carrito> ObtenerCarritosPendientes()
        {
            try
            {
                return _carritoMinoristaRepository.ObtenerCarritosPendientes();
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al obtener carritos pendientes minorista",
                    "ObtenerCarritosPendientes", TipoEvento.Error);
                throw;
            }
        }

        public override List<CarritoItem> MostrarDetalleCarrito(Guid carritoId)
        {
            try
            {
                return _carritoMinoristaRepository.MostrarDetalleCarritoPorId(carritoId);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al mostrar detalle de carrito minorista por Id",
                    "MostrarDetalleCarrito", TipoEvento.Error);
                throw;
            }
        }

        public override bool AprobarCarrito(Guid carritoId, DateTime fechaEntrega)
        {
            try
            {
                var resultado = _carritoMinoristaRepository.AceptarCarritoPorId(carritoId, fechaEntrega);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    $"Carrito minorista aceptado por Id: {carritoId}",
                    "AprobarCarrito", TipoEvento.Message);
                return resultado;
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al aceptar carrito minorista por Id",
                    "AprobarCarrito", TipoEvento.Error);
                throw;
            }
        }
    }
}
