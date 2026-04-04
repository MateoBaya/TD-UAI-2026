using IngSoft.UI.Multidioma;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IngSoft.UI.Dictionaries
{
    public static class DictionaryPermisos
    {
        public static Dictionary<string, string> PermisoControl = new Dictionary<string, string>()
        {

            { "usuariosToolStripMenuItem", "Usuarios" },
            { "agregarNuevoToolStripMenuItem","AltaUsuario" },
            { "modificarUsuarioToolStripMenuItem","ModificarUsuarios" },
            { "verTodosToolStripMenuItem","VerTodosUsuarios" },
            { "eliminarUsuarioToolStripMenuItem","BajaUsuario" },
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
