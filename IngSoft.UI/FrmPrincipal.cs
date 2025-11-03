using System;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Services;

namespace IngSoft.UI
{
    public partial class FrmPrincipal : Form
    {
        private readonly IUsuarioServices _usuarioServices;
        public FrmPrincipal()
        {
            InitializeComponent();
            _usuarioServices = ServicesFactory.CreateUsuarioServices();
            SingleInstancesManager.Instance.AgregarObjeto(_usuarioServices);
            SingleInstancesManager.Instance.AgregarObjeto(ServicesFactory.CreateBitacoraServices());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
                FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
            }
        }
        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogOutUser();
            FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
        }

        private void LogOutUser()
        {
            _usuarioServices.LogOutUser();
        }



        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SessionManager.GetInstance().IsLoggedIn())
            {
                LogOutUser();
            }
        }

        private void permisosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPermiso frmPermiso = new FrmPermiso();
            frmPermiso.ShowDialog();
        }

        private void FrmPrincipal_Shown(object sender, EventArgs e)
        {
            FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
        }
    }
}
