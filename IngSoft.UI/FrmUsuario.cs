using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;


namespace IngSoft.UI
{
    public partial class FrmUsuario : Form
    {
        private readonly IUsuarioServices _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<IUsuarioServices>();
        private List<Usuario> _usuarios;
        public FrmUsuario()
        {
            InitializeComponent();
        }

        private void agregarNuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            CrearControlesAgregarUsuario();
        }

        private void CrearControlesAgregarUsuario()
        {
            FrmUsuarioFlexiblizador.TextBoxCreator("Usuario", new Point(FrmUsuario.ActiveForm.Width / 2 - 230, FrmUsuario.ActiveForm.Height / 8));
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
        }


        internal void EliminarControlesAdicionalesUsuario()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this);
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
            FrmUsuarioFlexiblizador.TextBoxCreator("Usuario", new Point(FrmUsuario.ActiveForm.Width / 2 - 100, FrmUsuario.ActiveForm.Height / 4));
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
        }

        private void modificarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliminarControlesAdicionalesUsuario();
            CrearControlesModificarUsuario();
        }
    }
}
