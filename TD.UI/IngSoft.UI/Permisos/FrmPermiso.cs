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



        private void FrmPermiso_Shown(object sender, EventArgs e)
        {
            FlexibilizadorFormularios.MenuStripHider(this.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
        }

        private void asignarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void verTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void eliminarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void modificarPermisoToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
