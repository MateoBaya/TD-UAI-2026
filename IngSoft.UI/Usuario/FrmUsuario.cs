using System;
using System.Collections.Generic;
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
    public partial class FrmUsuario : Form, IObserver
    {


        public FrmUsuario()
        {
            InitializeComponent();
       }

        private void agregarNuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //EliminarControlesAdicionalesUsuario();
            //CrearControlesAgregarUsuario();
            //AplicarIdiomaActual();
        }

        private void CrearControlesAgregarUsuario()
        {

        }
        private void CrearControlesModificarUsuario()
        {

        }

        private void verTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        internal void EliminarControlesAdicionalesUsuario()
        {
            //FlexibilizadorFormularios.EliminarControlesAdicionalesForm(FrmUsuario.ActiveForm);
        }


        //private static void ActualizarBotoneraSesion(ToolStripItemCollection botonera)
        //{
        //    ToolStripItem loginToolStripButton = botonera["loginToolStripMenuItem"];//botonera.Find("loginToolStripMenuItem", true).FirstOrDefault();
        //    ToolStripItem logoutToolStripButton = botonera["cerrarSesionToolStripMenuItem"]; ;//botonera.Find("cerrarSesionToolStripMenuItem", true).FirstOrDefault();
        //    ToolStripItem agregarUsuarioToolStripButton = botonera["agregarNuevoToolStripMenuItem"];
        //    ToolStripItem verTodosToolStripButton = botonera["verTodosToolStripMenuItem"];
        //    if (SessionManager.GetInstance().IsLoggedIn())
        //    {
        //        //loginToolStripButton.Visible = false;
        //        //logoutToolStripButton.Visible = true;
        //        agregarUsuarioToolStripButton.Visible = true;
        //        verTodosToolStripButton.Visible = true;
        //    }
        //    else
        //    {
        //        //loginToolStripButton.Visible = true;
        //        //logoutToolStripButton.Visible = false;
        //        agregarUsuarioToolStripButton.Visible = false;
        //        verTodosToolStripButton.Visible = false;
        //    }
        //}

        //private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    EliminarControlesAdicionalesUsuario();
        //    FrmUsuarioFlexiblizador.TextBoxCreator("UserName", new Point(FrmUsuario.ActiveForm.Width / 2 - 100, FrmUsuario.ActiveForm.Height / 4));
        //    FrmUsuarioFlexiblizador.TextBoxCreator("Contraseña", new Point((FrmUsuario.ActiveForm.Width / 2 - 100), FrmUsuario.ActiveForm.Height / 8 + FrmUsuario.ActiveForm.Height / 4));
        //    try
        //    {
        //        FrmUsuarioFlexiblizador.LoginButtonCreator();
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    _usuarioServices.LogOutUser();
        //    EliminarControlesAdicionalesUsuario();
        //}

        //private void FrmUsuario_Shown(object sender, EventArgs e)
        //{
        //    EliminarControlesAdicionalesUsuario();
        //    FrmUsuarioFlexiblizador.ActualizarMenuSegunPermisosUsuario();
        //}

        //private void FrmUsuario_Load(object sender, EventArgs e)
        //{
        //    SuscribirseAIdiomaActual();
        //    AplicarIdiomaActual();
        //}

        //private void modificarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    EliminarControlesAdicionalesUsuario();
        //    CrearControlesModificarUsuario();
        //    AplicarIdiomaActual();
        //}

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

        public void Actualizar()
        {
            //    if (MultidiomaManager.GetIdioma() != null)
            //    {
            //        var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
            //        MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            //    }
        }

        //private void eliminarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Point dgvPosition = new Point(this.Size.Width / 16, this.Size.Height / 16);
        //    Size listSize = new Size(this.Size.Width / 2-this.Size.Width/16, this.Size.Height - this.Height/6);
        //    Point txtPosition = new Point(this.Size.Width / 2+this.Size.Width/6, this.Size.Height / 4);
        //    Point buttonPosition = new Point(txtPosition.X,txtPosition.Y+50);
        //    EliminarControlesAdicionalesUsuario();
        //    FrmUsuarioFlexiblizador.EliminarUsuarioCreator(dgvPosition, listSize, txtPosition, buttonPosition);
        //}
    }
}
