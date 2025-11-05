namespace IngSoft.UI
{
    partial class FrmControlDeCambios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblControlCambiosTitulo = new System.Windows.Forms.Label();
            this.lblEntidad = new System.Windows.Forms.Label();
            this.cboEntidades = new System.Windows.Forms.ComboBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvControlCambios = new System.Windows.Forms.DataGridView();
            this.btnRestaurar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvControlCambios)).BeginInit();
            this.SuspendLayout();
            // 
            // lblControlCambiosTitulo
            // 
            this.lblControlCambiosTitulo.AutoSize = true;
            this.lblControlCambiosTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControlCambiosTitulo.Location = new System.Drawing.Point(220, 20);
            this.lblControlCambiosTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblControlCambiosTitulo.Name = "lblControlCambiosTitulo";
            this.lblControlCambiosTitulo.Size = new System.Drawing.Size(223, 24);
            this.lblControlCambiosTitulo.TabIndex = 0;
            this.lblControlCambiosTitulo.Text = "CONTROL DE CAMBIOS";
            // 
            // lblEntidad
            // 
            this.lblEntidad.AutoSize = true;
            this.lblEntidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntidad.Location = new System.Drawing.Point(46, 68);
            this.lblEntidad.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEntidad.Name = "lblEntidad";
            this.lblEntidad.Size = new System.Drawing.Size(56, 17);
            this.lblEntidad.TabIndex = 1;
            this.lblEntidad.Text = "Entidad";
            // 
            // cboEntidades
            // 
            this.cboEntidades.FormattingEnabled = true;
            this.cboEntidades.Items.AddRange(new object[] {
            "Usuario"});
            this.cboEntidades.Location = new System.Drawing.Point(131, 68);
            this.cboEntidades.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cboEntidades.Name = "cboEntidades";
            this.cboEntidades.Size = new System.Drawing.Size(134, 21);
            this.cboEntidades.TabIndex = 2;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(46, 106);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(57, 17);
            this.lblUserName.TabIndex = 3;
            this.lblUserName.Text = "Usuario";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(131, 106);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(134, 20);
            this.txtUserName.TabIndex = 4;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(287, 106);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(65, 23);
            this.btnBuscar.TabIndex = 5;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dgvControlCambios
            // 
            this.dgvControlCambios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvControlCambios.Location = new System.Drawing.Point(19, 151);
            this.dgvControlCambios.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvControlCambios.Name = "dgvControlCambios";
            this.dgvControlCambios.RowHeadersWidth = 51;
            this.dgvControlCambios.RowTemplate.Height = 24;
            this.dgvControlCambios.Size = new System.Drawing.Size(652, 360);
            this.dgvControlCambios.TabIndex = 6;
            this.dgvControlCambios.SelectionChanged += new System.EventHandler(this.dgvControlCambios_SelectionChanged);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.Enabled = false;
            this.btnRestaurar.Location = new System.Drawing.Point(683, 166);
            this.btnRestaurar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(84, 28);
            this.btnRestaurar.TabIndex = 7;
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.UseVisualStyleBackColor = true;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // FrmControlDeCambios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 532);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.dgvControlCambios);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.cboEntidades);
            this.Controls.Add(this.lblEntidad);
            this.Controls.Add(this.lblControlCambiosTitulo);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrmControlDeCambios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmControlDeCambios";
            this.Load += new System.EventHandler(this.FrmControlDeCambios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvControlCambios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblControlCambiosTitulo;
        private System.Windows.Forms.Label lblEntidad;
        private System.Windows.Forms.ComboBox cboEntidades;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvControlCambios;
        private System.Windows.Forms.Button btnRestaurar;
    }
}