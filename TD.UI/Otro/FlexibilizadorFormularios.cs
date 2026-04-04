using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using IngSoft.Domain;
using IngSoft.UI.Dictionary;

namespace IngSoft.UI
{
    internal static class FlexibilizadorFormularios
    {
        // Helper para agregar o reemplazar un control ya configurado
        private static T AddOrReplaceControl<T>(Form parent, T control) where T : Control
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrWhiteSpace(control.Name)) throw new ArgumentException("El control debe tener un nombre", nameof(control.Name));

            var existing = parent.Controls.Find(control.Name, true).FirstOrDefault() as Control;
            if (existing != null)
            {
                parent.Controls.Remove(existing);
            }

            parent.Controls.Add(control);
            return control;
        }

        public static void EliminarControlesAdicionalesForm(Form form)
        {
            Control menuStrip = form.MainMenuStrip;
            form.Controls.Clear();
            form.Controls.Add(menuStrip);
        }

        // Crea (o reemplaza) un label + textbox dinámicamente en el formulario padre
        public static TextBox CreateTextBox(Form parent, string param, Point position, Size? size = null, Font font = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param)) throw new ArgumentException("param no puede ser vacío", nameof(param));

            var labelName = $"lbl{param}";
            var textBoxName = $"txt{param}";

            var lbl = new Label
            {
                Name = labelName,
                Location = new Point(position.X, position.Y - 20),
                Size = size ?? new Size(200, 20),
                Text = param,
                Visible = true
            };

            var txt = new TextBox
            {
                Name = textBoxName,
                Location = new Point(position.X, position.Y),
                Size = size ?? new Size(200, 30),
                Text = string.Empty,
                Visible = true,
                Enabled = true,
                ReadOnly = false,
                Font = font ?? new Font("Arial", 10)
            };

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, txt);

            return txt;
        }

        // Nuevo: crea (o reemplaza) un label + ListBox dinámicamente en el formulario padre
        public static ListBox CreateListBox(Form parent, string param, Point position, Size? size = null, IEnumerable<string> items = null, EventHandler onSelectedIndexChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param)) throw new ArgumentException("param no puede ser vacío", nameof(param));

            var labelName = $"lbl{param}";
            var listBoxName = $"lst{param}";

            var lbl = new Label
            {
                Name = labelName,
                Location = new Point(position.X, position.Y - 20),
                Size = new Size(200, 20),
                Text = param,
                Visible = true
            };

            var lst = new ListBox
            {
                Name = listBoxName,
                Location = new Point(position.X, position.Y),
                Size = size ?? new Size(200, 100),
                Visible = true,
                Enabled = true,
                SelectionMode = SelectionMode.One
            };

            if (items != null)
            {
                lst.Items.Clear();
                foreach (var it in items)
                {
                    lst.Items.Add(it ?? string.Empty);
                }
            }

            if (onSelectedIndexChanged != null)
                lst.SelectedIndexChanged += onSelectedIndexChanged;

            AddOrReplaceControl(parent, lbl);
            AddOrReplaceControl(parent, lst);

            return lst;
        }

        // Crea (o reemplaza) un CheckBox dinámicamente en el formulario padre (similar a CreateTextBox)
        public static CheckBox CreateCheckBox(Form parent, string param, Point position, Size? size = null, bool isChecked = false, EventHandler onCheckedChanged = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(param)) throw new ArgumentException("param no puede ser vacío", nameof(param));

            var checkBoxName = $"chk{param}";

            var chk = new CheckBox
            {
                Name = checkBoxName,
                Location = position,
                Size = size ?? new Size(200, 30),
                Text = param,
                Checked = isChecked,
                Visible = true,
                Enabled = true
            };

            if (onCheckedChanged != null)
                chk.CheckedChanged += onCheckedChanged;

            AddOrReplaceControl(parent, chk);

            return chk;
        }

        // Crea (o reemplaza) un botón dinámicamente y asigna el handler pasado
        public static Button CreateButton(Form parent, string name, Point position, Size size, string text, EventHandler onClick)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name no puede ser vacío", nameof(name));

            var btn = new Button
            {
                Name = name,
                Location = position,
                Size = size,
                Text = text,
                Visible = true,
                Enabled = true
            };

            if (onClick != null)
                btn.Click += onClick;

            AddOrReplaceControl(parent, btn);

            return btn;
        }

        // Sobrecarga con tamaño por defecto
        public static Button CreateButton(Form parent, string name, Point position, string text, EventHandler onClick)
        {
            return CreateButton(parent, name, position, new Size(200, 30), text, onClick);
        }

        // Crea (o reemplaza) un DataGridView en el formulario padre y le asigna el DataTable como DataSource
        public static DataGridView CreateDataGridView(Form parent, string name, Point position, Size size, DataTable dataSource = null)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name no puede ser vacío", nameof(name));

            var dgv = new DataGridView
            {
                Name = name,
                Location = position,
                Size = size,
                Visible = true,
                Enabled = true,
            };

            if (dataSource != null)
            {
                dgv.DataSource = dataSource;
            }

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = true;
            dgv.RowHeadersVisible = true;

            var existing = parent.Controls.Find(name, true).FirstOrDefault() as DataGridView;
            if (existing != null)
            {
                parent.Controls.Remove(existing);
            }

            parent.Controls.Add(dgv);

            return dgv;
        }

        // Sobrecarga genérica que recibe una colección de objetos y construye el DataTable internamente
        // columnDefinitions: diccionario nombre->tipo
        public static DataGridView CreateDataGridView<T>(Form parent, string name, Point position, Size size, IDictionary<string, Type> columnDefinitions, IEnumerable<T> items)
        {
            DataTable dt = new DataTable();
            if (columnDefinitions != null)
            {
                foreach (var kv in columnDefinitions)
                {
                    dt.Columns.Add(kv.Key, kv.Value);
                }
            }

            if (items != null && columnDefinitions != null)
            {
                var props = typeof(T).GetProperties();
                foreach (var item in items)
                {
                    var row = dt.NewRow();
                    foreach (var kv in columnDefinitions)
                    {
                        var colName = kv.Key;
                        var prop = props.FirstOrDefault(p => p.Name == colName);
                        object value = null;
                        if (prop != null)
                            value = prop.GetValue(item);
                        row[colName] = value ?? DBNull.Value;
                    }
                    dt.Rows.Add(row);
                }
            }

            return CreateDataGridView(parent, name, position, size, dt);
        }

        // Crea (o reemplaza) un TreeView en el formulario padre
        public static TreeView CreateTreeView(Form parent, string name, Point position, Size size)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name no puede ser vacío", nameof(name));

            var tree = new TreeView
            {
                Name = name,
                Location = position,
                Size = size,
                Visible = true,
                Enabled = true,
                HideSelection = false
            };

            var existing = parent.Controls.Find(name, true).FirstOrDefault() as TreeView;
            if (existing != null)
            {
                parent.Controls.Remove(existing);
            }

            parent.Controls.Add(tree);
            return tree;
        }

        // Carga iterativamente el TreeView a partir de un PermisoComponent usando su método Ejecutar.
        // Se crea un TreeNode por cada permiso; si el permiso tiene ParentName y el nodo padre ya fue creado se añade como hijo,
        // en caso contrario se añade al nivel raíz.
        public static void LoadTreeViewFromPermisos(TreeView treeView, PermisoComponent permisoRoot)
        {
            if (treeView == null) throw new ArgumentNullException(nameof(treeView));

            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();

                if (permisoRoot == null) return;

                // Diccionario para encontrar nodos creados por nombre (se asume nombres únicos)
                var nodesByName = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

                // Acción que se pasará a Ejecutar: crear nodo y agregar según ParentName
                Action<PermisoComponent> action = permiso =>
                {
                    if (permiso == null) return;

                    var node = new TreeNode(permiso.Nombre)
                    {
                        Name = permiso.Nombre,
                        Tag = permiso
                    };

                    // intentar obtener nombre del padre (puede ser null)
                    var parentName = permiso.ParentName;

                    if (!string.IsNullOrEmpty(parentName) && nodesByName.TryGetValue(parentName, out TreeNode parentNode))
                    {
                        parentNode.Nodes.Add(node);
                    }
                    else
                    {
                        treeView.Nodes.Add(node);
                    }

                    // registrar en diccionario para que sus hijos puedan referenciarlo
                    if (!nodesByName.ContainsKey(permiso.Nombre))
                        nodesByName.Add(permiso.Nombre, node);
                };

                // Usar Ejecutar para recorrer en orden y crear nodos
                permisoRoot.Ejecutar(action);

                treeView.ExpandAll();
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        public static void MenuStripHider(MenuStrip stripToHide, PermisoComponent permisoRoot)
        {
            foreach (ToolStripMenuItem item in stripToHide.Items)
            {
                MenuStripItemRecursiveHider(item, permisoRoot, new PermisoAtomico());
            }
        }
        private static void MenuStripItemRecursiveHider(ToolStripMenuItem item, PermisoComponent permisoRoot, PermisoComponent permisoTemp)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if(!(permisoRoot is null))
            {
                // Lógica para ocultar elementos del menú según los permisos
                if(DictionaryPermisos.PermisoControl.TryGetValue(item.Name, out string permisoNombre))
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
                {
                    if (subItem is ToolStripMenuItem subMenuItem)
                    {
                        MenuStripItemRecursiveHider(subMenuItem, permisoRoot, permisoTemp);
                    }
                }

            }
            else
            {
                if (DictionaryPermisos.PermisoControl.TryGetValue(item.Name, out string permisoNombre))
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
            {
                FlexibilizadorFormularios.MenuStripItemRecursiveHider(item, buttonsToShow);
            }
        }
        private static void MenuStripItemRecursiveHider(ToolStripMenuItem item, List<string> buttonsToShow)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if(!(buttonsToShow is null))
            {
                item.Visible = buttonsToShow.Contains(item.Name);
            }
            else
            {
                item.Visible = false;
            }

            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    MenuStripItemRecursiveHider(subMenuItem, buttonsToShow);
                }
            }
        }


    }
}
