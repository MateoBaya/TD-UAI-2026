using IngSoft.Abstractions;
using IngSoft.Domain;
using IngSoft.Services;
using IngSoft.UI.BitacoraUI;
using IngSoft.UI.Dictionaries;
using IngSoft.UI.Login;
using IngSoft.UI.Multidioma;
using IngSoft.UI.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal static class FrmPrincipalFlexibilizador
    {
        internal static void ActualizarMenuSegunEstadoSesion()
        {
            FrmPrincipal formulario = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            ToolStripMenuItem sesionbutton = formulario.MainMenuStrip.Items["sesionToolStripMenuItem"] as ToolStripMenuItem;

            FlexibilizadorFormularios.MenuStripHider(formulario.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
            if (SessionManager.GetInstance().IsLoggedIn())
            {
                IUsuario usuario = SessionManager.GetUsuario();
                sesionbutton.DropDownItems["cerrarSesionToolStripMenuItem"].Visible = true;
                sesionbutton.DropDownItems["iniciarSesionToolStripMenuItem"].Visible = false;
                formulario.Home(formulario, null);
            }
            else
            {
                sesionbutton.DropDownItems["cerrarSesionToolStripMenuItem"].Visible = false;
                sesionbutton.DropDownItems["iniciarSesionToolStripMenuItem"].Visible = true;
            }
        }

        internal static void ActualizarPanelVerticalSegunEstadoSesion()
        {
            FrmPrincipal form = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            Panel pnlVertical = form.Controls.Find("pnlNavBar", true).FirstOrDefault() as Panel;
            if (pnlVertical != null)
            {
                if (SessionManager.GetInstance().IsLoggedIn())
                {
                    FlexibilizadorFormularios.LoadSidePanelFromPermisos(
                        pnlVertical,
                        SessionManager.GetPermisos() as PermisoComponent,
                        SidePanelButtons(),
                        SidePanelButtonsActions());
                }
                else
                {
                    FlexibilizadorFormularios.SidePanelClearer(pnlVertical, form.ControlesSalvarSidePanel());
                }
            }
        }

        public static void HeaderClearer(MenuStrip header)
        {
            List<ToolStripMenuItem> botonesIgnorar = new List<ToolStripMenuItem>();
            botonesIgnorar.Add((header.Items.Find("menuToolStripMenuItem",    false))[0] as ToolStripMenuItem);
            botonesIgnorar.Add((header.Items.Find("xToolStripMenuItem",       false))[0] as ToolStripMenuItem);
            botonesIgnorar.Add((header.Items.Find("maximizeToolStripMenuItem",false))[0] as ToolStripMenuItem);
            botonesIgnorar.Add((header.Items.Find("minimizeToolStripMenuItem",false))[0] as ToolStripMenuItem);
            botonesIgnorar.Add((header.Items.Find("sesionToolStripMenuItem",  false))[0] as ToolStripMenuItem);
            FlexibilizadorFormularios.MenuStripClearer(header, botonesIgnorar);
        }

        public static Dictionary<string, EventHandler> SidePanelButtonsActions()
        {
            return new Dictionary<string, EventHandler>
            {
                { "usuariosToolStripMenuItem",        usuarioOnclick },
                { "bitacoraToolStripMenuItem",         bitacoraOnclick },
                { "permisosToolStripMenuItem",         permisosOnclick },
                { "multidiomasToolStripMenuItem",      multidiomasOnclick },
                { "backupsToolStripMenuItem",          backupsOnclick },
                { "controlDeCambiosToolStripMenuItem", controlDeCambiosOnclick },
            };
        }

        public static Dictionary<string, string> SidePanelButtons()
        {
            return new Dictionary<string, string>
            {
                { "usuariosToolStripMenuItem",        "Usuarios" },
                { "bitacoraToolStripMenuItem",         "Bitacora" },
                { "permisosToolStripMenuItem",         "Permisos" },
                { "multidiomasToolStripMenuItem",      "Multidiomas" },
                { "backupsToolStripMenuItem",          "Backup" },
                { "controlDeCambiosToolStripMenuItem", "Control de Cambios" },
            };
        }

        // ── Side-panel button handlers ───────────────────────────────────────────

        static EventHandler usuarioOnclick = (sender, e) =>
        {
            FrmUsuarioHeaderConfig   headerConfig   = new FrmUsuarioHeaderConfig();
            FrmUsuarioFlexiblizador  flexibilizador = new FrmUsuarioFlexiblizador();
        };

        /// <summary>
        /// Bitacora is now injected into pnlMain via FrmBitacoraHeaderConfig,
        /// replacing the old ShowDialog call.
        /// </summary>
        static EventHandler bitacoraOnclick = (sender, e) =>
        {
            new FrmBitacoraHeaderConfig();
        };

        static EventHandler permisosOnclick = (sender, e) =>
        {
            FrmPermisosHeaderConfig  headerConfig   = new FrmPermisosHeaderConfig();
            FrmPermisosFlexibilizador flexibilizador = new FrmPermisosFlexibilizador();
        };

        static EventHandler controlDeCambiosOnclick = (sender, e) =>
        {
            var frmControlCambios = new FrmControlDeCambios();
            frmControlCambios.ShowDialog();
        };

        static EventHandler multidiomasOnclick = (sender, e) =>
        {
            var frmMultidiomas = new FrmMultidiomas();
            frmMultidiomas.ShowDialog();
        };

        static EventHandler backupsOnclick = (sender, e) =>
        {
            var frmBackups = new FrmBackUp();
            frmBackups.ShowDialog();
        };

        internal static void CerrarMenuLateral()
        {
            try
            {
                Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>().CerrarPanelLateralSuave();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "No se pudo cerrar el panel lateral, es posible que no se haya abierto correctamente o que ya esté cerrado.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
