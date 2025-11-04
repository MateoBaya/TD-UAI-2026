using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;

namespace IngSoft.Services
{
    public abstract class MultidiomaManager
    {
        private static IIdioma _idioma;
        private static readonly Dictionary<Guid, IIdioma> _idiomasCache = new Dictionary<Guid, IIdioma>();
        public static IIdioma GetIdioma()
        {
            return _idioma;
        }
        public static void SetIdioma(IIdioma idioma)
        {
            _idioma = idioma;
        }
        public static IIdioma ObtenerIdiomaCache(IIdioma idioma)
        {
            if (idioma == null) return null;

            
            if (_idiomasCache.ContainsKey(idioma.Id))
            {                
                var idiomaCacheado = _idiomasCache[idioma.Id];
                idiomaCacheado.Nombre = idioma.Nombre;
                idiomaCacheado.Codigo = idioma.Codigo;
                idiomaCacheado.isDefault = idioma.isDefault;
                return idiomaCacheado;
            }

            _idiomasCache[idioma.Id] = idioma;
            return idioma;
        }
        public static List<IIdioma> ObtenerIdiomasCache(List<IIdioma> idiomas)
        {
            if (idiomas == null) return new List<IIdioma>();

            return idiomas.Select(i => ObtenerIdiomaCache(i)).ToList();
        }
        public static void LimpiarCacheIdiomas()
        {
            _idiomasCache.Clear();
        }

        public static void CambiarIdiomaControles(Form thisForm, List<IControlIdioma> controlIdiomas)
        {
            // Cambiar idioma de controles estándar
            CambiarIdiomaControlesRecursivo(thisForm.Controls, controlIdiomas);

            // Cambiar idioma de MenuStrip y sus items
            foreach (Control control in thisForm.Controls)
            {
                if (control is MenuStrip menuStrip)
                {
                    CambiarIdiomaMenuStrip(menuStrip.Items, controlIdiomas);
                }
            }
        }

        private static void CambiarIdiomaControlesRecursivo(Control.ControlCollection controls, List<IControlIdioma> controlIdiomas)
        {
            foreach (Control control in controls)
            {
                foreach (var idioma in controlIdiomas)
                {
                    if (control.Name == idioma.NombreControl)
                    {
                        control.Text = idioma.TextoTraducido;
                        break;
                    }
                }

                // Recursión para controles contenedores (GroupBox, Panel, TabControl, etc.)
                if (control.HasChildren)
                {
                    CambiarIdiomaControlesRecursivo(control.Controls, controlIdiomas);
                }
            }
        }

        private static void CambiarIdiomaMenuStrip(ToolStripItemCollection items, List<IControlIdioma> controlIdiomas)
        {
            foreach (ToolStripItem item in items)
            {
                foreach (var idioma in controlIdiomas)
                {
                    if (item.Name == idioma.NombreControl)
                    {
                        item.Text = idioma.TextoTraducido;
                        break;
                    }
                }

                // Procesar sub-items (menús desplegables)
                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    CambiarIdiomaMenuStrip(menuItem.DropDownItems, controlIdiomas);
                }
            }
        }
    }
}
