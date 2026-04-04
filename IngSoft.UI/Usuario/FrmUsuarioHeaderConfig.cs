using IngSoft.Abstractions;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.ApplicationServices.Implementation;
using IngSoft.Domain;
using IngSoft.UI.Multidioma;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal class FrmUsuarioHeaderConfig
    {
        FrmPrincipal formulario;
        FrmUsuarioFlexiblizador flexibilizador;
        EventHandler agregarUsuarioOnClick;
        EventHandler verUsuariosOnClick;
        EventHandler eliminarUsuarioOnClick;
        EventHandler modificarUsuarioOnClick;

        public FrmUsuarioHeaderConfig()
        {
            formulario = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            flexibilizador = new FrmUsuarioFlexiblizador();

            agregarUsuarioOnClick = AgregarUsuarioEventHandler;
            modificarUsuarioOnClick = ModificarUsuarioEventHandler;
            verUsuariosOnClick = VerTodosUsuarioEventHandler;
            eliminarUsuarioOnClick = EliminarUsuarioEventHandler;

            MenuStrip header = formulario.MainMenuStrip;
            if(header != null)
            {
                ToolStripMenuItem agregarUsuarioMenu, modificarUsuarioMenu, verUsuarioMenu, eliminarUsuarioMenu;

                agregarUsuarioMenu = new ToolStripMenuItem("Agregar Nuevo");
                agregarUsuarioMenu.Name = "agregarNuevoToolStripMenuItem";
                agregarUsuarioMenu.Click += agregarUsuarioOnClick;
                modificarUsuarioMenu = new ToolStripMenuItem("Modificar Usuario");
                modificarUsuarioMenu.Name = "modificarUsuarioToolStripMenuItem";
                modificarUsuarioMenu.Click += modificarUsuarioOnClick;
                verUsuarioMenu = new ToolStripMenuItem("Ver Todos");
                verUsuarioMenu.Name = "verTodosToolStripMenuItem";
                verUsuarioMenu.Click += verUsuariosOnClick;
                eliminarUsuarioMenu = new ToolStripMenuItem("Eliminar Usuario");
                eliminarUsuarioMenu.Name = "eliminarUsuarioToolStripMenuItem";
                eliminarUsuarioMenu.Click += eliminarUsuarioOnClick;
                List<ToolStripMenuItem> botonesIgnorar = new List<ToolStripMenuItem>();

                FrmPrincipalFlexibilizador.HeaderClearer(header);

                header.Items.Add(agregarUsuarioMenu);
                header.Items.Add(modificarUsuarioMenu);
                header.Items.Add(verUsuarioMenu);
                header.Items.Add(eliminarUsuarioMenu);

                FlexibilizadorFormularios.MenuStripHider(header,Services.SessionManager.GetPermisos() as PermisoComponent);
                formulario.AplicarIdiomaActual();
            }

        }


        private void AgregarUsuarioEventHandler(object o,EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionalesUsuario();
            flexibilizador.TextBoxCreator("UserName", new Point(formulario.GetPanelMain.Width / 2 - 230, formulario.GetPanelMain.Height / 8));
            flexibilizador.TextBoxCreator("Nombre", new Point((formulario.GetPanelMain.Width / 2 - 280) + (280), formulario.GetPanelMain.Height / 8));
            flexibilizador.TextBoxCreator("Apellido", new Point((formulario.GetPanelMain.Width / 2 - 230), formulario.GetPanelMain.Height / 4));
            flexibilizador.TextBoxCreator("Email", new Point((formulario.GetPanelMain.Width / 2 - 280) + (280), formulario.GetPanelMain.Height / 4));
            flexibilizador.TextBoxCreator("Contraseña", new Point((formulario.GetPanelMain.Width / 2 - 230), (formulario.GetPanelMain.Height / 8 + formulario.GetPanelMain.Height / 4)));
            flexibilizador.GuardarUsuarioButtonCreator();
            formulario.AplicarIdiomaActual();
            formulario.LastAction = AgregarUsuarioEventHandler;
        }

        private void ModificarUsuarioEventHandler(object o,EventArgs e)
        {

            flexibilizador.EliminarControlesAdicionalesUsuario();


            Point position = new Point(formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 8);
            Size size = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);


            flexibilizador.dataGridViewWithSelectionChanged(GetUsuarios(e), position, size);
            Point pointInicial = new Point(formulario.GetPanelMain.Width / 2 + 20, formulario.GetPanelMain.Height / 16);
            flexibilizador.TextBoxCreator("UserName", new Point(pointInicial.X, pointInicial.Y + formulario.GetPanelMain.Height / 10));
            flexibilizador.TextBoxCreator("Nombre", new Point(pointInicial.X, pointInicial.Y + (formulario.GetPanelMain.Height / 10) * 2));
            flexibilizador.TextBoxCreator("Apellido", new Point(pointInicial.X, pointInicial.Y + (formulario.GetPanelMain.Height / 10) * 3));
            flexibilizador.TextBoxCreator("Email", new Point(pointInicial.X, pointInicial.Y + (formulario.GetPanelMain.Height / 10) * 4));
            flexibilizador.CheckboxCreator("Bloqueado", new Point(pointInicial.X, pointInicial.Y + (formulario.GetPanelMain.Height / 10) * 6));
            flexibilizador.ModificarUsuarioButtonCreator(new Point(pointInicial.X, pointInicial.Y + (formulario.GetPanelMain.Height / 10) * 7));
            formulario.AplicarIdiomaActual();
            formulario.LastAction = ModificarUsuarioEventHandler;
        }

        private void EliminarUsuarioEventHandler(object o,EventArgs e)
        {
            var parent = formulario.GetPanelMain;
            if (parent == null) return;

            Point dgvPosition = new Point(formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 16);
            Size listSize = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height - formulario.GetPanelMain.Height / 6);
            Point txtPosition = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 6, formulario.GetPanelMain.Height / 4);
            Point buttonPosition = new Point(txtPosition.X, txtPosition.Y + 50);
            flexibilizador.EliminarControlesAdicionalesUsuario();

            // Create DataGridView with users (so selection fills textboxes similarly to modify flow)
            flexibilizador.dataGridViewWithSelectionChanged(GetUsuarios(e), dgvPosition, listSize);

            // Create textbox for username (will be filled by the DataGridView selection handler)
            flexibilizador.TextBoxCreator("UserName", txtPosition);

            // Create Button using flexible position
            flexibilizador.EliminarUsuarioButtonCreator(dgvPosition, listSize, txtPosition, buttonPosition);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = EliminarUsuarioEventHandler;
        }

        private void VerTodosUsuarioEventHandler(object o, EventArgs e)
        {

            flexibilizador.EliminarControlesAdicionalesUsuario();

            Point position = new Point(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 8);
            Size size = new Size(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);
            flexibilizador.DataGridViewUsuarioCreator(GetUsuarios(e), position, size);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = VerTodosUsuarioEventHandler;
        }

        private List<Usuario> GetUsuarios(EventArgs e)
        {
            if (!formulario.IsResizing(e))
            {
                try
                {
                    flexibilizador.Usuarios = flexibilizador.UsuarioServices.ObtenerUsuarios();
                }
                catch (Exception)
                {
                    // If fetching users fails, continue but leave list empty
                    flexibilizador.Usuarios = new List<Usuario>();
                }
            }
            return (flexibilizador.Usuarios != null && flexibilizador.Usuarios.Count > 0) ? flexibilizador.Usuarios : new List<Usuario>();
        }

    }

}
