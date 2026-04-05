using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Sets up the MenuStrip header for Control de Cambios and delegates UI creation
    /// to FrmControlDeCambiosFlexibilizador. Auto-loads the search view on construction,
    /// matching the original FrmControlDeCambios_Load behaviour.
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

            // Auto-load on open (mirrors buscarCambiosToolStripMenuItem_Click in FrmControlDeCambios_Load)
            BuscarCambiosEventHandler(this, EventArgs.Empty);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void BuscarCambiosEventHandler(object sender, EventArgs e)
        {
            var panel = _formulario.GetPanelMain;
            _flexibilizador.EliminarControlesAdicionales();

            // Mirrors the coordinate calculation in FrmControlDeCambios.buscarCambiosToolStripMenuItem_Click
            Point ptTitle       = new Point(panel.Width / 2 - 120, panel.Height / 20);
            Point ptFiltros     = new Point(panel.Width / 16, panel.Height / 8);
            Point ptDgv         = new Point(panel.Width / 40, panel.Height / 4);
            Size  szDgv         = new Size(panel.Width * 5 / 6,
                                           panel.Height / 2 + panel.Height / 8);
            Point ptBtnRestaurar = new Point(ptDgv.X + szDgv.Width + 10, ptDgv.Y + 15);
            Size  szBtn          = new Size(84, 28);

            _flexibilizador.TituloCreator("Control de Cambios", ptTitle);
            _flexibilizador.FiltrosFechaCreator(ptFiltros);
            _flexibilizador.DataGridViewCambiosCreator(ptDgv, szDgv);
            _flexibilizador.RestaurarButtonCreator(ptBtnRestaurar, szBtn);

            _formulario.AplicarIdiomaActual();
            _formulario.LastAction = BuscarCambiosEventHandler;
        }
    }
}
