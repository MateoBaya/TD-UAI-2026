using System.Collections.Generic;

namespace IngSoft.UI.Dictionary
{
    public static class DictionaryPermisos
    {
        public static Dictionary<string, string> PermisoControl = new Dictionary<string, string>()
        {
            //Necesito dar vuelta el diccionario para que funcione bien

            { "usuariosToolStripMenuItem", "Usuarios" },
            { "agregarNuevoToolStripMenuItem","AltaUsuario" },
            { "modificarUsuarioToolStripMenuItem","ModificarUsuarios" },
            { "verTodosToolStripMenuItem","VerTodosUsuarios" },
            //{  "Eliminar Usuario","BajaUsuario" },
            { "permisosToolStripMenuItem", "Permisos" },
            {  "agregarPermisoToolStripMenuItem","AltaPermiso" },
            { "asignarPermisoToolStripMenuItem", "AsignarPermisos" },
            {  "modificarPermisoToolStripMenuItem","ModificarPermiso" },
            {  "eliminarPermisoToolStripMenuItem","BajaPermiso" },
            {  "bitacoraToolStripMenuItem", "Bitacora" },
            {  "controlDeCambiosToolStripMenuItem", "ControlCambios" },
            {  "multidiomasToolStripMenuItem", "Multidiomas" },
            {  "crearIdiomaToolStripMenuItem", "CrearIdioma" },
            {  "modificarIdiomaToolStripMenuItem", "ModificarIdioma" },
            {  "backupsToolStripMenuItem", "Backup" },
        };
    }

}
