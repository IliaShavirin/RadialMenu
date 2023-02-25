using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BaseProj
{
    /// <summary>
    ///     Represents a control that displays hierarchical data in a tree structure
    ///     that has items that can expand and collapse.
    /// </summary>
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty CanResizeColumnsProperty = DependencyProperty.Register(
            "CanResizeColumns", typeof(bool), typeof(TreeListView), new PropertyMetadata(default(bool)));

        static TreeListView()
        {
            //Override the default style and the default control template
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView),
                new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        /// <summary>
        ///     Initialize a new instance of TreeListView.
        /// </summary>
        public TreeListView()
        {
            Columns = new GridViewColumnCollection();

            Loaded += OnLoaded;
        }

        public bool CanResizeColumns
        {
            get => (bool)GetValue(CanResizeColumnsProperty);
            set => SetValue(CanResizeColumnsProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!CanResizeColumns)
                foreach (var column in Columns)
                    if (column.Header as GridViewColumnHeader != null)
                    {
                        var thumb = ((GridViewColumnHeader)column.Header).Template.FindName("PART_HeaderGripper",
                            (FrameworkElement)column.Header) as Thumb;
                        if (thumb != null) thumb.Visibility = Visibility.Collapsed;
                    }
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the collection of System.Windows.Controls.GridViewColumn
        ///     objects that is defined for this TreeListView.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get => (GridViewColumnCollection)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        ///     Gets or sets whether columns in a TreeListView can be
        ///     reordered by a drag-and-drop operation. This is a dependency property.
        /// </summary>
        public bool AllowsColumnReorder
        {
            get => (bool)GetValue(AllowsColumnReorderProperty);
            set => SetValue(AllowsColumnReorderProperty, value);
        }

        #endregion

        #region Static Dependency Properties

        // Using a DependencyProperty as the backing store for AllowsColumnReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeListView),
                new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection),
                typeof(TreeListView),
                new UIPropertyMetadata(null));

        #endregion
    }
}