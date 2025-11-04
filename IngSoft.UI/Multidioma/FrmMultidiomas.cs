using System;
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
        private Panel panelContenedor;
        private Form formularioActual;

        public FrmMultidiomas()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
            InicializarPanelContenedor();
        }

        private void modificarIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MostrarFormularioEnPanel(new Modificar_Idioma());
        }

        private void crearIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MostrarFormularioEnPanel(new FrmCrearIdioma());
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
        private void InicializarPanelContenedor()
        {
            panelContenedor = new Panel
            {
                Dock = DockStyle.Fill,
                Name = "panelContenedor"
            };
            this.Controls.Add(panelContenedor);
            panelContenedor.BringToFront();
        }
        private void MostrarFormularioEnPanel(Form formulario)
        {
            // Cerrar y liberar el formulario anterior si existe
            if (formularioActual != null)
            {
                formularioActual.Close();
                formularioActual.Dispose();
            }

            // Configurar el nuevo formulario para mostrarse como control
            formulario.TopLevel = false; // Importante: permite que el Form sea un control hijo
            formulario.FormBorderStyle = FormBorderStyle.None; // Sin bordes
            formulario.Dock = DockStyle.Fill; // Ocupa todo el panel

            // Limpiar el panel y agregar el formulario
            panelContenedor.Controls.Clear();
            panelContenedor.Controls.Add(formulario);

            // Mostrar el formulario
            formulario.Show();

            // Guardar referencia
            formularioActual = formulario;

            // Suscribir al idioma si implementa IObserver
            if (formulario is IObserver observer)
            {
                SuscribirFormularioAIdioma(observer);
            }
        }
        private void SuscribirFormularioAIdioma(IObserver observer)
        {
            var idioma = MultidiomaManager.GetIdioma();
            if (idioma != null)
            {
                if (idioma is Idioma idiomaConcreto)
                {
                    var idiomaCacheado = MultidiomaManager.ObtenerIdiomaCache(idiomaConcreto);
                    idiomaCacheado.Suscribir(observer);
                }
                else
                {
                    idioma.Suscribir(observer);
                }
            }
        }

        private void FrmMultidiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
