using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BaseProj.AttachedBehaviours
{
    public static class UniformGridRecalculateRowsBehaviour
    {
        public static readonly DependencyProperty RecalculateRowsProperty = DependencyProperty.RegisterAttached(
            "RecalculateRows", typeof(bool), typeof(UniformGridRecalculateRowsBehaviour),
            new PropertyMetadata(default(bool), OnRemoveEmptyRowsChanged));

        public static void SetRecalculateRows(DependencyObject element, bool value)
        {
            element.SetValue(RecalculateRowsProperty, value);
        }

        public static bool GetRecalculateRows(DependencyObject element)
        {
            return (bool)element.GetValue(RecalculateRowsProperty);
        }

        private static void OnRemoveEmptyRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UniformGrid grid)) return;

            grid.Loaded += GridOnLoaded;
        }

        private static void GridOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is UniformGrid grid)) return;

            foreach (Control child in grid.Children) child.IsVisibleChanged += OnChildVisibilityChanged;

            UpdateRowsVisibility(grid);
        }

        private static void OnChildVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Control child) UpdateRowsVisibility(child.Parent as UniformGrid);
        }

        public static void UpdateRowsVisibility(UniformGrid grid)
        {
            var visibileChildrenCount = 0;

            foreach (UIElement child in grid.Children)
                if (child.Visibility == Visibility.Visible)
                    visibileChildrenCount++;

            var leftover = visibileChildrenCount % grid.Columns;
            var totalRows = visibileChildrenCount / grid.Columns;
            if (leftover > 0) totalRows++;
            grid.Rows = totalRows;
        }
    }
}