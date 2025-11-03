using IngSoft.Domain;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace IngSoft.UI
{
    public partial class FrmPermiso : Form
    {
        public FrmPermiso()
        {
            InitializeComponent();
        }

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
        }

        private void FrmPermiso_Shown(object sender, EventArgs e)
        {
            FlexibilizadorFormularios.MenuStripHider(this.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
        }

        private void asignarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();
            Point pointVerTodos = new Point(this.Width / 12, this.Height / 8);
            Size sizeVerTodos = new Size(this.Width / 4, this.Height / 2 + this.Height / 4);
            FrmPermisosFlexibilizador.AsignarPermisosVerTodosDelSistema(pointVerTodos,sizeVerTodos);
            Size sizeLista = new Size(this.Width / 4 - this.Width/16, this.Height/8);
            Point pointLista = new Point(this.Width / 2 - sizeLista.Width/2, this.Height / 8);
            Point pointArbol = new Point(this.Width /2 + this.Width/6, this.Height / 8);
            Size sizeArbol = new Size(this.Width / 4, this.Height / 2 + this.Height / 4);
            FrmPermisosFlexibilizador.ListaConTodosUsuarios(pointLista,sizeLista,pointArbol,sizeArbol);
            Point pointButtonAsignar = new Point(this.Width / 2 - 50, pointLista.Y + sizeLista.Height + 10);
            Size sizeButtonAsignar = new Size (100, 30);
            FrmPermisosFlexibilizador.BotonAsignarPermisosCreator(pointButtonAsignar, sizeButtonAsignar);
            Point pointButtonQuitar = new Point(this.Width / 2 -50, pointButtonAsignar.Y + sizeButtonAsignar.Height + 10);
            Size sizeButtonQuitar = new Size(100, 30);
            FrmPermisosFlexibilizador.BotonQuitarPermisosCreator(pointButtonQuitar, sizeButtonQuitar);
            Point pointButtonGuardar = new Point(this.Width / 2 - 60, pointButtonQuitar.Y + sizeButtonQuitar.Height + 20);
            Size sizeButtonGuardar = new Size(120, 40);
            FrmPermisosFlexibilizador.BotonGuardarCambiosPermisosCreator(pointButtonGuardar,sizeButtonGuardar);
            Size sizePermisosPendientesAsignar = new Size(this.Width/8, this.Height / 4);
            Point pointPermisosPendientesAsignar = new Point(this.Width/2 - sizePermisosPendientesAsignar.Width -15, (pointButtonGuardar.Y + sizeButtonGuardar.Height)+ 20);
            FrmPermisosFlexibilizador.PermisosPendientesAsignarTreeViewCreator(pointPermisosPendientesAsignar, sizePermisosPendientesAsignar);
            Size sizePermisosPendientesQuitar = new Size(this.Width / 8, this.Height / 4);
            Point pointPermisosPendientesQuitar = new Point(pointPermisosPendientesAsignar.X + sizePermisosPendientesAsignar.Width +15, pointPermisosPendientesAsignar.Y);
            FrmPermisosFlexibilizador.PermisosPendientesRemoverTreeViewCreator(pointPermisosPendientesQuitar, sizePermisosPendientesQuitar);
        }

        private void verTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(this.Width / 8, this.Height / 8);
            Size sizeArbolSelector = new Size(this.Width/2 - this.Width / 4, this.Height - this.Height / 4);
            FrmPermisosFlexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector, FrmPermisosFlexibilizador.Tree_AfterSelect_FillTextboxes);
            Point pointNombre = new Point(this.Width / 2 + this.Width / 16, this.Height / 4);
            Point pointPadre = new Point(pointNombre.X, pointNombre.Y + 50);
            Point pointButton = new Point(pointNombre.X, pointPadre.Y + 50);
            FrmPermisosFlexibilizador.PantallaAgregarPermisoCreator(pointNombre,pointPadre,pointButton);
        }

        private void eliminarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(this.Width / 8, this.Height / 8);
            Size sizeArbolSelector = new Size(this.Width / 2 - this.Width / 4, this.Height - this.Height / 4);
            FrmPermisosFlexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector,FrmPermisosFlexibilizador.Tree_AfterSelect_FillTextboxes_Delete);
            Point pointNombre = new Point(this.Width / 2 + this.Width / 16, this.Height / 4);
            Point pointButton = new Point(pointNombre.X, pointNombre.Y + 50);
            FrmPermisosFlexibilizador.PantallaEliminarPermisoCreator(pointNombre, pointButton);
        }

        private void modificarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();
            Point pointArbolSelector = new Point(this.Width / 8, this.Height / 8);
            Size sizeArbolSelector = new Size(this.Width / 2 - this.Width / 4, this.Height - this.Height / 4);
            FrmPermisosFlexibilizador.CrearArbolPermisosConSelector(pointArbolSelector, sizeArbolSelector,FrmPermisosFlexibilizador.Tree_AfterSelect_FillTextboxes_Modify);
            Point pointNombre = new Point(this.Width / 2 + this.Width / 16, this.Height / 4);
            Point pointNuevoNombre = new Point(pointNombre.X, pointNombre.Y + 50);
            Point pointPadre = new Point(pointNombre.X, pointNuevoNombre.Y + 50);
            Point pointButton = new Point(pointNombre.X, pointPadre.Y + 50);
            FrmPermisosFlexibilizador.PantallaModificarPermisoCreator(pointNombre, pointNuevoNombre, pointPadre, pointButton);
        }
    }
}
