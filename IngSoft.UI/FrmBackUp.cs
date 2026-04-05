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
    public partial class FrmBackUp : Form, IObserver
    {
        private readonly IMultidiomaServices _multidiomaServices;

        public FrmBackUp()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
        }

        private void FrmBackUp_Shown(object sender, EventArgs e)
        {
            FlexibilizadorFormularios.MenuStripHider(this.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
            gestionarBackupToolStripMenuItem_Click(null, null);
        }

        private void gestionarBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionales();

            Point ptTitle = new Point(this.Width / 2 - 110, this.Height / 14);
            Point ptDgv = new Point(this.Width / 16, this.Height / 5);
            Size szDgv = new Size(this.Width * 3 / 4, this.Height * 3 / 4 - this.Height / 5);
            Point ptBtnCrear = new Point(ptDgv.X + szDgv.Width + 20, ptDgv.Y);
            Point ptBtnRestaurar = new Point(ptBtnCrear.X, ptBtnCrear.Y + 60);
            Size szBtn = new Size(123, 40);

            FrmBackUpFlexibilizador.CrearVistaBackup(ptTitle, ptDgv, szDgv, ptBtnCrear, ptBtnRestaurar, szBtn);
            AplicarIdiomaActual();
        }

        private void FrmBackUp_Load(object sender, EventArgs e)
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
    }
}
