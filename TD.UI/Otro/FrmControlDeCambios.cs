using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmControlDeCambios : Form, IObserver
    {
        private readonly IUsuarioHistoricoServices _usuarioHistoricoServices;
        private readonly IUsuarioServices _usuarioServices;
        private readonly IBitacoraServices _bitacoraServices;
        private readonly IMultidiomaServices _multidiomaServices;

        public FrmControlDeCambios()
        {
            InitializeComponent();
            _usuarioHistoricoServices = ServicesFactory.CreateUsuarioHistoricoServices();
            _usuarioServices = ServicesFactory.CreateUsuarioServices();
            _bitacoraServices = ServicesFactory.CreateBitacoraServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
        }

        private void buscarCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();

            Point ptTitle = new Point(this.Width / 2 - 120, this.Height / 20);
            Point ptFiltros = new Point(this.Width / 16, this.Height / 8);
            Point ptDgv = new Point(this.Width / 40, this.Height / 4);
            Size szDgv = new Size(this.Width * 5 / 6, this.Height / 2 + this.Height / 8);
            Point ptBtnRestaurar = new Point(ptDgv.X + szDgv.Width + 10, ptDgv.Y + 15);
            Size szBtn = new Size(84, 28);

            FrmControlDeCambiosFlexibilizador.CrearVistaBusqueda(
                this,
                _usuarioHistoricoServices,
                _usuarioServices,
                _bitacoraServices,
                ptTitle, ptFiltros, ptDgv, szDgv, ptBtnRestaurar, szBtn);

            AplicarIdiomaActual();
        }

        private void FrmControlDeCambios_Load(object sender, EventArgs e)
        {
            buscarCambiosToolStripMenuItem_Click(null, null);
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
