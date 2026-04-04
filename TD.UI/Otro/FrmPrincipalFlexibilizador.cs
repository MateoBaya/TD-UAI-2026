using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IngSoft.Domain;
using IngSoft.Abstractions;
using IngSoft.UI.Dictionary;

namespace IngSoft.UI
{
    internal static class FrmPrincipalFlexibilizador
    {
        internal static void ActualizarMenuSegunEstadoSesion()
        {
            ToolStripMenuItem sesionbutton = FrmPrincipal.ActiveForm.MainMenuStrip.Items["sesionToolStripMenuItem"] as ToolStripMenuItem;
            Label lblPrincipal = FrmPrincipal.ActiveForm.Controls.Find("label1", true).FirstOrDefault() as Label;
            FlexibilizadorFormularios.MenuStripHider(FrmPrincipal.ActiveForm.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
            if (SessionManager.GetInstance().IsLoggedIn())
            {
                lblPrincipal.Visible = SessionManager.GetInstance().IsLoggedIn();
                IUsuario usuario= SessionManager.GetUsuario();
                sesionbutton.DropDownItems["cerrarSesionToolStripMenuItem"].Visible = true;
                sesionbutton.DropDownItems["iniciarSesionToolStripMenuItem"].Visible = false;
                lblPrincipal.Text = lblPrincipal.Visible ? $"{usuario.Nombre} {usuario.Apellido}" : "Usuario Desconectado";
            }
            else
            {
                lblPrincipal.Visible = false;
                sesionbutton.DropDownItems["cerrarSesionToolStripMenuItem"].Visible = false;
                sesionbutton.DropDownItems["iniciarSesionToolStripMenuItem"].Visible = true;
            }
        }
    }
}
