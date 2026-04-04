using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IngSoft.UI.Pagination
{
    /// <summary>
    /// Decorator that wraps a DataGridView with a pagination control strip.
    ///
    /// Pattern: Decorator — DataGridViewPaginationDecorator holds a DataGridView
    /// (the wrapped component) and adds paging behaviour without modifying the
    /// grid class itself. Callers interact only with this decorator via
    /// SetDataSource / Resize / PageSize.
    ///
    /// Physical layout inside the parent:
    ///   ┌─────────────────────────────────┐
    ///   │         DataGridView            │  ← gridHeight = totalHeight - paginationBarHeight
    ///   ├─────────────────────────────────┤
    ///   │  [◄ Anterior]  Pág 1 / 3  [Siguiente ►]  │  ← fixed 30 px bar
    ///   └─────────────────────────────────┘
    /// </summary>
    public class DataGridViewPaginationDecorator
    {
        // ── Wrapped component ────────────────────────────────────────────────────
        private readonly DataGridView _grid;

        // ── Pagination state ─────────────────────────────────────────────────────
        private IList _fullDataSource;
        private int _currentPage  = 1;
        private int _pageSize     = 15;

        // ── Pagination bar controls ──────────────────────────────────────────────
        private readonly Panel  _paginationBar;
        private readonly Button _btnAnterior;
        private readonly Button _btnSiguiente;
        private readonly Label  _lblPagina;

        // ── Geometry kept for Resize ─────────────────────────────────────────────
        private Point _position;
        private Size  _totalSize;

        private const int BarHeight = 30;

        // ── Constructor ──────────────────────────────────────────────────────────

        /// <param name="grid">The DataGridView to decorate. Must already be created
        /// (e.g. via FlexibilizadorFormularios.CreateDataGridView) but not yet
        /// added to the parent — this decorator manages its placement.</param>
        /// <param name="parent">The control that owns both the grid and the bar.</param>
        /// <param name="position">Top-left of the combined (grid + bar) area.</param>
        /// <param name="totalSize">Total size of the combined area.</param>
        /// <param name="pageSize">Rows per page (default 15).</param>
        public DataGridViewPaginationDecorator(DataGridView grid, Control parent,
            Point position, Size totalSize, int pageSize = 15)
        {
            if (grid   == null) throw new ArgumentNullException(nameof(grid));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            _grid     = grid;
            _pageSize = pageSize;
            _position = position;
            _totalSize = totalSize;

            // ── Size the grid to leave room for the bar ──────────────────────────
            ApplyGeometry(position, totalSize);

            // ── Add grid to parent (CreateDataGridView already removed any old one)
            if (!parent.Controls.Contains(_grid))
                parent.Controls.Add(_grid);

            // ── Build the pagination bar ─────────────────────────────────────────
            _paginationBar = new Panel
            {
                Name     = _grid.Name + "_paginationBar",
                Location = new Point(position.X, position.Y + totalSize.Height - BarHeight),
                Size     = new Size(totalSize.Width, BarHeight),
                Visible  = true
            };

            _btnAnterior = new Button
            {
                Name     = _grid.Name + "_btnAnterior",
                Text     = "◄ Anterior",
                Dock     = DockStyle.Left,
                Width    = 100,
                Enabled  = false
            };
            _btnAnterior.Click += (s, e) => GoToPage(_currentPage - 1);

            _btnSiguiente = new Button
            {
                Name     = _grid.Name + "_btnSiguiente",
                Text     = "Siguiente ►",
                Dock     = DockStyle.Right,
                Width    = 100,
                Enabled  = false
            };
            _btnSiguiente.Click += (s, e) => GoToPage(_currentPage + 1);

            _lblPagina = new Label
            {
                Name      = _grid.Name + "_lblPagina",
                Text      = string.Empty,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Arial", 9)
            };

            _paginationBar.Controls.Add(_lblPagina);
            _paginationBar.Controls.Add(_btnAnterior);
            _paginationBar.Controls.Add(_btnSiguiente);

            parent.Controls.Add(_paginationBar);
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Replaces the data source and resets to page 1.
        /// Accepts any IList (List&lt;T&gt;, BindingList&lt;T&gt;, etc.).
        /// </summary>
        public void SetDataSource(IList data)
        {
            _fullDataSource = data;
            _currentPage    = 1;
            RefreshPage();
        }

        /// <summary>
        /// Rows per page. Setting this refreshes the current view.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                _pageSize = value;
                _currentPage = 1;
                RefreshPage();
            }
        }

        /// <summary>
        /// Call this whenever the parent panel is resized so the grid and bar
        /// reposition correctly without needing to recreate the decorator.
        /// </summary>
        public void Resize(Point position, Size totalSize)
        {
            _position  = position;
            _totalSize = totalSize;
            ApplyGeometry(position, totalSize);
            _paginationBar.Location = new Point(position.X, position.Y + totalSize.Height - BarHeight);
            _paginationBar.Width    = totalSize.Width;
        }

        // ── Internal paging logic ────────────────────────────────────────────────

        private int TotalPages =>
            _fullDataSource == null || _fullDataSource.Count == 0
                ? 1
                : (int)Math.Ceiling(_fullDataSource.Count / (double)_pageSize);

        private void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages) return;
            _currentPage = page;
            RefreshPage();
        }

        private void RefreshPage()
        {
            if (_fullDataSource == null)
            {
                _grid.DataSource = null;
                UpdatePaginationBar();
                return;
            }

            // Slice the page out of the full list using LINQ on the non-generic IList
            var pageItems = _fullDataSource
                .Cast<object>()
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            // Bind as a typed list so the DataGridView auto-generates columns correctly.
            // We use a BindingList wrapping the object list; columns were already built
            // from the first SetDataSource call so this is safe.
            _grid.DataSource = pageItems.Count > 0
                ? (object)BuildTypedList(pageItems)
                : null;

            UpdatePaginationBar();
        }

        /// <summary>
        /// Rebuilds a typed IList at runtime so DataGridView column inference works.
        /// Reflection is used once per page flip, which is acceptable for UI paging.
        /// </summary>
        private static IList BuildTypedList(List<object> items)
        {
            if (items.Count == 0) return new List<object>();

            var itemType  = items[0].GetType();
            var listType  = typeof(List<>).MakeGenericType(itemType);
            var typedList = (IList)Activator.CreateInstance(listType);
            foreach (var item in items)
                typedList.Add(item);
            return typedList;
        }

        private void UpdatePaginationBar()
        {
            int total = TotalPages;
            _lblPagina.Text       = $"Página {_currentPage} / {total}";
            _btnAnterior.Enabled  = _currentPage > 1;
            _btnSiguiente.Enabled = _currentPage < total;
        }

        private void ApplyGeometry(Point position, Size totalSize)
        {
            int gridHeight = Math.Max(0, totalSize.Height - BarHeight);
            _grid.Location = new Point(position.X, position.Y);
            _grid.Size     = new Size(totalSize.Width, gridHeight);
        }
    }
}
