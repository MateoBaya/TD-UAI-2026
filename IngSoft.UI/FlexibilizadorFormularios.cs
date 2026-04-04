using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using IngSoft.Domain;
using IngSoft.UI.Dictionaries;
using IngSoft.UI.Pagination;

namespace IngSoft.UI
{
    internal static class FlexibilizadorFormularios
    {
        // ── Internal helper ──────────────────────────────────────────────────────

        private static T AddOrReplaceControl<T>(Control parent, T control) where T : Control
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrWhiteSpace(control.Name))
                throw new ArgumentException("El control debe tener un nombre", nameof(control.Name));

            var existing = parent.Controls.Find(control.Name, true).FirstOrDefault() as Control;
            if (existing != null)
                parent.Controls.Remove(existing);

            parent.Controls.Add(control);
            return control;
        }

        // ── Form-level helpers ───────────────────────────────────────────────────

        public static void EliminarControlesAdicionalesForm(Control form, Dictionary<Control, Control> controlesEvitar)
        {
            Control sideBar = form.Controls.Find("pnlNavBar", true).FirstOrDefault();
            form.Controls.Clear();
            if (sideBar != null)
                form.Controls.Add(sideBar);
            foreach (Control control in controlesEvitar.Keys)
                controlesEvitar[control].Controls.Add(control);
        }

        // ── Input controls ───────────────────────────────────────────────────────

        /// <summary>Crea (o reemplaza) un label + TextBox en el formulario padre.</summary>
        public static TextBox CreateTextBox(Control parent, string param, Point position,
            Size? size = null, Font font = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("param no puede ser vacío", nameof(param));

            var lbl = new Label
            {
                Name     = $"lbl{param}",
                Location = new Point(position.X, position.Y - 20),
                Size     = size ?? new Size(200, 20),
                Text     = param,
                Visible  = true
            };

            var txt = new TextBox
            {
                Name     = $"txt{param}",
                Location = new Point(position.X, position.Y),
                Size     = size ?? new Size(200, 30),
                Text     = string.Empty,
                Visible  = true,
                Enabled  = true,
                ReadOnly = false,
                Font     = font ?? new Font("Arial", 10)
            };

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, txt);
            return txt;
        }

        /// <summary>
        /// Crea (o reemplaza) un label + TextBox con PasswordChar='*'.
        /// Reutilizable para cualquier campo de contraseña o PIN.
        /// </summary>
        public static TextBox CreatePasswordTextBox(Control parent, string param, Point position,
            Size? size = null, Font font = null)
        {
            var txt = CreateTextBox(parent, param, position, size, font);
            txt.PasswordChar = '*';
            return txt;
        }

        /// <summary>
        /// Crea (o reemplaza) un label + ComboBox con los items provistos.
        /// Selecciona el primer item por defecto.
        /// </summary>
        public static ComboBox CreateComboBox(Control parent, string param, Point position,
            Size? size = null, IEnumerable<string> items = null,
            EventHandler onSelectedIndexChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("param no puede ser vacío", nameof(param));

            var lbl = new Label
            {
                Name     = $"lbl{param}",
                Location = new Point(position.X, position.Y - 20),
                Size     = new Size(size?.Width ?? 200, 20),
                Text     = param,
                Visible  = true
            };

            var cbo = new ComboBox
            {
                Name          = $"cbo{param}",
                Location      = new Point(position.X, position.Y),
                Size          = size ?? new Size(200, 30),
                Visible       = true,
                Enabled       = true,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            if (items != null)
            {
                cbo.Items.Clear();
                foreach (var item in items)
                    cbo.Items.Add(item ?? string.Empty);
                if (cbo.Items.Count > 0)
                    cbo.SelectedIndex = 0;
            }

            if (onSelectedIndexChanged != null)
                cbo.SelectedIndexChanged += onSelectedIndexChanged;

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, cbo);
            return cbo;
        }

        /// <summary>
        /// Crea (o reemplaza) un label + DateTimePicker.
        /// El picker arranca con la fecha de hoy; se puede habilitar/deshabilitar desde el exterior.
        /// </summary>
        public static DateTimePicker CreateDateTimePicker(Control parent, string param,
            Point position, Size? size = null, bool enabled = true,
            EventHandler onValueChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("param no puede ser vacío", nameof(param));

            var lbl = new Label
            {
                Name     = $"lbl{param}",
                Location = new Point(position.X, position.Y - 20),
                Size     = new Size(size?.Width ?? 200, 20),
                Text     = param,
                Visible  = true
            };

            var dtp = new DateTimePicker
            {
                Name     = $"dtp{param}",
                Location = new Point(position.X, position.Y),
                Size     = size ?? new Size(200, 30),
                Visible  = true,
                Enabled  = enabled,
                Value    = DateTime.Today,
                Format   = DateTimePickerFormat.Short
            };

            if (onValueChanged != null)
                dtp.ValueChanged += onValueChanged;

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, dtp);
            return dtp;
        }

        /// <summary>Crea (o reemplaza) un label + ListBox en el formulario padre.</summary>
        public static ListBox CreateListBox(Control parent, string param, Point position,
            Size? size = null, IEnumerable<string> items = null,
            EventHandler onSelectedIndexChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("param no puede ser vacío", nameof(param));

            var lbl = new Label
            {
                Name     = $"lbl{param}",
                Location = new Point(position.X, position.Y - 20),
                Size     = new Size(200, 20),
                Text     = param,
                Visible  = true
            };

            var lst = new ListBox
            {
                Name          = $"lst{param}",
                Location      = new Point(position.X, position.Y),
                Size          = size ?? new Size(200, 100),
                Visible       = true,
                Enabled       = true,
                SelectionMode = SelectionMode.One
            };

            if (items != null)
            {
                lst.Items.Clear();
                foreach (var it in items)
                    lst.Items.Add(it ?? string.Empty);
            }

            if (onSelectedIndexChanged != null)
                lst.SelectedIndexChanged += onSelectedIndexChanged;

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, lst);
            return lst;
        }

        /// <summary>Crea (o reemplaza) un CheckBox en el formulario padre.</summary>
        public static CheckBox CreateCheckBox(Control parent, string param, Point position,
            Size? size = null, bool isChecked = false, EventHandler onCheckedChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException("param no puede ser vacío", nameof(param));

            var chk = new CheckBox
            {
                Name     = $"chk{param}",
                Location = position,
                Size     = size ?? new Size(200, 30),
                Text     = param,
                Checked  = isChecked,
                Visible  = true,
                Enabled  = true
            };

            if (onCheckedChanged != null)
                chk.CheckedChanged += onCheckedChanged;

            AddOrReplaceControl(parent, chk);
            return chk;
        }

        // ── Button helpers ───────────────────────────────────────────────────────

        /// <summary>Crea (o reemplaza) un Button con tamaño explícito.</summary>
        public static Button CreateButton(Control parent, string name, Point position,
            Size size, string text, EventHandler onClick)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name no puede ser vacío", nameof(name));

            var btn = new Button
            {
                Name     = name,
                Location = position,
                Size     = size,
                Text     = text,
                Visible  = true,
                Enabled  = true
            };

            if (onClick != null)
                btn.Click += onClick;

            AddOrReplaceControl(parent, btn);
            return btn;
        }

        /// <summary>Sobrecarga con tamaño por defecto (200 x 30).</summary>
        public static Button CreateButton(Control parent, string name, Point position,
            string text, EventHandler onClick)
            => CreateButton(parent, name, position, new Size(200, 30), text, onClick);

        // ── Grid helpers ─────────────────────────────────────────────────────────

        /// <summary>Crea (o reemplaza) un DataGridView con DataTable como DataSource.</summary>
        public static DataGridView CreateDataGridView(Control parent, string name,
            Point position, Size size, DataTable dataSource = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name no puede ser vacío", nameof(name));

            var dgv = new DataGridView
            {
                Name                    = name,
                Location                = position,
                Size                    = size,
                Visible                 = true,
                Enabled                 = true,
                AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect             = false,
                ReadOnly                = true,
                AllowUserToAddRows      = false,
                AllowUserToDeleteRows   = false,
                AllowUserToOrderColumns = true,
                RowHeadersVisible       = true
            };

            if (dataSource != null)
                dgv.DataSource = dataSource;

            var existing = parent.Controls.Find(name, true).FirstOrDefault() as DataGridView;
            if (existing != null)
                parent.Controls.Remove(existing);

            parent.Controls.Add(dgv);
            return dgv;
        }

        /// <summary>
        /// Sobrecarga genérica: construye el DataTable internamente a partir de objetos.
        /// columnDefinitions: diccionario nombre->tipo.
        /// </summary>
        public static DataGridView CreateDataGridView<T>(Control parent, string name,
            Point position, Size size, IDictionary<string, Type> columnDefinitions,
            IEnumerable<T> items)
        {
            var dt = new DataTable();
            if (columnDefinitions != null)
                foreach (var kv in columnDefinitions)
                    dt.Columns.Add(kv.Key, kv.Value);

            if (items != null && columnDefinitions != null)
            {
                var props = typeof(T).GetProperties();
                foreach (var item in items)
                {
                    var row = dt.NewRow();
                    foreach (var kv in columnDefinitions)
                    {
                        var prop = props.FirstOrDefault(p => p.Name == kv.Key);
                        row[kv.Key] = prop?.GetValue(item) ?? DBNull.Value;
                    }
                    dt.Rows.Add(row);
                }
            }

            return CreateDataGridView(parent, name, position, size, dt);
        }

        /// <summary>
        /// Factory for a paginated DataGridView using the Decorator pattern.
        /// Returns a <see cref="DataGridViewPaginationDecorator"/> that owns the
        /// inner DataGridView and its pagination bar.
        ///
        /// Callers use decorator.SetDataSource(list) instead of grid.DataSource = ...
        /// and call decorator.Resize(position, size) on window resize.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="name">Name for the inner DataGridView.</param>
        /// <param name="position">Top-left of the combined (grid + pagination bar) area.</param>
        /// <param name="totalSize">Total size including the 30 px pagination bar.</param>
        /// <param name="pageSize">Rows per page (default 15).</param>
        /// <param name="onDataBindingComplete">Optional handler attached to the inner grid's
        /// DataBindingComplete event (e.g. to auto-size a column).</param>
        public static DataGridViewPaginationDecorator CreatePaginatedDataGridView(
            Control parent, string name, Point position, Size totalSize,
            int pageSize = 15,
            DataGridViewBindingCompleteEventHandler onDataBindingComplete = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name no puede ser vacío", nameof(name));

            // Create the raw grid first (not added to parent yet — the decorator handles that)
            var dgv = new DataGridView
            {
                Name                    = name,
                Visible                 = true,
                Enabled                 = true,
                AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect             = false,
                ReadOnly                = true,
                AllowUserToAddRows      = false,
                AllowUserToDeleteRows   = false,
                AllowUserToOrderColumns = true,
                RowHeadersVisible       = true
            };

            // Remove any pre-existing grid with the same name so AddOrReplaceControl stays consistent
            var existing = parent.Controls.Find(name, true).FirstOrDefault();
            if (existing != null)
                parent.Controls.Remove(existing);

            if (onDataBindingComplete != null)
                dgv.DataBindingComplete += onDataBindingComplete;

            return new DataGridViewPaginationDecorator(dgv, parent, position, totalSize, pageSize);
        }

        // ── TreeView helpers ─────────────────────────────────────────────────────

        /// <summary>Crea (o reemplaza) un TreeView en el formulario padre.</summary>
        public static TreeView CreateTreeView(Control parent, string name,
            Point position, Size size)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name no puede ser vacío", nameof(name));

            var tree = new TreeView
            {
                Name          = name,
                Location      = position,
                Size          = size,
                Visible       = true,
                Enabled       = true,
                HideSelection = false
            };

            var existing = parent.Controls.Find(name, true).FirstOrDefault() as TreeView;
            if (existing != null)
                parent.Controls.Remove(existing);

            parent.Controls.Add(tree);
            return tree;
        }

        /// <summary>
        /// Carga iterativamente el TreeView a partir de un PermisoComponent usando Ejecutar.
        /// </summary>
        public static void LoadTreeViewFromPermisos(TreeView treeView, PermisoComponent permisoRoot)
        {
            if (treeView == null) throw new ArgumentNullException(nameof(treeView));

            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                if (permisoRoot == null) return;

                var nodesByName = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

                Action<PermisoComponent> action = permiso =>
                {
                    if (permiso == null) return;
                    var node = new TreeNode(permiso.Nombre) { Name = permiso.Nombre, Tag = permiso };
                    var parentName = permiso.ParentName;
                    if (!string.IsNullOrEmpty(parentName) &&
                        nodesByName.TryGetValue(parentName, out TreeNode parentNode))
                        parentNode.Nodes.Add(node);
                    else
                        treeView.Nodes.Add(node);
                    if (!nodesByName.ContainsKey(permiso.Nombre))
                        nodesByName.Add(permiso.Nombre, node);
                };

                permisoRoot.Ejecutar(action);
                treeView.ExpandAll();
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        // ── MenuStrip helpers ────────────────────────────────────────────────────

        public static void MenuStripHider(MenuStrip stripToHide, PermisoComponent permisoRoot)
        {
            foreach (ToolStripMenuItem item in stripToHide.Items)
                MenuStripItemRecursiveHider(item, permisoRoot, new PermisoAtomico());
        }

        public static void MenuStripClearer(MenuStrip stripToClear, List<ToolStripMenuItem> botonesEvitar)
        {
            stripToClear.Items.Clear();
            foreach (ToolStripMenuItem item in botonesEvitar)
                stripToClear.Items.Add(item);
        }

        private static void MenuStripItemRecursiveHider(ToolStripMenuItem item,
            PermisoComponent permisoRoot, PermisoComponent permisoTemp)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (!(permisoRoot is null))
            {
                if (DictionaryPermisos.PermisoControl.TryGetValue(item.Name, out string permisoNombre))
                {
                    permisoTemp.Nombre = permisoNombre;
                    item.Enabled = !(permisoRoot.BuscarRecursivo(permisoTemp) is null);
                    item.Visible = item.Enabled;
                }
                else
                {
                    item.Enabled = true;
                    item.Visible = true;
                }
                foreach (ToolStripItem subItem in item.DropDownItems)
                    if (subItem is ToolStripMenuItem sub)
                        MenuStripItemRecursiveHider(sub, permisoRoot, permisoTemp);
            }
            else
            {
                if (DictionaryPermisos.PermisoControl.TryGetValue(item.Name, out _))
                {
                    item.Visible = false;
                    item.Enabled = false;
                }
                else
                {
                    item.Visible = true;
                    item.Enabled = true;
                }
            }
        }

        public static void MenuStripHider(MenuStrip stripToHide, List<string> buttonsToShow)
        {
            foreach (ToolStripMenuItem item in stripToHide.Items)
                MenuStripItemRecursiveHider(item, buttonsToShow);
        }

        private static void MenuStripItemRecursiveHider(ToolStripMenuItem item,
            List<string> buttonsToShow)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.Visible = buttonsToShow?.Contains(item.Name) ?? false;
            foreach (ToolStripItem subItem in item.DropDownItems)
                if (subItem is ToolStripMenuItem sub)
                    MenuStripItemRecursiveHider(sub, buttonsToShow);
        }

        // ── Side-panel helpers ───────────────────────────────────────────────────

        public static void LoadSidePanelFromPermisos(Panel sidePanel, PermisoComponent root,
            Dictionary<string, string> buttons, Dictionary<string, EventHandler> buttonAction)
        {
            int currentY = 0;
            foreach (Control control in sidePanel.Controls)
                currentY = Math.Max(currentY, control.Location.Y + control.Height);

            sidePanel.AutoScroll = true;

            foreach (string boton in buttons.Keys)
            {
                if (root.Operacion(DictionaryPermisos.PermisoControl[boton]))
                {
                    CreateButtonToPanel(boton, buttons[boton], sidePanel,
                        new Point(0, currentY), buttonAction[boton]);
                    currentY += 50;
                }
            }

            if (currentY > sidePanel.Height)
            {
                sidePanel.AutoScrollMinSize = new Size(0, currentY);
                sidePanel.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
            }
        }

        public static void CreateButtonToPanel(string name, string text, Panel panel,
            Point position, EventHandler onClick)
        {
            if (panel == null) throw new ArgumentNullException(nameof(panel));

            var panelButton = new Panel
            {
                Name     = "pnl" + name,
                Location = position,
                Size     = new Size(panel.Width, 50)
            };

            var label = new Label
            {
                Name      = name,
                Text      = text,
                Parent    = panelButton,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Arial", 10, FontStyle.Bold),
                Padding   = new Padding(10, 0, 0, 0)
            };

            label.MouseEnter += (s, e) =>
            {
                panelButton.BackColor = ProfessionalColors.ButtonPressedHighlight;
                panelButton.ForeColor = SystemColors.ButtonHighlight;
            };
            label.MouseLeave += (s, e) =>
            {
                panelButton.BackColor = SystemColors.Control;
                panelButton.ForeColor = SystemColors.ControlText;
            };

            if (onClick != null)
            {
                panelButton.Click += onClick;
                label.Click       += onClick;
            }

            panel.Controls.Add(panelButton);
            panelButton.Controls.Add(label);
        }

        public static void SidePanelClearer(Panel sidePanel, Dictionary<Control, Control> saveControls)
        {
            if (sidePanel == null) throw new ArgumentNullException(nameof(sidePanel));
            sidePanel.Controls.Clear();
            if (saveControls != null && saveControls.Count > 0)
                foreach (Control control in saveControls.Keys)
                    saveControls[control].Controls.Add(control);
        }
    }
}
