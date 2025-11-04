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
            this.lblUsuario = new System.Windows.Forms.Label();
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
            this.lblControlCambiosTitulo.Location = new System.Drawing.Point(294, 25);
            this.lblControlCambiosTitulo.Name = "lblControlCambiosTitulo";
            this.lblControlCambiosTitulo.Size = new System.Drawing.Size(286, 29);
            this.lblControlCambiosTitulo.TabIndex = 0;
            this.lblControlCambiosTitulo.Text = "CONTROL DE CAMBIOS";
            // 
            // lblEntidad
            // 
            this.lblEntidad.AutoSize = true;
            this.lblEntidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntidad.Location = new System.Drawing.Point(61, 84);
            this.lblEntidad.Name = "lblEntidad";
            this.lblEntidad.Size = new System.Drawing.Size(65, 20);
            this.lblEntidad.TabIndex = 1;
            this.lblEntidad.Text = "Entidad";
            // 
            // cboEntidades
            // 
            this.cboEntidades.FormattingEnabled = true;
            this.cboEntidades.Items.AddRange(new object[] {
            "Usuario"});
            this.cboEntidades.Location = new System.Drawing.Point(175, 84);
            this.cboEntidades.Name = "cboEntidades";
            this.cboEntidades.Size = new System.Drawing.Size(178, 24);
            this.cboEntidades.TabIndex = 2;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(61, 130);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(67, 20);
            this.lblUsuario.TabIndex = 3;
            this.lblUsuario.Text = "Usuario";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(175, 130);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(178, 22);
            this.txtUserName.TabIndex = 4;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(383, 130);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(87, 28);
            this.btnBuscar.TabIndex = 5;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dgvControlCambios
            // 
            this.dgvControlCambios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvControlCambios.Location = new System.Drawing.Point(25, 186);
            this.dgvControlCambios.Name = "dgvControlCambios";
            this.dgvControlCambios.RowHeadersWidth = 51;
            this.dgvControlCambios.RowTemplate.Height = 24;
            this.dgvControlCambios.Size = new System.Drawing.Size(869, 443);
            this.dgvControlCambios.TabIndex = 6;
            this.dgvControlCambios.SelectionChanged += new System.EventHandler(this.dgvControlCambios_SelectionChanged);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.Enabled = false;
            this.btnRestaurar.Location = new System.Drawing.Point(911, 204);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(112, 35);
            this.btnRestaurar.TabIndex = 7;
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.UseVisualStyleBackColor = true;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // FrmControlDeCambios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 655);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.dgvControlCambios);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.cboEntidades);
            this.Controls.Add(this.lblEntidad);
            this.Controls.Add(this.lblControlCambiosTitulo);
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
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvControlCambios;
        private System.Windows.Forms.Button btnRestaurar;
    }
}