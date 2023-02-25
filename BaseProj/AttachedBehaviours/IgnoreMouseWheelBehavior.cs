using System.Windows;
using System.Windows.Input;

namespace BaseProj.AttachedBehaviours
{
    public sealed class IgnoreMouseWheelBehavior
    {
        public static readonly DependencyProperty IsIgnoreMouseWheelProperty = DependencyProperty.RegisterAttached(
            "IsIgnoreMouseWheel", typeof(bool), typeof(IgnoreMouseWheelBehavior),
            new PropertyMetadata(default(bool), OnIsIgnoreMouseWheelChanged));

        private static void OnIsIgnoreMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as UIElement;
            if (fe == null) return;

            if ((bool)e.NewValue)
                fe.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
            else
                fe.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        }

        public static void SetIsIgnoreMouseWheel(DependencyObject element, bool value)
        {
            element.SetValue(IsIgnoreMouseWheelProperty, value);
        }

        public static bool GetIsIgnoreMouseWheel(DependencyObject element)
        {
            return (bool)element.GetValue(IsIgnoreMouseWheelProperty);
        }

        private static void AssociatedObject_PreviewMouseWheel(object s, MouseWheelEventArgs e)
        {
            var fe = (UIElement)s;
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;

            fe.RaiseEvent(e2);
        }
    }
}