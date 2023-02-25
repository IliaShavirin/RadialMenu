using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace BaseProj.ListViewLayoutManager
{
    // ------------------------------------------------------------------------
    public class TreeViewLayoutManager
    {
        private const double zeroWidthRange = 0.1;

        // ----------------------------------------------------------------------
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
            typeof(bool),
            typeof(TreeViewLayoutManager),
            new FrameworkPropertyMetadata(OnLayoutManagerEnabledChanged));

        // ----------------------------------------------------------------------
        // members
        private GridViewColumn autoSizedColumn;
        private bool loaded;
        private Cursor resizeCursor;
        private bool resizing;
        private ScrollViewer scrollViewer;

        // ----------------------------------------------------------------------
        public TreeViewLayoutManager(TreeListView treeView)
        {
            if (treeView == null) throw new ArgumentNullException("treeView");

            TreeView = treeView;
            TreeView.Loaded += TreeViewLoaded;
            TreeView.Unloaded += TreeViewUnloaded;
        } // TreeViewLayoutManager

        // ----------------------------------------------------------------------
        public TreeListView TreeView { get; }

        // ----------------------------------------------------------------------
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; } =
            ScrollBarVisibility.Auto; // VerticalScrollBarVisibility

        // ----------------------------------------------------------------------
        public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(EnabledProperty, enabled);
        } // SetEnabled

        // ----------------------------------------------------------------------
        public void Refresh()
        {
            InitColumns();
            DoResizeColumns();
        } // Refresh

        // ----------------------------------------------------------------------
        private void RegisterEvents(DependencyObject start)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    var gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        var thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                            FixedColumn.IsFixedColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = false;
                        }
                        else
                        {
                            thumb.PreviewMouseMove += ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown += ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(
                                GridViewColumn.WidthProperty,
                                typeof(GridViewColumn)).AddValueChanged(gridViewColumn, GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    var columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged += GridColumnHeaderSizeChanged;
                }
                else if (scrollViewer == null && childVisual is ScrollViewer)
                {
                    scrollViewer = childVisual as ScrollViewer;
                    scrollViewer.ScrollChanged += ScrollViewerScrollChanged;
                    // assume we do the regulation of the horizontal scrollbar
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scrollViewer.VerticalScrollBarVisibility = VerticalScrollBarVisibility;
                }

                RegisterEvents(childVisual); // recursive
            }
        } // RegisterEvents

        // ----------------------------------------------------------------------
        private void UnregisterEvents(DependencyObject start)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    var gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        var thumb = childVisual as Thumb;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                            FixedColumn.IsFixedColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = true;
                        }
                        else
                        {
                            thumb.PreviewMouseMove -= ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown -= ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(
                                GridViewColumn.WidthProperty,
                                typeof(GridViewColumn)).RemoveValueChanged(gridViewColumn, GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader)
                {
                    var columnHeader = childVisual as GridViewColumnHeader;
                    columnHeader.SizeChanged -= GridColumnHeaderSizeChanged;
                }
                else if (scrollViewer == null && childVisual is ScrollViewer)
                {
                    scrollViewer = childVisual as ScrollViewer;
                    scrollViewer.ScrollChanged -= ScrollViewerScrollChanged;
                }

                UnregisterEvents(childVisual); // recursive
            }
        } // UnregisterEvents

        // ----------------------------------------------------------------------
        private GridViewColumn FindParentColumn(DependencyObject element)
        {
            if (element == null) return null;

            while (element != null)
            {
                var gridViewColumnHeader = element as GridViewColumnHeader;
                if (gridViewColumnHeader != null) return gridViewColumnHeader.Column;
                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        } // FindParentColumn

        // ----------------------------------------------------------------------
        private GridViewColumnHeader FindColumnHeader(DependencyObject start, GridViewColumn gridViewColumn)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is GridViewColumnHeader)
                {
                    var gridViewHeader = childVisual as GridViewColumnHeader;
                    if (gridViewHeader.Column == gridViewColumn) return gridViewHeader;
                }

                var childGridViewHeader = FindColumnHeader(childVisual, gridViewColumn); // recursive
                if (childGridViewHeader != null) return childGridViewHeader;
            }

            return null;
        } // FindColumnHeader

        // ----------------------------------------------------------------------
        private void InitColumns()
        {
            foreach (var gridViewColumn in TreeView.Columns)
            {
                if (!RangeColumn.IsRangeColumn(gridViewColumn)) continue;

                var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);
                if (!minWidth.HasValue && !maxWidth.HasValue) continue;

                var columnHeader = FindColumnHeader(TreeView, gridViewColumn);
                if (columnHeader == null) continue;

                var actualWidth = columnHeader.Column.ActualWidth;
                if (minWidth.HasValue)
                {
                    columnHeader.MinWidth = minWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth < columnHeader.MinWidth)
                        gridViewColumn.Width = columnHeader.MinWidth;
                }

                if (maxWidth.HasValue)
                {
                    columnHeader.MaxWidth = maxWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth > columnHeader.MaxWidth)
                        gridViewColumn.Width = columnHeader.MaxWidth;
                }

                //GridViewColumnHeader columnHeader = FindColumnHeader( treeView, gridViewColumn );
                //if ( columnHeader == null )
                //{
                //    continue;
                //}

                //double actualWidth = columnHeader.ActualWidth;
                //if ( minWidth.HasValue )
                //{
                //    columnHeader.MinWidth = minWidth.Value;
                //    if ( !double.IsInfinity( actualWidth ) && actualWidth < columnHeader.MinWidth )
                //    {
                //        gridViewColumn.Width = columnHeader.MinWidth;
                //    }
                //}
                //if ( maxWidth.HasValue )
                //{
                //    columnHeader.MaxWidth = maxWidth.Value;
                //    if ( !double.IsInfinity( actualWidth ) && actualWidth > columnHeader.MaxWidth )
                //    {
                //        gridViewColumn.Width = columnHeader.MaxWidth;
                //    }
                //}
            }
        } // InitColumns

        // ----------------------------------------------------------------------
        protected virtual void ResizeColumns()
        {
            // treeview width
            var actualWidth = double.PositiveInfinity;
            if (scrollViewer != null) actualWidth = scrollViewer.ViewportWidth;
            if (double.IsInfinity(actualWidth)) actualWidth = TreeView.ActualWidth;
            if (double.IsInfinity(actualWidth) || actualWidth <= 0) return;

            double resizeableRegionCount = 0;
            double otherColumnsWidth = 0;
            // determine column sizes
            foreach (var gridViewColumn in TreeView.Columns)
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    var proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null) resizeableRegionCount += proportionalWidth.Value;
                }
                else
                {
                    otherColumnsWidth += gridViewColumn.ActualWidth;
                }

            if (resizeableRegionCount <= 0)
            {
                // no proportional columns present: commit the regulation to the scroll viewer
                if (scrollViewer != null) scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                // search the first fill column
                GridViewColumn fillColumn = null;
                for (var i = 0; i < TreeView.Columns.Count; i++)
                {
                    var gridViewColumn = TreeView.Columns[i];
                    if (IsFillColumn(gridViewColumn))
                    {
                        fillColumn = gridViewColumn;
                        break;
                    }
                }

                if (fillColumn != null)
                {
                    var otherColumnsWithoutFillWidth = otherColumnsWidth - fillColumn.ActualWidth;
                    var fillWidth = actualWidth - otherColumnsWithoutFillWidth;
                    if (fillWidth > 0 && fillWidth > fillColumn.ActualWidth)
                    {
                        var minWidth = RangeColumn.GetRangeMinWidth(fillColumn);
                        var maxWidth = RangeColumn.GetRangeMaxWidth(fillColumn);

                        var setWidth = !(minWidth.HasValue && fillWidth < minWidth.Value);
                        if (maxWidth.HasValue && fillWidth > maxWidth.Value) setWidth = false;
                        if (setWidth)
                        {
                            if (scrollViewer != null)
                                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            fillColumn.Width = fillWidth - 1;
                        }
                    }
                }

                return;
            }

            var resizeableColumnsWidth = actualWidth - otherColumnsWidth;
            if (resizeableColumnsWidth <= 0) return; // missing space

            // resize columns
            var resizeableRegionWidth = resizeableColumnsWidth / resizeableRegionCount;
            foreach (var gridViewColumn in TreeView.Columns)
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    var proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null)
                        gridViewColumn.Width = proportionalWidth.Value * resizeableRegionWidth;
                }
        } // ResizeColumns

        // ----------------------------------------------------------------------
        // returns the delta
        private double SetRangeColumnToBounds(GridViewColumn gridViewColumn)
        {
            var startWidth = gridViewColumn.Width;

            var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
            var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

            if (minWidth.HasValue && maxWidth.HasValue && minWidth > maxWidth) return 0; // invalid case

            if (minWidth.HasValue && gridViewColumn.Width < minWidth.Value)
                gridViewColumn.Width = minWidth.Value;
            else if (maxWidth.HasValue && gridViewColumn.Width > maxWidth.Value) gridViewColumn.Width = maxWidth.Value;

            return gridViewColumn.Width - startWidth;
        } // SetRangeColumnToBounds

        // ----------------------------------------------------------------------
        private bool IsFillColumn(GridViewColumn gridViewColumn)
        {
            if (gridViewColumn == null) return false;

            if (TreeView.Columns.Count == 0) return false;

            var isFillColumn = RangeColumn.GetRangeIsFillColumn(gridViewColumn);
            return isFillColumn.HasValue && isFillColumn.Value;
        } // IsFillColumn

        // ----------------------------------------------------------------------
        private void DoResizeColumns()
        {
            if (resizing) return;

            resizing = true;
            try
            {
                ResizeColumns();
            }
            finally
            {
                resizing = false;
            }
        } // DoResizeColumns

        // ----------------------------------------------------------------------
        private void TreeViewLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(TreeView);
            InitColumns();
            DoResizeColumns();
            loaded = true;
        } // TreeViewLoaded

        // ----------------------------------------------------------------------
        private void TreeViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            UnregisterEvents(TreeView);
            loaded = false;
        } // TreeViewUnloaded

        // ----------------------------------------------------------------------
        private void ThumbPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null) return;
            var gridViewColumn = FindParentColumn(thumb);
            if (gridViewColumn == null) return;

            // suppress column resizing for proportional and fixed
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                FixedColumn.IsFixedColumn(gridViewColumn))
            {
                thumb.Cursor = null;
                return;
            }

            // check range column bounds
            if (thumb.IsMouseCaptured && RangeColumn.IsRangeColumn(gridViewColumn))
            {
                var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

                if (minWidth.HasValue && maxWidth.HasValue && minWidth > maxWidth) return; // invalid case

                if (resizeCursor == null) resizeCursor = thumb.Cursor; // save the resize cursor

                if (minWidth.HasValue && gridViewColumn.Width <= minWidth.Value)
                    thumb.Cursor = Cursors.No;
                else if (maxWidth.HasValue && gridViewColumn.Width >= maxWidth.Value)
                    thumb.Cursor = Cursors.No;
                else
                    thumb.Cursor = resizeCursor; // between valid min/max
            }
        } // ThumbPreviewMouseMove

        // ----------------------------------------------------------------------
        private void ThumbPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var thumb = sender as Thumb;
            var gridViewColumn = FindParentColumn(thumb);

            // suppress column resizing for proportional, fixed and range fill columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                FixedColumn.IsFixedColumn(gridViewColumn))
                e.Handled = true;
        } // ThumbPreviewMouseLeftButtonDown

        // ----------------------------------------------------------------------
        private void GridColumnWidthChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            var gridViewColumn = sender as GridViewColumn;

            // suppress column resizing for proportional and fixed columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) ||
                FixedColumn.IsFixedColumn(gridViewColumn)) return;

            // ensure range column within the bounds
            if (RangeColumn.IsRangeColumn(gridViewColumn))
            {
                // special case: auto column width - maybe conflicts with min/max range
                if (gridViewColumn != null && gridViewColumn.Width.Equals(double.NaN))
                {
                    autoSizedColumn = gridViewColumn;
                    return; // handled by the change header size event
                }

                // ensure column bounds
                if (Math.Abs(SetRangeColumnToBounds(gridViewColumn) - 0) > zeroWidthRange) return;
            }

            DoResizeColumns();
        } // GridColumnWidthChanged

        // ----------------------------------------------------------------------
        // handle autosized column
        private void GridColumnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (autoSizedColumn == null) return;

            var gridViewColumnHeader = sender as GridViewColumnHeader;
            if (gridViewColumnHeader != null && gridViewColumnHeader.Column == autoSizedColumn)
            {
                if (gridViewColumnHeader.Width.Equals(double.NaN))
                {
                    // sync column with 
                    gridViewColumnHeader.Column.Width = gridViewColumnHeader.ActualWidth;
                    DoResizeColumns();
                }

                autoSizedColumn = null;
            }
        } // GridColumnHeaderSizeChanged

        // ----------------------------------------------------------------------
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (loaded && Math.Abs(e.ViewportWidthChange - 0) > zeroWidthRange) DoResizeColumns();
        } // ScrollViewerScrollChanged

        // ----------------------------------------------------------------------
        private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var treeView = dependencyObject as TreeListView;
            if (treeView != null)
            {
                var enabled = (bool)e.NewValue;
                if (enabled) new TreeViewLayoutManager(treeView);
            }
        } // OnLayoutManagerEnabledChanged
    } // class TreeViewLayoutManager
}