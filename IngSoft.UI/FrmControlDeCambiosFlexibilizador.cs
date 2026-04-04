using System;
using System.Drawing;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.Domain;
using IngSoft.Domain.Enums;

namespace IngSoft.UI
{
    internal static class FrmControlDeCambiosFlexibilizador
    {
        public static void CrearVistaBusqueda(
            FrmControlDeCambios form,
            IUsuarioHistoricoServices historSvc,
            IUsuarioServices usuSvc,
            IBitacoraServices bitSvc,
            Point ptTitle,
            Point ptFiltros,
            Point ptDgv,
            Size szDgv,
            Point ptBtnRestaurar,
            Size szBtn)
        {
            // Title
            var lbl = new Label
            {
                Name = "lblControlCambiosTitulo",
                Text = "CONTROL DE CAMBIOS",
                Font = new Font("Microsoft Sans Serif", 13.8F),
                AutoSize = true,
                Location = ptTitle
            };
            form.Controls.Add(lbl);

            // Entidad label + ComboBox
            var lblEntidad = new Label
            {
                Name = "lblEntidad",
                Text = "Entidad",
                Font = new Font("Microsoft Sans Serif", 10.2F),
                AutoSize = true,
                Location = ptFiltros
            };
            form.Controls.Add(lblEntidad);

            var cbo = new ComboBox
            {
                Name = "cboEntidades",
                Location = new Point(ptFiltros.X + 90, ptFiltros.Y),
                Size = new Size(134, 21),
                FormattingEnabled = true
            };
            cbo.Items.Add("Usuario");
            form.Controls.Add(cbo);

            // UserName label + TextBox
            var lblUser = new Label
            {
                Name = "lblUserName",
                Text = "Usuario",
                Font = new Font("Microsoft Sans Serif", 10.2F),
                AutoSize = true,
                Location = new Point(ptFiltros.X, ptFiltros.Y + 38)
            };
            form.Controls.Add(lblUser);

            var txt = new TextBox
            {
                Name = "txtUserName",
                Location = new Point(ptFiltros.X + 90, ptFiltros.Y + 38),
                Size = new Size(134, 20)
            };
            form.Controls.Add(txt);

            // DataGridView
            var dgv = FlexibilizadorFormularios.CreateDataGridView(form, "dgvControlCambios", ptDgv, szDgv);
            dgv.CellFormatting += DgvControlCambios_CellFormatting;
            dgv.SelectionChanged += (s, e) =>
            {
                var btnRest = form.Controls.Find("btnRestaurar", true);
                if (btnRest.Length > 0) btnRest[0].Enabled = dgv.SelectedRows.Count > 0;
            };

            // Buscar button
            FlexibilizadorFormularios.CreateButton(
                form, "btnBuscar",
                new Point(ptFiltros.X + 90 + 134 + 8, ptFiltros.Y + 38),
                new Size(65, 23),
                "Buscar",
                (s, e) =>
                {
                    var cboCtrl = form.Controls.Find("cboEntidades", true)[0] as ComboBox;
                    var txtCtrl = form.Controls.Find("txtUserName", true)[0] as TextBox;

                    if (cboCtrl?.SelectedItem is null)
                    {
                        MessageBox.Show("Seleccione una entidad para buscar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (string.IsNullOrEmpty(txtCtrl?.Text))
                    {
                        MessageBox.Show("Ingrese un nombre de usuario para buscar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ActualizarGridView(dgv, txtCtrl.Text, historSvc);
                });

            // Restaurar button (initially disabled)
            var btnRestaurar = FlexibilizadorFormularios.CreateButton(
                form, "btnRestaurar",
                ptBtnRestaurar, szBtn, "Restaurar",
                (s, e) =>
                {
                    usuSvc.SetRegistradoBitacora((usuario, descripcion, origen, tipoEvento) =>
                        RegistrarEnBitacora(bitSvc, usuario, descripcion, origen, tipoEvento));

                    if (dgv.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Seleccione un registro para restaurar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var usuarioHistorico = dgv.SelectedRows[0].DataBoundItem as UsuarioHistorico;
                    if (usuarioHistorico == null) return;

                    var usuarioARevertir = new Usuario
                    {
                        IdUsuario = usuarioHistorico.IdUsuario,
                        Nombre = usuarioHistorico.Nombre,
                        Apellido = usuarioHistorico.Apellido,
                        Email = usuarioHistorico.Email,
                        UserName = usuarioHistorico.UserName,
                        Bloqueado = usuarioHistorico.Bloqueado,
                        CantidadIntentos = usuarioHistorico.CantidadIntentos,
                        FechaEliminado = usuarioHistorico.FechaEliminado
                    };
                    usuSvc.ModificarUsuario(usuarioARevertir);

                    var txtCtrl = form.Controls.Find("txtUserName", true)[0] as TextBox;
                    ActualizarGridView(dgv, txtCtrl?.Text ?? string.Empty, historSvc);
                });
            btnRestaurar.Enabled = false;
        }

        private static void ActualizarGridView(DataGridView dgv, string userName, IUsuarioHistoricoServices svc)
        {
            var historico = svc.ObtenerUsuarioHistorico(userName);
            dgv.DataSource = historico;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgv.Columns["IdUsuario"] != null) dgv.Columns["IdUsuario"].Visible = false;
            if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            if (dgv.Columns["TipoOperacion"] != null) dgv.Columns["TipoOperacion"].Visible = false;

            var columnaBloqueado = dgv.Columns["Bloqueado"];
            if (columnaBloqueado is DataGridViewCheckBoxColumn)
            {
                int displayIndex = columnaBloqueado.DisplayIndex;
                dgv.Columns.Remove(columnaBloqueado);
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Bloqueado",
                    HeaderText = "Bloqueado",
                    DataPropertyName = "Bloqueado",
                    DisplayIndex = displayIndex
                });
            }
            else if (columnaBloqueado != null)
            {
                columnaBloqueado.HeaderText = "Bloqueado";
            }

            if (dgv.Columns["CantidadIntentos"] != null) dgv.Columns["CantidadIntentos"].HeaderText = "Intentos";
            if (dgv.Columns["UserName"] != null) dgv.Columns["UserName"].HeaderText = "Usuario";
            if (dgv.Columns["FechaModificacion"] != null) dgv.Columns["FechaModificacion"].HeaderText = "Fecha";
            if (dgv.Columns["UsuarioModificador"] != null) dgv.Columns["UsuarioModificador"].HeaderText = "Modificado Por";
        }

        private static void DgvControlCambios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;
            if (dgv.Columns[e.ColumnIndex].Name == "Bloqueado" && e.Value != null)
            {
                if (e.Value is bool bloqueado)
                {
                    e.Value = bloqueado ? "Sí" : "No";
                    e.FormattingApplied = true;
                }
            }
        }

        private static void RegistrarEnBitacora(IBitacoraServices bitSvc, Usuario usuario, string descripcion, string origen, TipoEvento tipoEvento)
        {
            var bitacora = new Bitacora
            {
                Id = Guid.NewGuid(),
                Usuario = usuario,
                Fecha = DateTime.Now,
                Descripcion = descripcion,
                Origen = origen,
                TipoEvento = tipoEvento
            };
            bitSvc.GuardarBitacora(bitacora);
        }
    }
}
