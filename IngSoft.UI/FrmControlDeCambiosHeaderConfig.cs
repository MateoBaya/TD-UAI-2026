using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Sets up the MenuStrip header for Control de Cambios and delegates all UI
    /// creation to FrmControlDeCambiosFlexibilizador.
    /// The view is search-driven (no auto-load on open) to match the original form
    /// which required user input before showing any data.
    /// </summary>
    internal class FrmControlDeCambiosHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmControlDeCambiosFlexibilizador _flexibilizador;

        private readonly EventHandler _buscarCambiosOnClick;

        public FrmControlDeCambiosHeaderConfig()
        {
            _formulario           = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador       = new FrmControlDeCambiosFlexibilizador();
            _buscarCambiosOnClick = BuscarCambiosEventHandler;

            MenuStrip header = _formulario.MainMenuStrip;
            if (header == null) return;

            var menuBuscar = new ToolStripMenuItem("Buscar Cambios")
                                 { Name = "buscarCambiosToolStripMenuItem" };
            menuBuscar.Click += _buscarCambiosOnClick;

            FrmPrincipalFlexibilizador.HeaderClearer(header);
            header.Items.Add(menuBuscar);

            FlexibilizadorFormularios.MenuStripHider(
                header, SessionManager.GetPermisos() as PermisoComponent);

            _formulario.AplicarIdiomaActual();

            // Render the empty search form immediately (filters visible, grid empty until Buscar is clicked)
            BuscarCambiosEventHandler(this, EventArgs.Empty);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void BuscarCambiosEventHandler(object sender, EventArgs e)
        {
            var panel = _formulario.GetPanelMain;
            _flexibilizador.EliminarControlesAdicionales();

            // Filter row at the top
            Point ptFiltros      = new Point(panel.Width / 16, panel.Height / 10);
            // DGV below the filters
            Point ptDgv          = new Point(panel.Width / 40, panel.Height / 5);
            Size  szDgv          = new Size(panel.Width * 5 / 6,
                                            panel.Height / 2 + panel.Height / 8);
            // Restaurar button to the right of the DGV
            Point ptBtnRestaurar = new Point(ptDgv.X + szDgv.Width + 10, ptDgv.Y + 15);
            Size  szBtn          = new Size(84, 28);

            _flexibilizador.FiltrosBusquedaCreator(ptFiltros);
            _flexibilizador.DataGridViewCambiosCreator(ptDgv, szDgv);
            _flexibilizador.RestaurarButtonCreator(ptBtnRestaurar, szBtn);

            _formulario.AplicarIdiomaActual();
            _formulario.LastAction = BuscarCambiosEventHandler;
        }
    }
}
