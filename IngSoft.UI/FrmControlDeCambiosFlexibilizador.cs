using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI
{
    /// <summary>
    /// Instance-based Flexibilizador for Control de Cambios.
    /// Faithfully reproduces FrmControlDeCambios logic inside FrmPrincipal.GetPanelMain.
    ///
    /// Services used:
    ///   IUsuarioHistoricoServices  → ObtenerUsuarioHistorico(userName)
    ///   IUsuarioServices           → ModificarUsuario(usuario) + SetRegistradoBitacora
    ///   IBitacoraServices          → GuardarBitacora(bitacora)
    /// </summary>
    internal class FrmControlDeCambiosFlexibilizador
    {
        private readonly FrmPrincipal _form = SingleInstancesManager.Instance.ObtenerInstancia<FrmPrincipal>();
        private readonly IUsuarioHistoricoServices _usuarioHistoricoServices =
            ServicesFactory.CreateUsuarioHistoricoServices();
        private readonly IUsuarioServices _usuarioServices =
            ServicesFactory.CreateUsuarioServices();
        private readonly IBitacoraServices _bitacoraServices =
            ServicesFactory.CreateBitacoraServices();

        internal void EliminarControlesAdicionales()
        {
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(
                _form.GetPanelMain, _form.ControlesSalvar());
        }

        // ── Control creators ────────────────────────────────────────────────────

        /// <summary>
        /// Creates the search filter row: entity ComboBox, username TextBox, Buscar button.
        /// Mirrors the designer controls cboEntidades / txtUserName / btnBuscar.
        /// </summary>
        internal void FiltrosBusquedaCreator(Point position)
        {
            var lblEntidad = new Label
            {
                Name     = "lblEntidad",
                Text     = "Entidad:",
                Location = position,
                AutoSize = true
            };
            var cboEntidades = new ComboBox
            {
                Name          = "cboEntidades",
                Location      = new Point(position.X + 55, position.Y - 3),
                Width         = 130,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Populate with tracked entities. Extend as new historico services are added.
            cboEntidades.Items.Add("Usuario");
            cboEntidades.SelectedIndex = 0;

            var lblUserName = new Label
            {
                Name     = "lblUserNameBuscar",
                Text     = "Usuario:",
                Location = new Point(position.X + 200, position.Y),
                AutoSize = true
            };
            var txtUserName = new TextBox
            {
                Name     = "txtUserName",
                Location = new Point(position.X + 260, position.Y - 3),
                Width    = 150
            };
            var btnBuscar = new Button
            {
                Name     = "btnBuscarCambios",
                Text     = "Buscar",
                Location = new Point(position.X + 425, position.Y - 3),
                Size     = new Size(80, 24)
            };
            btnBuscar.Click += BtnBuscar_Click;

            foreach (Control c in new Control[] { lblEntidad, cboEntidades, lblUserName, txtUserName, btnBuscar })
                _form.GetPanelMain.Controls.Add(c);
        }

        /// <summary>
        /// Creates the UsuarioHistorico DataGridView with the same column configuration
        /// as ActualizarGridView in FrmControlDeCambios: renamed headers, Bloqueado as
        /// "Sí/No" text, hidden fields (Id, IdUsuario, TipoOperacion) simply excluded.
        /// </summary>
        internal DataGridView DataGridViewCambiosCreator(Point position, Size size)
        {
            // Only define the visible columns. Hidden fields are omitted entirely.
            var cols = new Dictionary<string, Type>
            {
                { "UserName",           typeof(string) },
                { "Nombre",             typeof(string) },
                { "Apellido",           typeof(string) },
                { "Email",              typeof(string) },
                { "Bloqueado",          typeof(bool) },
                { "CantidadIntentos",   typeof(int) },
                { "FechaModificacion",  typeof(DateTime) },
                { "UsuarioModificador", typeof(string) }
            };

            DataGridView dgv = FlexibilizadorFormularios.CreateDataGridView<UsuarioHistorico>(
                _form.GetPanelMain, "dgvControlDeCambios", position, size, cols,
                new List<UsuarioHistorico>());

            // Mirror post-binding configuration from ActualizarGridView
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect         = false;
            dgv.ReadOnly            = true;

            // Rename headers to match the original form
            if (dgv.Columns["UserName"]           != null) dgv.Columns["UserName"].HeaderText           = "Usuario";
            if (dgv.Columns["CantidadIntentos"]   != null) dgv.Columns["CantidadIntentos"].HeaderText   = "Intentos";
            if (dgv.Columns["FechaModificacion"]  != null) dgv.Columns["FechaModificacion"].HeaderText  = "Fecha";
            if (dgv.Columns["UsuarioModificador"] != null) dgv.Columns["UsuarioModificador"].HeaderText = "Modificado Por";

            // Bool → "Sí"/"No" for Bloqueado (mirrors DgvControlCambios_CellFormatting)
            dgv.CellFormatting += DgvControlCambios_CellFormatting;

            // Enable Restaurar button only when a row is selected (mirrors dgvControlCambios_SelectionChanged)
            dgv.SelectionChanged += DgvControlCambios_SelectionChanged;

            return dgv;
        }

        /// <summary>Restaurar button — disabled on creation; SelectionChanged enables it.</summary>
        internal Button RestaurarButtonCreator(Point position, Size size)
        {
            var btn = FlexibilizadorFormularios.CreateButton(
                _form.GetPanelMain, "btnRestaurarCambio", position, size,
                "Restaurar", BtnRestaurar_Click);

            btn.Enabled = false;
            return btn;
        }

        // Simple title creator used by HeaderConfig
        internal void TituloCreator(string title, Point position)
        {
            var lbl = new Label
            {
                Name = "lblTituloControlDeCambios",
                Text = title,
                Location = position,
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            var existing = _form.GetPanelMain.Controls.Find(lbl.Name, true).FirstOrDefault() as Control;
            if (existing != null)
                _form.GetPanelMain.Controls.Remove(existing);
            _form.GetPanelMain.Controls.Add(lbl);
        }

        // Alias to the existing filtros creator to match previous API
        internal void FiltrosFechaCreator(Point position)
        {
            FiltrosBusquedaCreator(position);
        }

        // ── Event handlers ───────────────────────────────────────────────────────

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            var cbo = _form.GetPanelMain.Controls.Find("cboEntidades", true).FirstOrDefault() as ComboBox;
            var txt = _form.GetPanelMain.Controls.Find("txtUserName",  true).FirstOrDefault() as TextBox;

            if (cbo?.SelectedItem is null)
            {
                MessageBox.Show("Seleccione una entidad para buscar.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txt?.Text))
            {
                MessageBox.Show("Ingrese un nombre de usuario para buscar.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ActualizarGridView(txt.Text);
        }

        private void BtnRestaurar_Click(object sender, EventArgs e)
        {
            // Wire bitácora delegate before the service call (mirrors btnRestaurar_Click)
            _usuarioServices.SetRegistradoBitacora(RegistrarEnBitacora);

            var usuarioHistorico = ObtenerUsuarioHistoricoSeleccionado();
            if (usuarioHistorico == null)
            {
                MessageBox.Show("Seleccione un registro para restaurar.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Exact replica of the Usuario mapping in the original form
            var usuarioARevertir = new Usuario
            {
                IdUsuario        = usuarioHistorico.IdUsuario,
                Nombre           = usuarioHistorico.Nombre,
                Apellido         = usuarioHistorico.Apellido,
                Email            = usuarioHistorico.Email,
                UserName         = usuarioHistorico.UserName,
                Bloqueado        = usuarioHistorico.Bloqueado,
                CantidadIntentos = usuarioHistorico.CantidadIntentos,
                FechaEliminado   = usuarioHistorico.FechaEliminado
            };

            try
            {
                _usuarioServices.ModificarUsuario(usuarioARevertir);
                MessageBox.Show("Cambio restaurado exitosamente.");

                // Refresh with same username (mirrors ActualizarGridView call after restore)
                var txt = _form.GetPanelMain.Controls
                               .Find("txtUserName", true).FirstOrDefault() as TextBox;
                if (txt != null && !string.IsNullOrEmpty(txt.Text))
                    ActualizarGridView(txt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restaurar el cambio: {ex.Message}");
            }
        }

        private void DgvControlCambios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null || e.ColumnIndex < 0) return;

            if (dgv.Columns[e.ColumnIndex].Name == "Bloqueado" && e.Value != null)
            {
                if (e.Value is bool bloqueado)
                {
                    e.Value             = bloqueado ? "Sí" : "No";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvControlCambios_SelectionChanged(object sender, EventArgs e)
        {
            var dgv = sender as DataGridView;
            var btn = _form.GetPanelMain.Controls
                           .Find("btnRestaurarCambio", true).FirstOrDefault() as Button;
            if (btn != null && dgv != null)
                btn.Enabled = dgv.SelectedRows.Count > 0;
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>Fetches and binds the user history. Mirrors ActualizarGridView.</summary>
        private void ActualizarGridView(string userName)
        {
            var dgv = _form.GetPanelMain.Controls
                           .Find("dgvControlDeCambios", true).FirstOrDefault() as DataGridView;
            if (dgv == null) return;

            try
            {
                dgv.DataSource = _usuarioHistoricoServices.ObtenerUsuarioHistorico(userName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener historial: {ex.Message}");
            }
        }

        private UsuarioHistorico ObtenerUsuarioHistoricoSeleccionado()
        {
            var dgv = _form.GetPanelMain.Controls
                           .Find("dgvControlDeCambios", true).FirstOrDefault() as DataGridView;

            return dgv != null && dgv.SelectedRows.Count > 0
                ? dgv.SelectedRows[0].DataBoundItem as UsuarioHistorico
                : null;
        }

        private void RegistrarEnBitacora(Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
        {
            _bitacoraServices.GuardarBitacora(new Bitacora
            {
                Id          = Guid.NewGuid(),
                Usuario     = usuario,
                Fecha       = DateTime.Now,
                Descripcion = descripcion,
                Origen      = origen,
                TipoEvento  = tipoEvento
            });
        }
    }
}
