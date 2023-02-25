using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BaseProj.AttachedBehaviours
{
    public static class LeftClickContextMenuBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(LeftClickContextMenuBehavior),
            new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement != null)
            {
                var IsEnabled = e.NewValue is bool && (bool)e.NewValue;

                if (IsEnabled)
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click += OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;

                    uiElement.PreviewMouseRightButtonUp += OnMouseRightButtonUp;
                }
                else
                {
                    if (uiElement is ButtonBase)
                        ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;

                    uiElement.PreviewMouseRightButtonUp -= OnMouseRightButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            if (fe != null)
            {
                // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
                // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control, although I'm not sure)
                // so we have to set up ContextMenu.DataContext manually here

                fe.ContextMenu.PlacementTarget = fe;

                if (fe.ContextMenu.DataContext == null)
                    fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty,
                        new Binding { Source = fe, Path = new PropertyPath("DataContext"), Mode = BindingMode.TwoWay });

                fe.ContextMenu.IsOpen = true;
            }
        }

        private static void OnMouseRightButtonUp(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            if (fe != null) e.Handled = true;
        }
    }
}