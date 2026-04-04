using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using IngSoft.Abstractions.Multidioma;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.Domain;
using IngSoft.Domain.Multidioma;
using IngSoft.Services;
using IngSoft.UI.Login;
using IngSoft.UI.Multidioma;
using IngSoft.UI.BitacoraUI;

namespace IngSoft.UI
{
    public partial class FrmPrincipal : Form, IObserver
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IMultidiomaServices _multidiomaServices;
        internal EventHandler LastAction;

        // Constants for custom caption & resize
        private const int cCaption = 32;   // Caption bar height
        private const int resizeArea = 80;  // Area in pixels sensitive to resize

        // Animation fields
        private bool _isPanelOpen = false;
        private readonly int _animationStep = 1; // pixels per iteration
        private readonly int _animationDelay = 1; // ms delay between iterations
        private bool _midAnimation = false;
        
        internal Panel GetPanelMain
        {
             get { return pnlMain; }
        }

        // New fields for smooth animations
        private CancellationTokenSource _animationCts;
        private readonly int _animationDurationMs = 350; // default duration for smooth animation

        public FrmPrincipal()
        {
            InitializeComponent();

            // Keep form borderless but enable smooth resizing/draggable caption
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            _usuarioServices = ServicesFactory.CreateUsuarioServices();
            _multidiomaServices = ServicesFactory.CreateMultidiomaServices();

            SingleInstancesManager.Instance.AgregarObjeto(_usuarioServices);
            SingleInstancesManager.Instance.AgregarObjeto(ServicesFactory.CreateBitacoraServices());

            pnlNavBar.HorizontalScroll.Maximum = 0;
            pnlNavBar.HorizontalScroll.Enabled = false;
            pnlNavBar.AutoScroll = false;
            pnlNavBar.VerticalScroll.Visible = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // paint caption area
            var rcCaption = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            using (var brush = new SolidBrush(Color.DarkBlue))
            {
                e.Graphics.FillRectangle(brush, rcCaption);
            }

            // draw resize grip hint in lower-right
            var gripRect = new Rectangle(this.ClientSize.Width - resizeArea, this.ClientSize.Height - resizeArea, resizeArea, resizeArea);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, gripRect);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);

                // top caption area (drag)
                if (clientPoint.Y >= 0 && clientPoint.Y < cCaption)
                {
                    m.Result = (IntPtr)HTCAPTION;
                    return;
                }

                bool left = clientPoint.X <= resizeArea;
                bool right = clientPoint.X >= this.ClientSize.Width - resizeArea;
                bool top = clientPoint.Y <= resizeArea;
                bool bottom = clientPoint.Y >= this.ClientSize.Height - resizeArea;

                if (left && top) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (right && top) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (left && bottom) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (right && bottom) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (top) { m.Result = (IntPtr)HTTOP; return; }
                if (bottom) { m.Result = (IntPtr)HTBOTTOM; return; }
                if (left) { m.Result = (IntPtr)HTLEFT; return; }
                if (right) { m.Result = (IntPtr)HTRIGHT; return; }

                m.Result = (IntPtr)HTCLIENT;
                return;
            }

            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            CargarIdiomas();
            EstablecerIdiomaPorDefecto();
            SingleInstancesManager.Instance.AgregarObjeto(this);
        }

        /// <summary>
        /// Instead of opening FrmLogin as a modal dialog, the login UI is now
        /// injected directly into pnlMain via FrmLoginHeaderConfig / FrmLoginFlexibilizador.
        /// Post-login side-effects (menu & side-panel update) are triggered by
        /// FrmLoginHeaderConfig through its OnLoginSuccess callback.
        /// </summary>
        private void iniciarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Constructing FrmLoginHeaderConfig is enough:
            // it clears pnlMain, injects the login controls, and wires the success callback.
            new FrmLoginHeaderConfig();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogOutUser();
            FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
            FrmPrincipalFlexibilizador.ActualizarPanelVerticalSegunEstadoSesion();
        }

        private void LogOutUser()
        {
            _usuarioServices.LogOutUser();
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SessionManager.GetInstance().IsLoggedIn())
            {
                LogOutUser();
            }
        }

        private void menuPrincipal_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void controlDeCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var FrmControlCambios = new FrmControlDeCambios();
            FrmControlCambios.ShowDialog();
        }

        private void multidiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmMultidioma = new FrmMultidiomas();
            if (frmMultidioma.ShowDialog() == DialogResult.OK)
            {
                CargarIdiomas();
            }
        }

        private void cboIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboIdiomas.SelectedItem is Idioma))
                return;

            var idiomaSeleccionado = MultidiomaManager.ObtenerIdiomaCache((Idioma)cboIdiomas.SelectedItem);

            if (MultidiomaManager.GetIdioma() != null) 
            {
                MultidiomaManager.GetIdioma().Desuscribir(this);
            }

            MultidiomaManager.SetIdioma(idiomaSeleccionado);
            idiomaSeleccionado.Suscribir(this);
            idiomaSeleccionado.NotificarObservers();
        }

        private void CargarIdiomas()
        {
            var idiomas = _multidiomaServices.ObtenerIdiomas()
                .Cast<IIdioma>().ToList();

            var idiomasCacheados = MultidiomaManager.ObtenerIdiomasCache(idiomas);

            cboIdiomas.DataSource = idiomasCacheados;
            cboIdiomas.DisplayMember = "Codigo";
            cboIdiomas.ValueMember = null;
        }

        private void EstablecerIdiomaPorDefecto()
        {
            var idiomaPorDefecto = (IIdioma)_multidiomaServices.ObtenerIdiomaPorDefecto();
            if (idiomaPorDefecto == null) return;

            idiomaPorDefecto = MultidiomaManager.ObtenerIdiomaCache(idiomaPorDefecto);

            cboIdiomas.SelectedItem = idiomaPorDefecto;
            MultidiomaManager.SetIdioma(idiomaPorDefecto);
            idiomaPorDefecto.Suscribir(this);

            var controles = _multidiomaServices.ObtenerControlesPorIdioma(idiomaPorDefecto.Id)
                .Cast<IControlIdioma>().ToList();
            MultidiomaManager.CambiarIdiomaControles(this, controles);
        }

        internal void AplicarIdiomaActual()
        {
            // Aplicar el idioma actual al formulario
            if (MultidiomaManager.GetIdioma() != null)
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }

        public void Actualizar()
        {
            if (MultidiomaManager.GetIdioma() != null) 
            {
                var controles = _multidiomaServices.ObtenerControlesPorIdioma(MultidiomaManager.GetIdioma().Id)
                    .Cast<IControlIdioma>().ToList();
                MultidiomaManager.CambiarIdiomaControles(this, controles);
            }
        }        

        private void permisosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPermiso frmPermiso = new FrmPermiso();
            frmPermiso.ShowDialog();
        }

        private void FrmPrincipal_Shown(object sender, EventArgs e)
        {
            FrmPrincipalFlexibilizador.ActualizarMenuSegunEstadoSesion();
        }

        private void backupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmBackUp = new FrmBackUp();
            frmBackUp.ShowDialog();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Al hacer click en el icono de menú, abrir el panel lateral
            AbrirPanelLateralSuave();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Al hacer click en el botón de cierre dentro del panel, cerrarlo con animación
            CerrarPanelLateralSuave();
        }

        // Cierra el panel lateral con una animación horizontal hasta ocultarlo totalmente
        internal async void CerrarPanelLateral()
        {
            if (pnlNavBar == null && !_isPanelOpen) return;
            try
            {
                int target = -pnlNavBar.Width;
                while (pnlNavBar.Left > target)
                {
                    int next = pnlNavBar.Left - _animationStep;
                    if (next < target) next = target;
                    pnlNavBar.Left = next;
                    pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                    pnlMain.Width += _animationStep;
                    await System.Threading.Tasks.Task.Delay(_animationDelay);
                }

                pnlNavBar.Left = target;
                pnlMain.Width = this.Width;
                pnlMain.Width -= (this.Width - pnlMain.Right);
                _isPanelOpen = false;
            }
            catch (Exception)
            {
                pnlNavBar.Left = -pnlNavBar.Width;
                _isPanelOpen = false;
            }
        }

        // Abre el panel lateral con una animación horizontal hasta posicionarlo totalmente visible
        internal async void AbrirPanelLateral()
        {
            if (pnlNavBar == null && _isPanelOpen) return;

            try
            {
                int target = 0;
                while (pnlNavBar.Left < target)
                {
                    int next = pnlNavBar.Left + _animationStep;
                    if (next > target) next = target;
                    pnlNavBar.Left = next;
                    pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                    pnlMain.Width = pnlMain.Width - _animationStep;
                    await System.Threading.Tasks.Task.Delay(_animationDelay);
                }
                pnlNavBar.Left = target;
                pnlMain.Width = this.Width - pnlNavBar.Width;
                pnlMain.Width -= (this.Width - pnlMain.Right);
                _isPanelOpen = true;
            }
            catch (Exception)
            {
                pnlNavBar.Left = 0;
                _isPanelOpen = true;
            }
        }

        // Smooth animation helpers (new) - do not remove the older methods above
        private double EaseOutCubic(double t)
        {
            // t in [0,1]
            return 1 - Math.Pow(1 - t, 3);
        }

        // New: smooth close animation using time-based easing and cancellation support
        internal async Task CerrarPanelLateralSuave(int durationMs = -1)
        {
            if (pnlNavBar == null) return;
            if (durationMs <= 0) durationMs = _animationDurationMs;

            _animationCts?.Cancel();
            _animationCts = new CancellationTokenSource();
            var ct = _animationCts.Token;

            int startLeft = pnlNavBar.Left;
            int endLeft = -pnlNavBar.Width;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                while (pnlNavBar.Left > endLeft)
                {
                    ct.ThrowIfCancellationRequested();

                    double elapsed = Math.Min(sw.ElapsedMilliseconds, durationMs);
                    double t = elapsed / (double)durationMs;
                    double eased = EaseOutCubic(t);

                    int nextLeft = (int)Math.Round(startLeft + (endLeft - startLeft) * eased);

                    pnlNavBar.Left = nextLeft;
                    pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                    pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);

                    if (elapsed >= durationMs) break;

                    await Task.Delay(16, ct); // ~60fps
                }

                pnlNavBar.Left = endLeft;
                pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);
                _isPanelOpen = false;
            }
            catch (OperationCanceledException)
            {
                // animation cancelled - leave controls in their current state
            }
            catch (Exception)
            {
                // on any error, ensure panel is fully closed
                pnlNavBar.Left = -pnlNavBar.Width;
                pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);
                _isPanelOpen = false;
            }
            finally
            {
                _animationCts?.Dispose();
                _animationCts = null;
            }
        }

        // New: smooth open animation using time-based easing and cancellation support
        internal async Task AbrirPanelLateralSuave(int durationMs = -1)
        {
            if (pnlNavBar == null) return;
            if (durationMs <= 0) durationMs = _animationDurationMs;

            _animationCts?.Cancel();
            _animationCts = new CancellationTokenSource();
            var ct = _animationCts.Token;

            int startLeft = pnlNavBar.Left;
            int endLeft = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                while (pnlNavBar.Left < endLeft)
                {
                    ct.ThrowIfCancellationRequested();

                    double elapsed = Math.Min(sw.ElapsedMilliseconds, durationMs);
                    double t = elapsed / (double)durationMs;
                    double eased = EaseOutCubic(t);

                    int nextLeft = (int)Math.Round(startLeft + (endLeft - startLeft) * eased);

                    pnlNavBar.Left = nextLeft;
                    pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                    pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);

                    if (elapsed >= durationMs) break;

                    await Task.Delay(16, ct); // ~60fps
                }

                pnlNavBar.Left = endLeft;
                pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);
                _isPanelOpen = true;
            }
            catch (OperationCanceledException)
            {
                // animation cancelled
            }
            catch (Exception)
            {
                pnlNavBar.Left = 0;
                pnlMain.Left = pnlNavBar.Left + pnlNavBar.Width;
                pnlMain.Width = Math.Max(0, this.ClientSize.Width - pnlMain.Left);
                _isPanelOpen = true;
            }
            finally
            {
                _animationCts?.Dispose();
                _animationCts = null;
            }
        }

        internal void Home(object sender, EventArgs e)
        {
            Label lblUser = new Label();
            lblUser.Name = "label1";
            lblUser.Location = new System.Drawing.Point(150, 211);
            Usuario usuario = SessionManager.GetUsuario() as Usuario;
            if (usuario != null)
            {
                lblUser.Text = $"{usuario.Nombre} {usuario.Apellido}";
                lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lblUser.AutoSize = true;
            }
            FlexibilizadorFormularios.EliminarControlesAdicionalesForm(this.GetPanelMain, ControlesSalvar());

            pnlMain.Controls.Add(lblUser);
            LastAction = Home;
        }

        internal Dictionary<Control, Control> ControlesSalvar()
        {
            Dictionary<Control, Control> controles = new Dictionary<Control, Control>();
                
            if (this.MainMenuStrip != null)
            {
                controles.Add(this.MainMenuStrip, this.GetPanelMain);
            }
            return controles;
        }

        internal Dictionary<Control, Control> ControlesSalvarSidePanel()
        {
            Dictionary<Control, Control> controlesIdiomas = new Dictionary<Control, Control>();

            controlesIdiomas.Add(lblIdioma, lblIdioma.Parent);
            controlesIdiomas.Add(cboIdiomas, cboIdiomas.Parent);
            controlesIdiomas.Add(menuStripSidePanel, pnlNavBar);
            return controlesIdiomas;
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void maximizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal; 
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void xSidePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CerrarPanelLateralSuave();
        }

        private void tuMayoristaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Home(sender, e);
        }

        private void Resizing()
        {
            pnlMain.Height = this.Height;
            pnlMain.Width = this.Width - pnlNavBar.Right;
            pnlNavBar.Height = this.Height;
        }

        private async void FrmPrincipal_Resize(object sender, EventArgs e)
        {
            this.Resizing();
            try
            {
                await System.Threading.Tasks.Task.Delay(20);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void menuPrincipal_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        // EventArgs indicating the LastAction was triggered by a resize
        internal class ResizeActionEventArgs : EventArgs
        {
            public bool IsResize { get; }
            public ResizeActionEventArgs(bool isResize)
            {
                IsResize = isResize;
            }
        }

        private async void pnlMain_Resize(object sender, EventArgs e)
        {
            if (_midAnimation == false)
            {
                if (LastAction != null)
                {
                    // When resizing, call LastAction with a ResizeActionEventArgs indicating a resize
                    try
                    {
                        LastAction.Invoke(sender, new ResizeActionEventArgs(true));
                    }
                    catch (Exception)
                    {
                        // swallow exceptions from invoked handlers during resize
                    }

                    _midAnimation = true;
                    try
                    {
                        await System.Threading.Tasks.Task.Delay(200);
                        _midAnimation = false;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
        }

        internal bool IsResizing(EventArgs e)
        {
            bool isResize = false;
            if (e is FrmPrincipal.ResizeActionEventArgs resizeArgs)
            {
                isResize = resizeArgs.IsResize;
            }
            return isResize;
        }
    }
}
