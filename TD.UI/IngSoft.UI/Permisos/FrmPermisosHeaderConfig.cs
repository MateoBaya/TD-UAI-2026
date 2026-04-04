using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IngSoft.Domain;

namespace IngSoft.UI.Permisos
{
    internal class FrmPermisosHeaderConfig
    {
        FrmPrincipal formulario;
        FrmPermisosFlexibilizador flexibilizador;
        EventHandler agregarPermisoOnClick;
        EventHandler modificarPermisoOnClick;
        EventHandler eliminarPermisoOnClick;
        EventHandler asignarPermisoOnClick;

        public FrmPermisosHeaderConfig()
        {
            formulario = Services.SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
            flexibilizador = new FrmPermisosFlexibilizador();
            agregarPermisoOnClick = AgregarPermisoEventHandler;
            modificarPermisoOnClick = ModificarPermisoEventHandler;
            eliminarPermisoOnClick = EliminarPermisoEventHandler;
            asignarPermisoOnClick = AsignarPermisoEventHandler;
            MenuStrip header = formulario.MainMenuStrip;
            if (header != null)
            {
                ToolStripMenuItem agregarPermisoMenu, modificarPermisoMenu, eliminarPermisoMenu, asignarPermisoMenu;
                agregarPermisoMenu = new ToolStripMenuItem("Agregar Nuevo");
                agregarPermisoMenu.Name = "agregarPermisoToolStripMenuItem";
                agregarPermisoMenu.Click += agregarPermisoOnClick;
                modificarPermisoMenu = new ToolStripMenuItem("Modificar Permiso");
                modificarPermisoMenu.Name = "modificarPermisoToolStripMenuItem";
                modificarPermisoMenu.Click += modificarPermisoOnClick;
                eliminarPermisoMenu = new ToolStripMenuItem("Eliminar Permiso");
                eliminarPermisoMenu.Name = "eliminarPermisoToolStripMenuItem";
                eliminarPermisoMenu.Click += eliminarPermisoOnClick;
                asignarPermisoMenu = new ToolStripMenuItem("Asignar Permisos");
                asignarPermisoMenu.Name = "asignarPermisoToolStripMenuItem";
                asignarPermisoMenu.Click += asignarPermisoOnClick;
                List<ToolStripMenuItem> botonesIgnorar = new List<ToolStripMenuItem>();
                FrmPrincipalFlexibilizador.HeaderClearer(header);
                header.Items.Add(agregarPermisoMenu);
                header.Items.Add(modificarPermisoMenu);
                header.Items.Add(eliminarPermisoMenu);
                header.Items.Add(asignarPermisoMenu);
                formulario.AplicarIdiomaActual();
                FlexibilizadorFormularios.MenuStripHider(header, Services.SessionManager.GetPermisos() as PermisoComponent);
            }
        }

        private void AgregarPermisoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 8);
            Size sizeArbolSelector = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height - formulario.GetPanelMain.Height / 4);
            flexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector, flexibilizador.Tree_AfterSelect_FillTextboxes);
            Point pointNombre = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 4);
            Point pointPadre = new Point(pointNombre.X, pointNombre.Y + 50);
            Point pointButton = new Point(pointNombre.X, pointPadre.Y + 50);
            flexibilizador.PantallaAgregarPermisoCreator(pointNombre, pointPadre, pointButton);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = AgregarPermisoEventHandler;
        }

        private void ModificarPermisoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 8);
            Size sizeArbolSelector = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height - formulario.GetPanelMain.Height / 4);
            flexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector, flexibilizador.Tree_AfterSelect_FillTextboxes_Modify);
            Point pointNombre = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 4);
            Point pointNuevoNombre = new Point(pointNombre.X, pointNombre.Y + 50);
            Point pointPadre = new Point(pointNombre.X, pointNuevoNombre.Y + 50);
            Point pointButton = new Point(pointNombre.X, pointPadre.Y + 50);
            flexibilizador.PantallaModificarPermisoCreator(pointNombre, pointNuevoNombre, pointPadre, pointButton);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = ModificarPermisoEventHandler;
        }

        private void EliminarPermisoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 8);
            Size sizeArbolSelector = new Size(formulario.GetPanelMain.Width / 2 - formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height - formulario.GetPanelMain.Height / 4);
            flexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector, flexibilizador.Tree_AfterSelect_FillTextboxes_Delete);
            Point pointNombre = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 4);
            Point pointButton = new Point(pointNombre.X, pointNombre.Y + 50);
            flexibilizador.PantallaEliminarPermisoCreator(pointNombre, pointButton);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = EliminarPermisoEventHandler;
        }

        private void AsignarPermisoEventHandler(object sender, EventArgs e)
        {
            flexibilizador.EliminarControlesAdicionales();
            Point pointVerTodos = new Point(formulario.GetPanelMain.Width / 12, formulario.GetPanelMain.Height / 8);
            Size sizeVerTodos = new Size(formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);
            flexibilizador.AsignarPermisosVerTodosDelSistema(pointVerTodos, sizeVerTodos);
            Size sizeLista = new Size(formulario.GetPanelMain.Width / 4 - formulario.GetPanelMain.Width / 16, formulario.GetPanelMain.Height / 8);
            Point pointLista = new Point(formulario.GetPanelMain.Width / 2 - sizeLista.Width / 2, formulario.GetPanelMain.Height / 8);
            Point pointArbol = new Point(formulario.GetPanelMain.Width / 2 + formulario.GetPanelMain.Width / 6, formulario.GetPanelMain.Height / 8);
            Size sizeArbol = new Size(formulario.GetPanelMain.Width / 4, formulario.GetPanelMain.Height / 2 + formulario.GetPanelMain.Height / 4);
            flexibilizador.ListaConTodosUsuarios(pointLista, sizeLista, pointArbol, sizeArbol);
            Point pointButtonAsignar = new Point(formulario.GetPanelMain.Width / 2 - 50, pointLista.Y + sizeLista.Height + 10);
            Size sizeButtonAsignar = new Size(100, 30);
            flexibilizador.BotonAsignarPermisosCreator(pointButtonAsignar, sizeButtonAsignar);
            Point pointButtonQuitar = new Point(formulario.GetPanelMain.Width / 2 - 50, pointButtonAsignar.Y + sizeButtonAsignar.Height + 10);
            Size sizeButtonQuitar = new Size(100, 30);
            flexibilizador.BotonQuitarPermisosCreator(pointButtonQuitar, sizeButtonQuitar);
            Point pointButtonGuardar = new Point(formulario.GetPanelMain.Width / 2 - 60, pointButtonQuitar.Y + sizeButtonQuitar.Height + 20);
            Size sizeButtonGuardar = new Size(120, 40);
            flexibilizador.BotonGuardarCambiosPermisosCreator(pointButtonGuardar, sizeButtonGuardar);
            Size sizePermisosPendientesAsignar = new Size(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 4);
            Point pointPermisosPendientesAsignar = new Point(formulario.GetPanelMain.Width / 2 - sizePermisosPendientesAsignar.Width - 15, (pointButtonGuardar.Y + sizeButtonGuardar.Height) + 20);
            flexibilizador.PermisosPendientesAsignarTreeViewCreator(pointPermisosPendientesAsignar, sizePermisosPendientesAsignar);
            Size sizePermisosPendientesQuitar = new Size(formulario.GetPanelMain.Width / 8, formulario.GetPanelMain.Height / 4);
            Point pointPermisosPendientesQuitar = new Point(pointPermisosPendientesAsignar.X + sizePermisosPendientesAsignar.Width + 15, pointPermisosPendientesAsignar.Y);
            flexibilizador.PermisosPendientesRemoverTreeViewCreator(pointPermisosPendientesQuitar, sizePermisosPendientesQuitar);
            formulario.AplicarIdiomaActual();
            formulario.LastAction = AsignarPermisoEventHandler;
        }
    }
}
