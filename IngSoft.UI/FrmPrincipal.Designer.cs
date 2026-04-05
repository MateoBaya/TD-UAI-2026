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
            this.lblIdioma = new System.Windows.Forms.Label();
            this.cboIdiomas = new System.Windows.Forms.ComboBox();
            this.pnlNavBar = new System.Windows.Forms.Panel();
            this.menuStripSidePanel = new System.Windows.Forms.MenuStrip();
            this.xSidePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tuMayoristaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iniciarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlNavBar.SuspendLayout();
            this.menuStripSidePanel.SuspendLayout();
            this.menuPrincipal.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIdioma
            // 
            this.lblIdioma.AutoSize = true;
            this.lblIdioma.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdioma.Location = new System.Drawing.Point(39, 46);
            this.lblIdioma.Name = "lblIdioma";
            this.lblIdioma.Size = new System.Drawing.Size(58, 20);
            this.lblIdioma.TabIndex = 5;
            this.lblIdioma.Text = "Idioma";
            // 
            // cboIdiomas
            // 
            this.cboIdiomas.FormattingEnabled = true;
            this.cboIdiomas.Location = new System.Drawing.Point(120, 46);
            this.cboIdiomas.Name = "cboIdiomas";
            this.cboIdiomas.Size = new System.Drawing.Size(62, 24);
            this.cboIdiomas.TabIndex = 0;
            this.cboIdiomas.TabStop = false;
            this.cboIdiomas.SelectedIndexChanged += new System.EventHandler(this.cboIdiomas_SelectedIndexChanged);
            // 
            // pnlNavBar
            // 
            this.pnlNavBar.Controls.Add(this.menuStripSidePanel);
            this.pnlNavBar.Controls.Add(this.cboIdiomas);
            this.pnlNavBar.Controls.Add(this.lblIdioma);
            this.pnlNavBar.Location = new System.Drawing.Point(-226, 0);
            this.pnlNavBar.Name = "pnlNavBar";
            this.pnlNavBar.Size = new System.Drawing.Size(223, 642);
            this.pnlNavBar.TabIndex = 7;
            // 
            // menuStripSidePanel
            // 
            this.menuStripSidePanel.BackColor = System.Drawing.Color.MidnightBlue;
            this.menuStripSidePanel.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripSidePanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xSidePanelToolStripMenuItem,
            this.tuMayoristaToolStripMenuItem});
            this.menuStripSidePanel.Location = new System.Drawing.Point(0, 0);
            this.menuStripSidePanel.Name = "menuStripSidePanel";
            this.menuStripSidePanel.Size = new System.Drawing.Size(223, 33);
            this.menuStripSidePanel.TabIndex = 1;
            this.menuStripSidePanel.Text = "menuStrip1";
            // 
            // xSidePanelToolStripMenuItem
            // 
            this.xSidePanelToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.xSidePanelToolStripMenuItem.BackColor = System.Drawing.Color.MidnightBlue;
            this.xSidePanelToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xSidePanelToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.xSidePanelToolStripMenuItem.Name = "xSidePanelToolStripMenuItem";
            this.xSidePanelToolStripMenuItem.Size = new System.Drawing.Size(40, 29);
            this.xSidePanelToolStripMenuItem.Text = "X";
            this.xSidePanelToolStripMenuItem.Click += new System.EventHandler(this.xSidePanelToolStripMenuItem_Click);
            // 
            // tuMayoristaToolStripMenuItem
            // 
            this.tuMayoristaToolStripMenuItem.Font = new System.Drawing.Font("Lucida Fax", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tuMayoristaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tuMayoristaToolStripMenuItem.Name = "tuMayoristaToolStripMenuItem";
            this.tuMayoristaToolStripMenuItem.Size = new System.Drawing.Size(150, 29);
            this.tuMayoristaToolStripMenuItem.Text = "Tu Mayorista";
            this.tuMayoristaToolStripMenuItem.Click += new System.EventHandler(this.tuMayoristaToolStripMenuItem_Click);
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.xToolStripMenuItem,
            this.maximizeToolStripMenuItem,
            this.minimizeToolStripMenuItem,
            this.sesionToolStripMenuItem});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Padding = new System.Windows.Forms.Padding(2, 0, 0, 2);
            this.menuPrincipal.Size = new System.Drawing.Size(884, 26);
            this.menuPrincipal.TabIndex = 3;
            this.menuPrincipal.Text = "menuStrip1";
            this.menuPrincipal.DoubleClick += new System.EventHandler(this.menuPrincipal_DoubleClick);
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(36, 24);
            this.menuToolStripMenuItem.Text = "☰";
            this.menuToolStripMenuItem.Click += new System.EventHandler(this.menuToolStripMenuItem_Click);
            // 
            // xToolStripMenuItem
            // 
            this.xToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.xToolStripMenuItem.BackColor = System.Drawing.Color.Red;
            this.xToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.xToolStripMenuItem.Name = "xToolStripMenuItem";
            this.xToolStripMenuItem.Size = new System.Drawing.Size(38, 24);
            this.xToolStripMenuItem.Text = "🗙";
            this.xToolStripMenuItem.Click += new System.EventHandler(this.xToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(38, 24);
            this.maximizeToolStripMenuItem.Text = "🗖";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.maximizeToolStripMenuItem1_Click);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(38, 24);
            this.minimizeToolStripMenuItem.Text = "🗕";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.minimizeToolStripMenuItem_Click);
            // 
            // sesionToolStripMenuItem
            // 
            this.sesionToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sesionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iniciarSesionToolStripMenuItem,
            this.cerrarSesionToolStripMenuItem});
            this.sesionToolStripMenuItem.Name = "sesionToolStripMenuItem";
            this.sesionToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.sesionToolStripMenuItem.Text = "Sesion";
            // 
            // iniciarSesionToolStripMenuItem
            // 
            this.iniciarSesionToolStripMenuItem.Name = "iniciarSesionToolStripMenuItem";
            this.iniciarSesionToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.iniciarSesionToolStripMenuItem.Text = "Iniciar Sesion";
            this.iniciarSesionToolStripMenuItem.Click += new System.EventHandler(this.iniciarSesionToolStripMenuItem_Click);
            // 
            // cerrarSesionToolStripMenuItem
            // 
            this.cerrarSesionToolStripMenuItem.Name = "cerrarSesionToolStripMenuItem";
            this.cerrarSesionToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.cerrarSesionToolStripMenuItem.Text = "Cerrar Sesion";
            this.cerrarSesionToolStripMenuItem.Click += new System.EventHandler(this.cerrarSesionToolStripMenuItem_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.menuPrincipal);
            this.pnlMain.Location = new System.Drawing.Point(-2, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(884, 639);
            this.pnlMain.TabIndex = 8;
            this.pnlMain.Resize += new System.EventHandler(this.pnlMain_Resize);
            // 
            // FrmPrincipal
            // 
            this.ClientSize = new System.Drawing.Size(882, 639);
            this.Controls.Add(this.pnlNavBar);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuPrincipal;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.FrmPrincipal_Shown);
            this.Resize += new System.EventHandler(this.FrmPrincipal_Resize);
            this.pnlNavBar.ResumeLayout(false);
            this.pnlNavBar.PerformLayout();
            this.menuStripSidePanel.ResumeLayout(false);
            this.menuStripSidePanel.PerformLayout();
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblIdioma;
        private System.Windows.Forms.ComboBox cboIdiomas;
        private System.Windows.Forms.Panel pnlNavBar;
        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem sesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iniciarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cerrarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizeToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStripSidePanel;
        private System.Windows.Forms.ToolStripMenuItem xSidePanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tuMayoristaToolStripMenuItem;
    }
}

