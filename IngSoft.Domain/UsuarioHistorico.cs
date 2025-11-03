using System;
using IngSoft.Domain.Enums;

namespace IngSoft.Domain
{
    public class UsuarioHistorico : Entity
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Bloqueado { get; set; } = false;
        public int CantidadIntentos { get; set; } = 0;
        public DateTime FechaModificacion { get; set; }
        public TipoOperacion TipoOperacion { get; set; }
        public string UsuarioModificador { get; set; }
    }
}
