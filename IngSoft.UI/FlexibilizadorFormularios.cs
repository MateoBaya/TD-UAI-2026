using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

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
    }
}
