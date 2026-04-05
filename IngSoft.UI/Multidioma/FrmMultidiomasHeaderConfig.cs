using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI.Multidioma
{
    /// <summary>
    /// Sets up the MenuStrip header for Multidiomas and delegates all UI creation
    /// to FrmMultidiomasFlexibilizador. Mirror of FrmUsuarioHeaderConfig.
    /// </summary>
    internal class FrmMultidiomasHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmMultidiomasFlexibilizador _flexibilizador;

        private readonly EventHandler _crearOnClick;
        private readonly EventHandler _modificarOnClick;

        public FrmMultidiomasHeaderConfig()
        {
            _formulario    = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador = new FrmMultidiomasFlexibilizador();

            _crearOnClick    = CrearIdiomaEventHandler;
            _modificarOnClick = ModificarIdiomaEventHandler;

            MenuStrip header = _formulario.MainMenuStrip;
            if (header == null) return;

            var menuCrear     = new ToolStripMenuItem("Crear Idioma")
                                    { Name = "crearIdiomaToolStripMenuItem" };
            var menuModificar = new ToolStripMenuItem("Modificar Idioma")
                                    { Name = "modificarIdiomaToolStripMenuItem" };

            menuCrear.Click     += _crearOnClick;
            menuModificar.Click += _modificarOnClick;

            FrmPrincipalFlexibilizador.HeaderClearer(header);
            header.Items.Add(menuCrear);
            header.Items.Add(menuModificar);

            FlexibilizadorFormularios.MenuStripHider(
                header, SessionManager.GetPermisos() as PermisoComponent);

            _formulario.AplicarIdiomaActual();

            // Auto-load the default view on open
            CrearIdiomaEventHandler(this, EventArgs.Empty);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void CrearIdiomaEventHandler(object sender, EventArgs e)
        {
            var panel = _formulario.GetPanelMain;
            _flexibilizador.EliminarControlesAdicionales();

            _flexibilizador.TextBoxCreator(
                "Nombre", new Point(panel.Width / 3, panel.Height / 4));
            _flexibilizador.TextBoxCreator(
                "Codigo", new Point(panel.Width / 3, panel.Height / 4 + 60));
            _flexibilizador.CrearIdiomaButtonCreator(
                new Point(panel.Width / 3, panel.Height / 4 + 120));

            _formulario.AplicarIdiomaActual();
            _formulario.LastAction = CrearIdiomaEventHandler;
        }

        private void ModificarIdiomaEventHandler(object sender, EventArgs e)
        {
            var panel = _formulario.GetPanelMain;
            _flexibilizador.EliminarControlesAdicionales();

            Point ptCombo = new Point(panel.Width / 8, panel.Height / 6);
            Point ptDgv   = new Point(panel.Width / 8, ptCombo.Y + 40);
            Size  szDgv   = new Size(panel.Width * 3 / 4,
                                     panel.Height / 2 + panel.Height / 8);
            Point ptBtn   = new Point(ptDgv.X, ptDgv.Y + szDgv.Height + 10);

            _flexibilizador.ComboBoxIdiomaCreator(ptCombo);
            _flexibilizador.DataGridViewControlesCreator(ptDgv, szDgv);
            _flexibilizador.GuardarIdiomaButtonCreator(ptBtn);

            _formulario.AplicarIdiomaActual();
            _formulario.LastAction = ModificarIdiomaEventHandler;
        }
    }
}
