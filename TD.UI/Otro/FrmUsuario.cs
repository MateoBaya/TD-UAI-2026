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
        private readonly IUsuarioServices _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<IUsuarioServices>();
        private readonly IMultidiomaServices _multidiomaServices;

        private List<Usuario> _usuarios;
        public FrmUsuario()
        {
            InitializeComponent();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();
        }

        private void agregarNuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            CrearControlesAgregarUsuario();
            AplicarIdiomaActual();
        }

        private void CrearControlesAgregarUsuario()
        {
            FrmUsuarioFlexiblizador.TextBoxCreator("UserName", new Point(FrmUsuario.ActiveForm.Width / 2 - 230, FrmUsuario.ActiveForm.Height / 8));
            FrmUsuarioFlexiblizador.TextBoxCreator("Nombre", new Point((FrmUsuario.ActiveForm.Width / 2 - 280) + (280), FrmUsuario.ActiveForm.Height / 8));
            FrmUsuarioFlexiblizador.TextBoxCreator("Apellido", new Point((FrmUsuario.ActiveForm.Width / 2 - 230), FrmUsuario.ActiveForm.Height / 4));
            FrmUsuarioFlexiblizador.TextBoxCreator("Email", new Point((FrmUsuario.ActiveForm.Width / 2 - 280) + (280), FrmUsuario.ActiveForm.Height / 4));
            FrmUsuarioFlexiblizador.TextBoxCreator("Contraseña", new Point((FrmUsuario.ActiveForm.Width / 2 - 230), (FrmUsuario.ActiveForm.Height / 8 + FrmUsuario.ActiveForm.Height / 4)));
            FrmUsuarioFlexiblizador.GuardarUsuarioButtonCreator();
        }
        private void CrearControlesModificarUsuario()
        {
            _usuarios = _usuarioServices.ObtenerUsuarios();
            Point position = new Point(FrmUsuario.ActiveForm.Width / 16, FrmUsuario.ActiveForm.Height / 8);
            Size size = new Size(FrmUsuario.ActiveForm.Width / 2 - FrmUsuario.ActiveForm.Width / 16, FrmUsuario.ActiveForm.Height / 2 + FrmUsuario.ActiveForm.Height / 4);
            new FrmUsuarioFlexiblizador().dataGridViewWithSelectionChanged(_usuarios, position, size);
            Point pointInicial = new Point(FrmUsuario.ActiveForm.Width / 2 + 20, FrmUsuario.ActiveForm.Height / 16);
            FrmUsuarioFlexiblizador.TextBoxCreator("UserName", new Point(pointInicial.X, pointInicial.Y + FrmUsuario.ActiveForm.Height / 10));
            FrmUsuarioFlexiblizador.TextBoxCreator("Nombre", new Point(pointInicial.X, pointInicial.Y + (FrmUsuario.ActiveForm.Height / 10) * 2));
            FrmUsuarioFlexiblizador.TextBoxCreator("Apellido", new Point(pointInicial.X, pointInicial.Y + (FrmUsuario.ActiveForm.Height / 10) * 3));
            FrmUsuarioFlexiblizador.TextBoxCreator("Email", new Point(pointInicial.X, pointInicial.Y + (FrmUsuario.ActiveForm.Height / 10) * 4));
            FrmUsuarioFlexiblizador.CheckboxCreator("Bloqueado", new Point(pointInicial.X, pointInicial.Y + (FrmUsuario.ActiveForm.Height / 10) * 6));
            FrmUsuarioFlexiblizador.ModificarUsuarioButtonCreator(new Point(pointInicial.X, pointInicial.Y + (FrmUsuario.ActiveForm.Height / 10) * 7));
        }

        private void verTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            _usuarios = _usuarioServices.ObtenerUsuarios();
            Point position = new Point(FrmUsuario.ActiveForm.Width / 8, FrmUsuario.ActiveForm.Height / 8);
            Size size = new Size(FrmUsuario.ActiveForm.Width / 2 + FrmUsuario.ActiveForm.Width / 4, FrmUsuario.ActiveForm.Height / 2 + FrmUsuario.ActiveForm.Height / 4);
            FrmUsuarioFlexiblizador.DataGridViewUsuarioCreator(_usuarios,position,size);
            AplicarIdiomaActual();
        }


        internal void EliminarControlesAdicionalesUsuario()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(FrmUsuario.ActiveForm);
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

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            FrmUsuarioFlexiblizador.TextBoxCreator("UserName", new Point(FrmUsuario.ActiveForm.Width / 2 - 100, FrmUsuario.ActiveForm.Height / 4));
            FrmUsuarioFlexiblizador.TextBoxCreator("Contraseña", new Point((FrmUsuario.ActiveForm.Width / 2 - 100), FrmUsuario.ActiveForm.Height / 8 + FrmUsuario.ActiveForm.Height / 4));
            try
            {
                FrmUsuarioFlexiblizador.LoginButtonCreator();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _usuarioServices.LogOutUser();
            EliminarControlesAdicionalesUsuario();
        }

        private void FrmUsuario_Shown(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            FrmUsuarioFlexiblizador.ActualizarMenuSegunPermisosUsuario();
        }

        private void FrmUsuario_Load(object sender, EventArgs e)
        {
            SuscribirseAIdiomaActual();
            AplicarIdiomaActual();
        }

        private void modificarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            CrearControlesModificarUsuario();
            AplicarIdiomaActual();
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
        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id);
                MultidiomaManager.CambiarIdiomaControles(this, controles.Cast<IControlIdioma>().ToList());
            }
        }

        private void eliminarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point dgvPosition = new Point(this.Size.Width / 16, this.Size.Height / 16);
            Size listSize = new Size(this.Size.Width / 2-this.Size.Width/16, this.Size.Height - this.Height/6);
            Point txtPosition = new Point(this.Size.Width / 2+this.Size.Width/6, this.Size.Height / 4);
            Point buttonPosition = new Point(txtPosition.X,txtPosition.Y+50);
            EliminarControlesAdicionalesUsuario();
            FrmUsuarioFlexiblizador.EliminarUsuarioCreator(dgvPosition, listSize, txtPosition, buttonPosition);
        }
    }
}
