namespace IngSoft.UI
{
    partial class FrmBitacora
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridBitacora = new System.Windows.Forms.DataGridView();
            this.txtBusquedaBitacora = new System.Windows.Forms.TextBox();
            this.lblBitacora = new System.Windows.Forms.Label();
            this.cboTipoEvento = new System.Windows.Forms.ComboBox();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblTipoEvento = new System.Windows.Forms.Label();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.chkFiltroDate = new System.Windows.Forms.CheckBox();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.lblFiltroFechas = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridBitacora)).BeginInit();
            this.SuspendLayout();
            // 
            // gridBitacora
            // 
            this.gridBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBitacora.Location = new System.Drawing.Point(37, 87);
            this.gridBitacora.Name = "gridBitacora";
            this.gridBitacora.RowHeadersWidth = 51;
            this.gridBitacora.RowTemplate.Height = 24;
            this.gridBitacora.Size = new System.Drawing.Size(814, 483);
            this.gridBitacora.TabIndex = 0;
            this.gridBitacora.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridBitacora_DataBindingComplete);
            // 
            // txtBusquedaBitacora
            // 
            this.txtBusquedaBitacora.Location = new System.Drawing.Point(37, 44);
            this.txtBusquedaBitacora.Multiline = true;
            this.txtBusquedaBitacora.Name = "txtBusquedaBitacora";
            this.txtBusquedaBitacora.Size = new System.Drawing.Size(530, 30);
            this.txtBusquedaBitacora.TabIndex = 4;
            this.txtBusquedaBitacora.TextChanged += new System.EventHandler(this.txtBusquedaBitacora_TextChanged);
            // 
            // lblBitacora
            // 
            this.lblBitacora.AutoSize = true;
            this.lblBitacora.Location = new System.Drawing.Point(37, 13);
            this.lblBitacora.Name = "lblBitacora";
            this.lblBitacora.Size = new System.Drawing.Size(68, 13);
            this.lblBitacora.TabIndex = 5;
            this.lblBitacora.Text = "BITACORAS";
            // 
            // cboTipoEvento
            // 
            this.cboTipoEvento.FormattingEnabled = true;
            this.cboTipoEvento.Items.AddRange(new object[] {
            "Todos",
            "Message",
            "Warning",
            "Error"});
            this.cboTipoEvento.Location = new System.Drawing.Point(582, 50);
            this.cboTipoEvento.Name = "cboTipoEvento";
            this.cboTipoEvento.Size = new System.Drawing.Size(117, 21);
            this.cboTipoEvento.TabIndex = 6;
            this.cboTipoEvento.SelectedIndexChanged += new System.EventHandler(this.cboTipoEvento_SelectedIndexChanged);
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Enabled = false;
            this.dtpFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesde.Location = new System.Drawing.Point(888, 166);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(138, 20);
            this.dtpFechaDesde.TabIndex = 7;
            this.dtpFechaDesde.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);
            // 
            // lblTipoEvento
            // 
            this.lblTipoEvento.AutoSize = true;
            this.lblTipoEvento.Location = new System.Drawing.Point(590, 32);
            this.lblTipoEvento.Name = "lblTipoEvento";
            this.lblTipoEvento.Size = new System.Drawing.Size(65, 13);
            this.lblTipoEvento.TabIndex = 8;
            this.lblTipoEvento.Text = "Tipo Evento";
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Location = new System.Drawing.Point(885, 144);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(71, 13);
            this.lblFechaDesde.TabIndex = 9;
            this.lblFechaDesde.Text = "Fecha Desde";
            // 
            // chkFiltroDate
            // 
            this.chkFiltroDate.AutoSize = true;
            this.chkFiltroDate.Location = new System.Drawing.Point(968, 104);
            this.chkFiltroDate.Name = "chkFiltroDate";
            this.chkFiltroDate.Size = new System.Drawing.Size(15, 14);
            this.chkFiltroDate.TabIndex = 10;
            this.chkFiltroDate.UseVisualStyleBackColor = true;
            this.chkFiltroDate.CheckedChanged += new System.EventHandler(this.chkFiltroDate_CheckedChanged);
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Enabled = false;
            this.dtpFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHasta.Location = new System.Drawing.Point(888, 229);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(138, 20);
            this.dtpFechaHasta.TabIndex = 11;
            this.dtpFechaHasta.ValueChanged += new System.EventHandler(this.dtpFechaHasta_ValueChanged);
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(885, 207);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(68, 13);
            this.lblFechaHasta.TabIndex = 12;
            this.lblFechaHasta.Text = "Fecha Hasta";
            // 
            // lblFiltroFechas
            // 
            this.lblFiltroFechas.AutoSize = true;
            this.lblFiltroFechas.Location = new System.Drawing.Point(885, 101);
            this.lblFiltroFechas.Name = "lblFiltroFechas";
            this.lblFiltroFechas.Size = new System.Drawing.Size(67, 13);
            this.lblFiltroFechas.TabIndex = 13;
            this.lblFiltroFechas.Text = "Filtro Fechas";
            // 
            // FrmBitacora
            // 
            this.ClientSize = new System.Drawing.Size(1041, 597);
            this.Controls.Add(this.lblFiltroFechas);
            this.Controls.Add(this.lblFechaHasta);
            this.Controls.Add(this.dtpFechaHasta);
            this.Controls.Add(this.chkFiltroDate);
            this.Controls.Add(this.lblFechaDesde);
            this.Controls.Add(this.lblTipoEvento);
            this.Controls.Add(this.dtpFechaDesde);
            this.Controls.Add(this.cboTipoEvento);
            this.Controls.Add(this.lblBitacora);
            this.Controls.Add(this.txtBusquedaBitacora);
            this.Controls.Add(this.gridBitacora);
            this.Name = "FrmBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.FrmBitacora_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridBitacora)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridBitacora;
        private System.Windows.Forms.TextBox txtBusquedaBitacora;
        private System.Windows.Forms.Label lblBitacora;
        private System.Windows.Forms.ComboBox cboTipoEvento;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Label lblTipoEvento;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.CheckBox chkFiltroDate;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.Label lblFiltroFechas;
    }
}
