using System;
using System.Collections.Generic;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class ProductoServices : GuardableEnBitacora, IProductoServices
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoServices(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository ?? FactoryRepository.CreateProductoRepository();
        }

        public void CrearProducto(Producto producto)
        {
            try
            {
                _productoRepository.CrearProducto(producto);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Producto creado exitosamente", "CrearProducto", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al crear producto", "CrearProducto", TipoEvento.Error);
                throw;
            }
        }

        public void ModificarProducto(Producto producto)
        {
            try
            {
                _productoRepository.ModificarProducto(producto);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Producto modificado exitosamente", "ModificarProducto", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al modificar producto", "ModificarProducto", TipoEvento.Error);
                throw;
            }
        }

        public void EliminarProducto(Producto producto)
        {
            try
            {
                _productoRepository.EliminarProducto(producto);
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Producto eliminado exitosamente", "EliminarProducto", TipoEvento.Message);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al eliminar producto", "EliminarProducto", TipoEvento.Error);
                throw;
            }
        }

        public Producto BuscarProducto(Producto producto)
        {
            try
            {
                return _productoRepository.BuscarProducto(producto);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al buscar producto", "BuscarProducto", TipoEvento.Error);
                throw;
            }
        }

        public List<Producto> BuscarProductos(List<Producto> productos)
        {
            try
            {
                return _productoRepository.BuscarProductos(productos);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al buscar productos", "BuscarProductos", TipoEvento.Error);
                throw;
            }
        }

        public List<Producto> BuscarProductosValidos(Producto producto)
        {
            try
            {
                return _productoRepository.BuscarProductosValidos(producto);
            }
            catch (Exception)
            {
                _registrarEnBitacora(SessionManager.GetUsuario() as Usuario, "Error al buscar productos válidos", "BuscarProductosValidos", TipoEvento.Error);
                throw;
            }
        }
    }
}
