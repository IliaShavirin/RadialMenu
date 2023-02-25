using System.Windows;

namespace BaseProj.AttachedBehaviours
{
    // https://stackoverflow.com/questions/23316274/inputbindings-work-only-when-focused
    // improved by artman to support window keybindings only if usercontrol is present on screen

    public class InputBindingBehavior
    {
        public static readonly DependencyProperty PropagateInputBindingsToWindowProperty =
            DependencyProperty.RegisterAttached("PropagateInputBindingsToWindow", typeof(bool),
                typeof(InputBindingBehavior),
                new PropertyMetadata(false, OnPropagateInputBindingsToWindowChanged));

        public static bool GetPropagateInputBindingsToWindow(FrameworkElement obj)
        {
            return (bool)obj.GetValue(PropagateInputBindingsToWindowProperty);
        }

        public static void SetPropagateInputBindingsToWindow(FrameworkElement obj, bool value)
        {
            obj.SetValue(PropagateInputBindingsToWindowProperty, value);
        }

        private static void OnPropagateInputBindingsToWindowChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)d).Loaded += frameworkElement_Loaded;
            ((FrameworkElement)d).Unloaded += frameworkElement_Unloaded;
        }

        private static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;

            var window = Window.GetWindow(frameworkElement);
            if (window == null) return;

            // Move input bindings from the FrameworkElement to the window.
            for (var i = frameworkElement.InputBindings.Count - 1; i >= 0; i--)
            {
                var inputBinding = frameworkElement.InputBindings[i];
                window.InputBindings.Add(inputBinding);
//                frameworkElement.InputBindings.Remove(inputBinding);
            }
        }

        private static void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;

            var window = Window.GetWindow(frameworkElement);
            if (window == null) return;

            for (var i = frameworkElement.InputBindings.Count - 1; i >= 0; i--)
            {
                var inputBinding = frameworkElement.InputBindings[i];
                if (window.InputBindings.Contains(inputBinding)) window.InputBindings.Remove(inputBinding);
//                frameworkElement.InputBindings.Remove(inputBinding);
            }
        }
    }
}