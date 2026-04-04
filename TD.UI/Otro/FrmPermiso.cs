using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmPermiso : Form, IObserver
    {
        private readonly IMultidiomaServices _multidiomaServices;

        public FrmPermiso()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
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
            AplicarIdiomaActual();
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
            AplicarIdiomaActual();
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
            AplicarIdiomaActual();
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
            AplicarIdiomaActual();
        }

        private void FrmPermiso_Load(object sender, EventArgs e)
        {
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }
        private void SuscribirseAIdiomaActual()
        {
            var idioma = MultidiomaManager.GetIdioma();

            if (idioma != null)
            {
                // Asegurarse de que sea la instancia cacheada
                if (idioma is Idioma idiomaConcreto)
                {
                    var idiomaCacheado = MultidiomaManager.ObtenerIdiomaCache(idiomaConcreto);
                    idiomaCacheado.Suscribir(this);
                }
                else
                {
                    idioma.Suscribir(this);
                }
            }
        }
        private void AplicarIdiomaActual()
        {
            // Aplicar el idioma actual al formulario
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }
        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }
    }
}
