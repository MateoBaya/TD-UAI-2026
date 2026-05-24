using System;

namespace IngSoft.Domain
{
    public class Venta : Entity
    {
        public Usuario UsuarioAprobador { get; set; }
        public Carrito Carrito { get; set; }
        public DateTime FechaUpdate { get; set; }
        public Estado Estado { get; set; }
        public DateTime FechaEntrega { get; set; }
    }
}
