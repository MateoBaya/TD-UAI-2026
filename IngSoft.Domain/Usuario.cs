using IngSoft.Abstractions;
using System;

namespace IngSoft.Domain
{
    public class Usuario: Entity, IUsuario
    {
        public Guid IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string UserName { get; set; }
        public bool Bloqueado { get; set; } = false;
        public int CantidadIntentos { get; set; } = 0;
        public string Dvh { get; set; }
    }
}
