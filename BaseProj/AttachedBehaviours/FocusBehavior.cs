using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaseProj.AttachedBehaviours
{
    public static class FocusBehavior
    {
        public static readonly DependencyProperty FocusFirstProperty =
            DependencyProperty.RegisterAttached(
                "FocusFirst",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusFirstPropertyChanged));

        public static readonly DependencyProperty InitialyFocusedProperty = DependencyProperty.RegisterAttached(
            "InitialyFocused", typeof(bool), typeof(FocusBehavior),
            new PropertyMetadata(default(bool), OnInitialyFocusedChanged));

        public static bool GetFocusFirst(Control control)
        {
            return (bool)control.GetValue(FocusFirstProperty);
        }

        public static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        private static void OnFocusFirstPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = obj as Control;
            if (control == null || !(args.NewValue is bool)) return;

            if ((bool)args.NewValue)
                control.Loaded += (sender, e) =>
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public static void SetInitialyFocused(DependencyObject element, bool value)
        {
            element.SetValue(InitialyFocusedProperty, value);
        }

        public static bool GetInitialyFocused(DependencyObject element)
        {
            return (bool)element.GetValue(InitialyFocusedProperty);
        }

        private static void OnInitialyFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as Control;
            if (control == null || !control.Focusable || !(args.NewValue is bool)) return;

            if ((bool)args.NewValue)
                control.Loaded += (sender, e) =>
                    control.Focus();
        }
    }
}