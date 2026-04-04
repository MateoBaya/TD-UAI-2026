using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using IngSoft.Domain;
using IngSoft.Services;
using IngSoft.UI.Permisos;

namespace IngSoft.UI.Login
{
    /// <summary>
    /// Configures the main MenuStrip for the Login module and injects the login
    /// UI into pnlMain via FrmLoginFlexibilizador.
    /// Mirrors the structure of FrmPermisosHeaderConfig.
    /// </summary>
    internal class FrmLoginHeaderConfig
    {
        private readonly FrmPrincipal _formulario;
        private readonly FrmLoginFlexibilizador _flexibilizador;

        public FrmLoginHeaderConfig()
        {
            _formulario = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            _flexibilizador = new FrmLoginFlexibilizador();

            // Wire the success callback: runs on the UI thread after a successful login
            _flexibilizador.OnLoginSuccess = OnLoginSuccess;

            MenuStrip header = _formulario.MainMenuStrip;
            if (header != null)
            {
                FrmPrincipalFlexibilizador.HeaderClearer(header);
                // Login module adds no extra top-level menu items beyond the cleared base;
                // the action is driven entirely from the side-panel button.
                _formulario.AplicarIdiomaActual();
                FlexibilizadorFormularios.MenuStripHider(
                    header,
                    SessionManager.GetPermisos() as PermisoComponent);
            }

            IniciarPantallaLogin();
        }

        // ── Screen builder ───────────────────────────────────────────────────────

        private void IniciarPantallaLogin()
        {
            _flexibilizador.EliminarControlesAdicionales();

            // Centre the login controls inside pnlMain
            int panelW = _formulario.GetPanelMain.Width;
            int panelH = _formulario.GetPanelMain.Height;

            int controlW = 200;
            int startX = panelW / 2 - controlW / 2;
            int startY = panelH / 4;

            Point pointUsuario = new Point(startX, startY);
            Point pointContrasena = new Point(startX, startY + 60);
            Point pointButton = new Point(startX, startY + 120);

            _flexibilizador.PantallaLoginCreator(pointUsuario, pointContrasena, pointButton);

            _formulario.LastAction = (s, e) => IniciarPantallaLogin();
        }

        // ── Post-login side-effects ──────────────────────────────────────────────

        /// <summary>
        /// Called by FrmLoginFlexibilizador after a successful login.
        /// Updates menu visibility and the side panel, then returns to Home.
        /// </summary>
        private void OnLoginSuccess()
        {
            FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
            FrmPrincipalFlexibilizador.ActualizarPanelVerticalSegunEstadoSesion();
        }
    }
}
