using System;
using System.Collections.Generic;
using System.Linq;

namespace IngSoft.Domain
{
    public class CarritoMinorista : Entity
    {
        private List<CarritoItem> items;
        private bool finalizado;
        private DateTime fechaCreacion;

        public List<CarritoItem> Items { get => items; set => items = value; }
        public bool Finalizado { get => finalizado; set => finalizado = value; }
        public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
        public float Total => items?.Sum(i => i.Subtotal) ?? 0f;
    }
}
