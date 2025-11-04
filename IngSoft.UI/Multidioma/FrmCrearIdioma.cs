using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI.Multidioma
{
    public partial class FrmCrearIdioma : Form, IObserver
    {
        private readonly IMultidiomaServices _multidiomaServices;
        public FrmCrearIdioma()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtIdioma.Text))
            {
                MessageBox.Show("El nombre del idioma no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código del idioma no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var nuevoIdioma = new Idioma
            {
                Id = Guid.NewGuid(),
                Nombre = txtIdioma.Text.Trim(),
                Codigo = txtCodigo.Text.Trim()
            };

            try
            {
                _multidiomaServices.CrearIdioma(nuevoIdioma);
                MessageBox.Show("Idioma creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdioma.Text = string.Empty;
                txtCodigo.Text = string.Empty;
            }
            catch (Exception)
            {
                MessageBox.Show("Se quiere crear un Idioma que ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void FrmCrearIdioma_Load(object sender, EventArgs e)
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
    }
}
