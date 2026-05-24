using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;
using System;

namespace IngSoft.ApplicationServices.Implementation
{
    public class VentaServices : GuardableEnBitacora, IVentaServices
    {
        private readonly IVentaRepository _ventaRepository;

        public VentaServices(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository ?? FactoryRepository.CreateVentaRepository();
        }

        public void GenerarVenta(Venta venta)
        {
            try
            {
                _ventaRepository.GenerarVenta(venta);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Venta generada exitosamente",
                    "GenerarVenta", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario,
                    "Error al generar venta",
                    "GenerarVenta", TipoEvento.Error);
                throw;
            }
        }
    }
}
