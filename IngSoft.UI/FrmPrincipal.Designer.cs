namespace IngSoft.UI
{
    partial class FrmPrincipal
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
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.sesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iniciarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitacoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlDeCambiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multidiomasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.permisosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lblIdioma = new System.Windows.Forms.Label();
            this.cboIdiomas = new System.Windows.Forms.ComboBox();
            this.menuPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuPrincipal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sesionToolStripMenuItem,
            this.usuariosToolStripMenuItem,
            this.bitacoraToolStripMenuItem,
            this.controlDeCambiosToolStripMenuItem,
            this.multidiomasToolStripMenuItem,
            this.permisosToolStripMenuItem});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(1706, 61);
            this.menuPrincipal.TabIndex = 3;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // sesionToolStripMenuItem
            // 
            this.sesionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iniciarSesionToolStripMenuItem,
            this.cerrarSesionToolStripMenuItem});
            this.sesionToolStripMenuItem.Name = "sesionToolStripMenuItem";
            this.sesionToolStripMenuItem.Size = new System.Drawing.Size(151, 57);
            this.sesionToolStripMenuItem.Text = "Sesion";
            // 
            // iniciarSesionToolStripMenuItem
            // 
            this.iniciarSesionToolStripMenuItem.Name = "iniciarSesionToolStripMenuItem";
            this.iniciarSesionToolStripMenuItem.Size = new System.Drawing.Size(429, 66);
            this.iniciarSesionToolStripMenuItem.Text = "Iniciar Sesion";
            this.iniciarSesionToolStripMenuItem.Click += new System.EventHandler(this.iniciarSesionToolStripMenuItem_Click);
            // 
            // cerrarSesionToolStripMenuItem
            // 
            this.cerrarSesionToolStripMenuItem.Name = "cerrarSesionToolStripMenuItem";
            this.cerrarSesionToolStripMenuItem.Size = new System.Drawing.Size(429, 66);
            this.cerrarSesionToolStripMenuItem.Text = "Cerrar Sesion";
            this.cerrarSesionToolStripMenuItem.Click += new System.EventHandler(this.cerrarSesionToolStripMenuItem_Click);
            // 
            // usuariosToolStripMenuItem
            // 
            this.usuariosToolStripMenuItem.Name = "usuariosToolStripMenuItem";
            this.usuariosToolStripMenuItem.Size = new System.Drawing.Size(184, 57);
            this.usuariosToolStripMenuItem.Text = "Usuarios";
            this.usuariosToolStripMenuItem.Click += new System.EventHandler(this.usuariosToolStripMenuItem_Click);
            // 
            // bitacoraToolStripMenuItem
            // 
            this.bitacoraToolStripMenuItem.Name = "bitacoraToolStripMenuItem";
            this.bitacoraToolStripMenuItem.Size = new System.Drawing.Size(177, 57);
            this.bitacoraToolStripMenuItem.Text = "Bitacora";
            this.bitacoraToolStripMenuItem.Click += new System.EventHandler(this.bitacoraToolStripMenuItem_Click);
            // 
            // controlDeCambiosToolStripMenuItem
            // 
            this.controlDeCambiosToolStripMenuItem.Name = "controlDeCambiosToolStripMenuItem";
            this.controlDeCambiosToolStripMenuItem.Size = new System.Drawing.Size(367, 57);
            this.controlDeCambiosToolStripMenuItem.Text = "Control De Cambios";
            this.controlDeCambiosToolStripMenuItem.Click += new System.EventHandler(this.controlDeCambiosToolStripMenuItem_Click);
            // 
            // multidiomasToolStripMenuItem
            // 
            this.multidiomasToolStripMenuItem.Name = "multidiomasToolStripMenuItem";
            this.multidiomasToolStripMenuItem.Size = new System.Drawing.Size(245, 57);
            this.multidiomasToolStripMenuItem.Text = "Multidiomas";
            this.multidiomasToolStripMenuItem.Click += new System.EventHandler(this.multidiomasToolStripMenuItem_Click);
            // 
            // permisosToolStripMenuItem
            // 
            this.permisosToolStripMenuItem.Name = "permisosToolStripMenuItem";
            this.permisosToolStripMenuItem.Size = new System.Drawing.Size(190, 57);
            this.permisosToolStripMenuItem.Text = "Permisos";
            this.permisosToolStripMenuItem.Click += new System.EventHandler(this.permisosToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(150, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 74);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // lblIdioma
            // 
            this.lblIdioma.AutoSize = true;
            this.lblIdioma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdioma.Location = new System.Drawing.Point(747, 41);
            this.lblIdioma.Name = "lblIdioma";
            this.lblIdioma.Size = new System.Drawing.Size(144, 47);
            this.lblIdioma.TabIndex = 5;
            this.lblIdioma.Text = "Idioma";
            // 
            // cboIdiomas
            // 
            this.cboIdiomas.FormattingEnabled = true;
            this.cboIdiomas.Location = new System.Drawing.Point(811, 41);
            this.cboIdiomas.Name = "cboIdiomas";
            this.cboIdiomas.Size = new System.Drawing.Size(62, 45);
            this.cboIdiomas.TabIndex = 6;
            this.cboIdiomas.SelectedIndexChanged += new System.EventHandler(this.cboIdiomas_SelectedIndexChanged);
            // 
            // FrmPrincipal
            // 
            this.ClientSize = new System.Drawing.Size(1706, 661);
            this.Controls.Add(this.cboIdiomas);
            this.Controls.Add(this.lblIdioma);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuPrincipal);
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.FrmPrincipal_Shown);
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem sesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iniciarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cerrarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bitacoraToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem controlDeCambiosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multidiomasToolStripMenuItem;
        private System.Windows.Forms.Label lblIdioma;
        private System.Windows.Forms.ComboBox cboIdiomas;
        private System.Windows.Forms.ToolStripMenuItem permisosToolStripMenuItem;
    }
}

