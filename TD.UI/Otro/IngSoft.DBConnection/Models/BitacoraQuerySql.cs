using System;

namespace IngSoft.DBConnection.Models
{
    public class BitacoraQuerySql
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Origen { get; set; }
        public int TipoEvento { get; set; }
        public string Dvh { get; set; }
        public Guid UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string UserName { get; set; }
    }
}
