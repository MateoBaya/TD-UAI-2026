using System;
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

        private void FrmIntegridadDB_Load(object sender, EventArgs e)
        {
            var errores = _integridadDB.Errores;
            dgvIntegridad.DataSource = errores;
            dgvIntegridad.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            _digitoVerificadorServices.RecaulcularDigitosVerificadores();
            MessageBox.Show("Dígitos verificadores recalculados correctamente.");
            this.Close();
        }
    }
}
