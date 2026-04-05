using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Sets up the MenuStrip header for Integridad DB and delegates UI creation
    /// to FrmIntegridadDBFlexibilizador. Auto-loads the integrity view on construction,
    /// matching the original FrmIntegridadDB_Load behaviour.
    /// </summary>
    internal class FrmIntegridadDBHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmIntegridadDBFlexibilizador _flexibilizador;

        private readonly EventHandler _verIntegridadOnClick;

        public FrmIntegridadDBHeaderConfig()
        {
            _formulario          = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador      = new FrmIntegridadDBFlexibilizador();
            _verIntegridadOnClick = VerIntegridadEventHandler;

            MenuStrip header = _formulario.MainMenuStrip;
            if (header == null) return;

            var menuVerIntegridad = new ToolStripMenuItem("Ver Integridad")
                                        { Name = "verIntegridadToolStripMenuItem" };
            menuVerIntegridad.Click += _verIntegridadOnClick;

            FrmPrincipalFlexibilizador.HeaderClearer(header);
            header.Items.Add(menuVerIntegridad);

            FlexibilizadorFormularios.MenuStripHider(
                header, SessionManager.GetPermisos() as PermisoComponent);

            _formulario.AplicarIdiomaActual();

            // Auto-load on open (mirrors verIntegridadToolStripMenuItem_Click in FrmIntegridadDB_Load)
            VerIntegridadEventHandler(this, EventArgs.Empty);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void VerIntegridadEventHandler(object sender, EventArgs e)
        {
            var panel = _formulario.GetPanelMain;
            _flexibilizador.EliminarControlesAdicionales();

            // Mirrors the coordinate calculation in FrmIntegridadDB.verIntegridadToolStripMenuItem_Click
            Point ptDgv = new Point(panel.Width / 16, panel.Height / 6);
            Size  szDgv = new Size(panel.Width - panel.Width / 8,
                                   panel.Height / 2 + panel.Height / 4);

            Point ptBtn = new Point(panel.Width / 16, panel.Height / 14);
            Size  szBtn = new Size(187, 41);

            _flexibilizador.RecalcularButtonCreator(ptBtn, szBtn);
            _flexibilizador.DataGridViewIntegridadCreator(ptDgv, szDgv);

            _formulario.AplicarIdiomaActual();
            _formulario.LastAction = VerIntegridadEventHandler;
        }
    }
}
