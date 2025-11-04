using System;
using System.Linq;
using System.Windows.Forms;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.Multidioma;

namespace IngSoft.UI
{
    public partial class FrmPrincipal : Form, IObserver
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IMultidiomaServices _multidiomaServices;
        public FrmPrincipal()
        {
            InitializeComponent();
            _usuarioServices = ServicesFactory.CreateUsuarioServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            ActualizarMenuSegunEstadoSesion();
            CargarIdiomas();
            EstablecerIdiomaPorDefecto();
        }
        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmUsuario = new FrmUsuario();
            frmUsuario.ShowDialog();
        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmBitacora = new FrmBitacora();
            frmBitacora.ShowDialog();
        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmLogin = new FrmLogin();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                ActualizarMenuSegunEstadoSesion();
            }
        }
        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usuarioServices.SetRegistradoBitacora(FrmUsuarioFlexiblizador.RegistrarEnBitacora);
            _usuarioServices.LogOutUser();
            ActualizarMenuSegunEstadoSesion();
        }

        private void ActualizarMenuSegunEstadoSesion()
        {
            if (SessionManager.GetInstance().IsLoggedIn())
            {
                iniciarSesionToolStripMenuItem.Enabled = false;
                cerrarSesionToolStripMenuItem.Enabled = true;
                usuariosToolStripMenuItem.Visible = true;
                bitacoraToolStripMenuItem.Visible = true;
                controlDeCambiosToolStripMenuItem.Visible = true;
                multidiomasToolStripMenuItem.Visible = true;
                label1.Visible = true;
                label1.Text = $"Bienvenido: {SessionManager.GetUsuario().Nombre} {SessionManager.GetUsuario().Apellido}";
            }
            else
            {
                iniciarSesionToolStripMenuItem.Enabled = true;
                cerrarSesionToolStripMenuItem.Enabled = false;
                usuariosToolStripMenuItem.Visible = false;
                bitacoraToolStripMenuItem.Visible = false;
                controlDeCambiosToolStripMenuItem.Visible = false;
                multidiomasToolStripMenuItem.Visible = false;
                label1.Visible = false;
            }
        }

        private void menuPrincipal_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void controlDeCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var FrmControlCambios = new FrmControlDeCambios();
            FrmControlCambios.ShowDialog();
        }
        private void multidiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var frmMultidioma = new FrmMultidiomas();
            if ( frmMultidioma.ShowDialog() == DialogResult.OK )
            {
                CargarIdiomas();
            }
        }
        private void cboIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboIdiomas.SelectedItem is Idioma))
                return;

            var idiomaSeleccionado = MultidiomaManager.ObtenerIdiomaCache((Idioma)cboIdiomas.SelectedItem);

            if (MultidiomaManager.GetIdioma() != null) 
            {
                MultidiomaManager.GetIdioma().Desuscribir(this);
                
            }

            MultidiomaManager.SetIdioma(idiomaSeleccionado);

            idiomaSeleccionado.Suscribir(this);

            idiomaSeleccionado.NotificarObservers();
        }

        private void CargarIdiomas()
        {
            var idiomas = _multidiomaServices.ObtenerIdiomas()
                .Cast<IIdioma>().ToList();

            var idiomasCacheados = MultidiomaManager.ObtenerIdiomasCache(idiomas);

            cboIdiomas.DataSource = idiomasCacheados;
            cboIdiomas.DisplayMember = "Codigo";
            cboIdiomas.ValueMember = null;
        }
        private void EstablecerIdiomaPorDefecto()
        {
            var idiomaPorDefecto = (IIdioma)_multidiomaServices.ObtenerIdiomaPorDefecto();
            if (idiomaPorDefecto == null) return;

            idiomaPorDefecto = MultidiomaManager.ObtenerIdiomaCache(idiomaPorDefecto);

            cboIdiomas.SelectedItem = idiomaPorDefecto;
            MultidiomaManager.SetIdioma(idiomaPorDefecto);
            idiomaPorDefecto.Suscribir(this);

            var controles = _multidiomaServices.ObtenerControlesPorIdioma(idiomaPorDefecto.Id)
                .Cast<IControlIdioma>().ToList();
            MultidiomaManager.CambiarIdiomaControles(this, controles);
        }

        public void Actualizar()
        {
            if(MultidiomaManager.GetIdioma() != null) 
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }

    }
}
