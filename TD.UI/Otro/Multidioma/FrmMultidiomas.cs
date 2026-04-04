using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI.Multidioma
{
    public partial class FrmMultidiomas : Form, IObserver
    {
        private readonly IMultidiomaServices _multidiomaServices;

        public FrmMultidiomas()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
        }

        private void crearIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();

            Point ptNombre = new Point(this.Width / 3, this.Height / 4);
            Point ptCodigo = new Point(ptNombre.X, ptNombre.Y + 60);
            Point ptBtn = new Point(ptNombre.X, ptCodigo.Y + 60);

            FrmMultidiomasFlexibilizador.CrearPantallaCrearIdioma(this, _multidiomaServices, ptNombre, ptCodigo, ptBtn);
            AplicarIdiomaActual();
        }

        private void modificarIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();

            Point ptCombo = new Point(this.Width / 8, this.Height / 6);
            Point ptDgv = new Point(this.Width / 8, ptCombo.Y + 50);
            Size szDgv = new Size(this.Width * 3 / 4, this.Height / 2 + this.Height / 8);
            Point ptBtn = new Point(ptDgv.X, ptDgv.Y + szDgv.Height + 10);

            FrmMultidiomasFlexibilizador.CrearPantallaModificarIdioma(this, _multidiomaServices, ptCombo, ptDgv, szDgv, ptBtn);
            AplicarIdiomaActual();
        }

        private void FrmMultidiomas_Load(object sender, EventArgs e)
        {
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }

        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }

        private void SuscribirseAIdiomaActual()
        {
            var idioma = MultidiomaManager.GetIdioma();
            if (idioma != null)
            {
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
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }

        private void FrmMultidiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
