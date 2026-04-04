using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Dto;
using IngSoft.ApplicationServices.Factory;
using IngSoft.ApplicationServices.Implementation;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmIntegridadDB : Form, IObserver
    {
        private ResultadoIntegridad _integridadDB;
        private readonly IMultidiomaServices _multidiomaServices;
        private readonly IDigitoVerificadorServices _digitoVerificadorServices;

        public FrmIntegridadDB(ResultadoIntegridad integridadDB)
        {
            InitializeComponent();
            _integridadDB = integridadDB;
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
            _digitoVerificadorServices = ServicesFactory.CreateDigitoVerificadorServices();
        }

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
        }

        private void verIntegridadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();

            Point ptDgv = new Point(this.Width / 16, this.Height / 6);
            Size szDgv = new Size(this.Width - this.Width / 8, this.Height / 2 + this.Height / 4);
            FrmIntegridadDBFlexibilizador.CrearVistaIntegridad(this, _integridadDB, ptDgv, szDgv);

            Point ptBtn = new Point(this.Width / 16, this.Height / 14);
            Size szBtn = new Size(187, 41);
            FrmIntegridadDBFlexibilizador.CrearBotonRecalcular(this, _digitoVerificadorServices, ptBtn, szBtn);

            AplicarIdiomaActual();
        }

        private void FrmIntegridadDB_Load(object sender, EventArgs e)
        {
            verIntegridadToolStripMenuItem_Click(null, null);
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
    }
}
