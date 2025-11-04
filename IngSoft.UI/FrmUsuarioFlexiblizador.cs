using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Services;
using IngSoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    internal  class FrmUsuarioFlexiblizador
    {
        // Delegado para registrar en la bitácora
        internal static void RegistrarEnBitacora(Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
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
        internal static void ActualizarMenuSegunPermisosUsuario()
        {
            FlexibilizadorFormularios.MenuStripHider(FrmUsuario.ActiveForm.MainMenuStrip, SessionManager.GetPermisos() as PermisoComponent);
        }

        internal static void TextBoxCreator(string param, Point position)
        {
            // Delegar la creación de TextBox + Label a FlexibilizadorFormularios
            FlexibilizadorFormularios.CreateTextBox(FrmUsuario.ActiveForm, param, position);
        }
        internal static void CheckboxCreator(string param, Point position)
        {
            // Delegar la creación de Checkbox + Label a FlexibilizadorFormularios
            FlexibilizadorFormularios.CreateCheckBox(FrmUsuario.ActiveForm, param, position);
        }
        private static void btnGuardar_Click(object sender, EventArgs e)
        {
            // Lógica para manejar el evento de clic del botón Guardar Usuario
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            try
            {
                usuarioServices.GuardarUsuario(new Usuario
                {
                    IdUsuario = 0,
                    UserName = FrmUsuario.ActiveForm.Controls.Find("txtUsuario", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                    Nombre = FrmUsuario.ActiveForm.Controls.Find("txtNombre", true).FirstOrDefault() is TextBox txtNombre ? txtNombre.Text : string.Empty,
                    Apellido = FrmUsuario.ActiveForm.Controls.Find("txtApellido", true).FirstOrDefault() is TextBox txtApellido ? txtApellido.Text : string.Empty,
                    Email = FrmUsuario.ActiveForm.Controls.Find("txtEmail", true).FirstOrDefault() is TextBox txtEmail ? txtEmail.Text : string.Empty,
                    Contrasena = FrmUsuario.ActiveForm.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty
                });
                MessageBox.Show("Usuario guardado con éxito.");
                new FrmUsuario().EliminarControlesAdicionalesUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}");
            }
        }
        private static void btnModificar_Click(object sender, EventArgs e)
        {
            // Lógica para manejar el evento de clic del botón Modificar Usuario
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            try
            {
                /*usuarioServices.ModificarUsuario(new Usuario
                {
                    UserName = FrmUsuario.ActiveForm.Controls.Find("txtUserName", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                    Nombre = FrmUsuario.ActiveForm.Controls.Find("txtNombre", true).FirstOrDefault() is TextBox txtNombre ? txtNombre.Text : string.Empty,
                    Apellido = FrmUsuario.ActiveForm.Controls.Find("txtApellido", true).FirstOrDefault() is TextBox txtApellido ? txtApellido.Text : string.Empty,
                    Email = FrmUsuario.ActiveForm.Controls.Find("txtEmail", true).FirstOrDefault() is TextBox txtEmail ? txtEmail.Text : string.Empty,
                    Contrasena = FrmUsuario.ActiveForm.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty,
                    Bloqueado = FrmUsuario.ActiveForm.Controls.Find("chkBloqueado", true).FirstOrDefault() is CheckBox chkBloqueado ? chkBloqueado.Checked : false
                });*/
                MessageBox.Show("Usuario modificado con éxito.");
                new FrmUsuario().EliminarControlesAdicionalesUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el usuario: {ex.Message}");
            }
        }
        private static void BtnLogin_Click(object sender, EventArgs e)
        {
            IUsuarioServices usuarioServices = ServicesFactory.CreateUsuarioServices();
            // Inyecta el delegado al servicio
            usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);


            Usuario mUsuarioActual = new Usuario
            {
                UserName = FrmUsuario.ActiveForm.Controls.Find("txtUsuario", true).FirstOrDefault() is TextBox txtUsuario ? txtUsuario.Text : string.Empty,
                Contrasena = FrmUsuario.ActiveForm.Controls.Find("txtContraseña", true).FirstOrDefault() is TextBox txtContraseña ? txtContraseña.Text : string.Empty
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
        internal static void GuardarUsuarioButtonCreator()
        {
            var position = new Point((FrmUsuario.ActiveForm.Width / 2) - 100, (FrmUsuario.ActiveForm.Height / 2 + FrmUsuario.ActiveForm.Height / 4));
            FlexibilizadorFormularios.CreateButton(FrmUsuario.ActiveForm, "btnGuardarUsuario", position, new Size(200,30), "Guardar Usuario", btnGuardar_Click);
        }
        internal static void ModificarUsuarioButtonCreator(Point position)
        {
            FlexibilizadorFormularios.CreateButton(FrmUsuario.ActiveForm, "btnModificarUsuario", position, new Size(200,30), "Modificar Usuario", btnModificar_Click);
        }
        internal static void LoginButtonCreator()
        {
            var position = new Point((FrmUsuario.ActiveForm.Width / 2) - 100, (FrmUsuario.ActiveForm.Height / 2 + FrmUsuario.ActiveForm.Height / 4));
            FlexibilizadorFormularios.CreateButton(FrmUsuario.ActiveForm, "btnLogin", position, new Size(200,30), "Login", BtnLogin_Click);
        }
        internal static DataGridView DataGridViewUsuarioCreator(List<Usuario> pUsuarios, Point position, Size size)
        {

                
            Dictionary<string, Type> columnDefinitions = new Dictionary<string, Type>
            {
                { "Id", typeof(Guid) },
                { "UserName", typeof(string) },
                { "Email", typeof(string) },
                { "Nombre", typeof(string) },
                { "Apellido", typeof(string) },
                { "Bloqueado", typeof(bool) },
                { "CantidadIntentos", typeof(int) }
            };

            return FlexibilizadorFormularios.CreateDataGridView<Usuario>(FrmUsuario.ActiveForm, "dataGridViewUsuarios", position, size,columnDefinitions, pUsuarios);
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
                                Control[] controls = FrmUsuario.ActiveForm.Controls.Find(checkBoxName, true);
                                if (controls.Length > 0 && controls[0] is CheckBox checkBox)
                                {
                                    checkBox.Checked = Convert.ToBoolean(row[column]);
                                }
                            }
                            else
                            {
                                string textBoxName = $"txt{column.ColumnName}";
                                Control[] controls = FrmUsuario.ActiveForm.Controls.Find(textBoxName, true);
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

    }
}
