using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.ApplicationServices.Implementation;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace IngSoft.UI
{
    internal  class FrmUsuarioFlexiblizador
    {
        // Delegado para registrar en la bitácora
        private List<Usuario> _usuarios;
        private FrmPrincipal form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly IUsuarioServices _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<IUsuarioServices>();
        private readonly IMultidiomaServices _multidiomaServices = ServicesFactory.CreateMultidiomaServices();

        public List<Usuario> Usuarios { get => _usuarios; set => _usuarios = value; }

        public IUsuarioServices UsuarioServices => _usuarioServices;

        public IMultidiomaServices MultidiomaServices => _multidiomaServices;

        internal   void RegistrarEnBitacora(Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
        {
            IBitacoraServices bitacoraServices = SingleInstancesManager.Instance.ObtenerInstancia<IBitacoraServices>();
            var bitacora = new Bitacora
            {
                Id = Guid.NewGuid(),
                Usuario = usuario,
                Fecha = DateTime.Now,
                Descripcion = descripcion,
                Origen = origen,
                TipoEvento = tipoEvento
            };
            bitacoraServices.GuardarBitacora(bitacora);
        }
        internal   void ActualizarMenuSegunPermisosUsuario()
        {
            FlexibilizadorFormularios.MenuStripHider(form.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
        }

        internal   void TextBoxCreator(string param, Point position)
        {
            // Delegar la creación de TextBox + Label a FlexibilizadorFormularios
            FlexibilizadorFormularios.CreateTextBox(form.GetPanelMain, param, position);
        }
        internal   void CheckboxCreator(string param, Point position)
        {
            // Delegar la creación de Checkbox + Label a FlexibilizadorFormularios
            FlexibilizadorFormularios.CreateCheckBox(form.GetPanelMain, param, position);
        }
        private   void btnGuardar_Click(object sender, EventArgs e)
        {
            // Lógica para manejar el evento de clic del botón Guardar Usuario
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            try
            {
                usuarioServices.CrearUsuario(new Usuario
                {
                    IdUsuario = 0,
                    UserName = form.GetPanelMain.Controls.Find("txtUserName", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                    Nombre = form.GetPanelMain.Controls.Find("txtNombre", true).FirstOrDefault() is TextBox txtNombre ? txtNombre.Text : string.Empty,
                    Apellido = form.GetPanelMain.Controls.Find("txtApellido", true).FirstOrDefault() is TextBox txtApellido ? txtApellido.Text : string.Empty,
                    Email = form.GetPanelMain.Controls.Find("txtEmail", true).FirstOrDefault() is TextBox txtEmail ? txtEmail.Text : string.Empty,
                    Contrasena = form.GetPanelMain.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty
                });
                MessageBox.Show("Usuario guardado con éxito.");
                EliminarControlesAdicionalesUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}");
            }
        }
        private   void btnModificar_Click(object sender, EventArgs e)
        {
            // Lógica para manejar el evento de clic del botón Modificar Usuario
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            try
            {
                usuarioServices.ModificarUsuario(new Usuario
                {
                    UserName = form.GetPanelMain.Controls.Find("txtUserName", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                    Nombre = form.GetPanelMain.Controls.Find("txtNombre", true).FirstOrDefault() is TextBox txtNombre ? txtNombre.Text : string.Empty,
                    Apellido = form.GetPanelMain.Controls.Find("txtApellido", true).FirstOrDefault() is TextBox txtApellido ? txtApellido.Text : string.Empty,
                    Email = form.GetPanelMain.Controls.Find("txtEmail", true).FirstOrDefault() is TextBox txtEmail ? txtEmail.Text : string.Empty,
                    Contrasena = form.GetPanelMain.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty,
                    Bloqueado = form.GetPanelMain.Controls.Find("chkBloqueado", true).FirstOrDefault() is CheckBox chkBloqueado ? chkBloqueado.Checked : false
                });
                MessageBox.Show("Usuario modificado con éxito.");
                new FrmUsuario().EliminarControlesAdicionalesUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el usuario: {ex.Message}");
            }
        }
        private   void BtnLogin_Click(object sender, EventArgs e)
        {
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            // Inyecta el delegado al servicio
            usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);


            Usuario mUsuarioActual = new Usuario
            {
                UserName = form.GetPanelMain.Controls.Find("txtUserName", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                Contrasena = form.GetPanelMain.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty
            };
            try
            {
                usuarioServices.LoginUser(mUsuarioActual);
                MessageBox.Show($"Iniciado sesion: {SessionManager.GetUsuario().UserName}");
                new FrmUsuario().EliminarControlesAdicionalesUsuario();
            }
            catch(UnauthorizedAccessException UnAcExc)
            {
                MessageBox.Show($"Error de autenticación: {UnAcExc.Message}");
            }
            catch (Exception ex) {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }
        internal   void GuardarUsuarioButtonCreator()
        {
            var position = new Point((form.GetPanelMain.Width / 2) - 100, (form.GetPanelMain.Height / 2 + form.GetPanelMain.Height / 4));
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnGuardarUsuario", position, new Size(200,30), "Guardar Usuario", btnGuardar_Click);
        }
        internal   void ModificarUsuarioButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnModificarUsuario", position, new Size(200,30), "Modificar Usuario", btnModificar_Click);
        }
        internal   void LoginButtonCreator()
        {
            var position = new Point((form.GetPanelMain.Width / 2) - 100, (form.GetPanelMain.Height / 2 + form.GetPanelMain.Height / 4));
            FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnLogin", position, new Size(200,30), "Login", BtnLogin_Click);
        }
        internal   DataGridView DataGridViewUsuarioCreator(List<Usuario> pUsuarios, Point position, Size size)
        {

                
            Dictionary<string, Type> columnDefinitions = new Dictionary<string, Type>
            {
                { "UserName", typeof(string) },
                { "Email", typeof(string) },
                { "Nombre", typeof(string) },
                { "Apellido", typeof(string) },
                { "Bloqueado", typeof(bool) },
                { "CantidadIntentos", typeof(int) },
                { "FechaEliminado", typeof(DateTime) }
            };

            return FlexibilizadorFormularios.CreateDataGridView<Usuario>(form.GetPanelMain, "dataGridViewUsuarios", position, size,columnDefinitions, pUsuarios);
        }
        public DataGridView dataGridViewWithSelectionChanged(List<Usuario> pUsuario,Point position, Size size)
        {
            DataGridView dgv = DataGridViewUsuarioCreator(pUsuario, position, size);
            dgv.SelectionChanged += DataGridViewUsuario_SelectionChanged;
            dgv.Rows[0].Selected = false; // Seleccionar la primera fila por defecto
            return dgv;
        }
        private void DataGridViewUsuario_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            FillControlsFromDataRowSelectedEvent(dgv, "txt");
        }

        private void FillControlsFromDataRowSelectedEvent(DataGridView dgv, string textBoxPrefix)
        {
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.CurrentRow != null)
                {
                    DataRowView dataRowView = dgv.CurrentRow.DataBoundItem as DataRowView;
                    if (dataRowView != null)
                    {
                        DataRow row = dataRowView.Row;
                        foreach (DataColumn column in row.Table.Columns)
                        {
                            if (column.DataType == typeof(bool))
                            {
                                string checkBoxName = $"chk{column.ColumnName}";
                                Control[] controls = form.GetPanelMain.Controls.Find(checkBoxName, true);
                                if (controls.Length > 0 && controls[0] is CheckBox checkBox)
                                {
                                    checkBox.Checked = Convert.ToBoolean(row[column]);
                                }
                            }
                            else
                            {
                                string textBoxName = $"txt{column.ColumnName}";
                                Control[] controls = form.GetPanelMain.Controls.Find(textBoxName, true);
                                if (controls.Length > 0 && controls[0] is TextBox textBox)
                                {
                                    textBox.Text = row[column].ToString();
                                }
                            }
                        }
                    }
                }
            };
        }
        internal void EliminarUsuarioButtonCreator(Point listPos, Size listSize, Point txtPos, Point btnPos)
        {
            Button btnEliminador = FlexibilizadorFormularios.CreateButton(form.GetPanelMain, "btnEliminarUsuario", btnPos, new Size(200, 30), "Eliminar Usuario", btnEliminar_Click);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var txt = form.GetPanelMain.Controls.Find("txtUserName", true).FirstOrDefault() as TextBox;
            if (txt == null) return;
            string username = txt.Text?.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Ingrese el UserName del usuario a eliminar o selecciónelo de la lista.");
                return;
            }

            try
            {
                IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
                usuarioServices.EliminarUsuario(new Usuario { UserName = username });
                MessageBox.Show($"Usuario '{username}' eliminado correctamente.");

                // clear textbox
                if (txt != null) txt.Text = string.Empty;

                EliminarControlesAdicionalesUsuario();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}");
            }

            return;
        }
        internal void EliminarControlesAdicionalesUsuario()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(form.GetPanelMain, form.ControlesSalvar());
        }
    }
}
