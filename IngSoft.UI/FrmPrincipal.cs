using System;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Services;
using IngSoft.Services.Encriptadores;

namespace IngSoft.UI
{
    public partial class FrmPrincipal : Form
    {
        private readonly IUsuarioServices _usuarioServices;
        public FrmPrincipal()
        {
            InitializeComponent();
            _usuarioServices = ServicesFactory.CreateUsuarioServices();
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            ActualizarMenuSegunEstadoSesion();
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
    }
}
