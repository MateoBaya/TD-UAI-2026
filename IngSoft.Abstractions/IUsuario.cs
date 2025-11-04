using System;
using System.Collections.Generic;

namespace IngSoft.Abstractions
{
    public interface IUsuario
    {
        Guid IdBitacora { get; set; }
        int IdUsuario { get; set; }
        string UserName {  get; set; }
        string Contrasena {  get; set; }
        string Nombre {  get; set; }
        string Apellido {  get; set; }
    }

}
