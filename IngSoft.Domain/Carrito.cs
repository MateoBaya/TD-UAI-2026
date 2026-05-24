using System;
using System.Collections.Generic;

namespace IngSoft.Domain
{
    public class Carrito : Entity
    {
        private List<CarritoItem> items;

        public int NroCarrito { get; set; }
        public Usuario Usuario { get; set; }
        public Estado Estado { get; set; }
        public DateTime FechaInsert { get; set; }
        public DateTime FechaUpdate { get; set; }

        public void AgregarCantidadesItem(CarritoItem item)
        {
            if (items == null)
                items = new List<CarritoItem>();

            var existente = items.Find(i => i.Producto?.Id == item.Producto?.Id);
            if (existente != null)
                existente.Cantidad += item.Cantidad;
            else
                items.Add(item);
        }

        public void ReducirCantidadesItem(CarritoItem item)
        {
            if (items == null) return;

            var existente = items.Find(i => i.Producto?.Id == item.Producto?.Id);
            if (existente == null) return;

            existente.Cantidad -= item.Cantidad;
            if (existente.Cantidad <= 0)
                items.Remove(existente);
        }

        public List<CarritoItem> MostrarDetalleCarrito()
        {
            return items ?? new List<CarritoItem>();
        }
    }
}
