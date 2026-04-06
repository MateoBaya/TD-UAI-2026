using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Configures the main MenuStrip for the Integridad DB module and injects the
    /// UI into pnlMain via FrmIntegridadDBFlexibilizador.
    ///
    /// Event wiring summary (mirrors FrmBitacoraHeaderConfig):
    ///   pnlMain_Resize  →  LastAction = ResizeControls   (dragged resize)
    ///   FrmPrincipal.Resize  →  _onFormResize = ResizeControls  (maximize / restore)
    ///   Both are subscribed inside IntegridadDBCreator and removed in
    ///   EliminarControlesAdicionales to avoid ghost handlers.
    ///
    ///   "Ver Integridad" menu click → RecalcularIntegridad()
    ///       re-fetches data without rebuilding controls, so it does NOT trigger
    ///       a full panel teardown + rebuild on what is already the active view.
    /// </summary>
    internal class FrmIntegridadDBHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmIntegridadDBFlexibilizador _flexibilizador;

        public FrmIntegridadDBHeaderConfig()
        {
            _formulario     = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador = new FrmIntegridadDBFlexibilizador();

            MenuStrip header = _formulario.MainMenuStrip;
            if (header != null)
            {
                var menuVerIntegridad = new ToolStripMenuItem("Ver Integridad")
                                            { Name = "verIntegridadToolStripMenuItem" };

                // Clicking the menu item refreshes data — it does NOT rebuild controls,
                // so it behaves the same as the Recalcular button rather than navigating
                // away and back. This avoids an unwanted re-verification on every resize.
                menuVerIntegridad.Click += (s, e) => _flexibilizador.RecalcularIntegridad();

                FrmPrincipalFlexibilizador.HeaderClearer(header);
                header.Items.Add(menuVerIntegridad);

                FlexibilizadorFormularios.MenuStripHider(
                    header, SessionManager.GetPermisos() as PermisoComponent);

                _formulario.AplicarIdiomaActual();
            }

            // Build controls + subscribe Form.Resize + load data once
            _flexibilizador.EliminarControlesAdicionales();
            _flexibilizador.IntegridadDBCreator();

            // LastAction handles dragged pnlMain resize — only repositions, never refetches
            _formulario.LastAction = (s, e) => _flexibilizador.ResizeControls();
        }
    }
}
