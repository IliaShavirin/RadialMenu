using System.Windows;

namespace BaseProj.AttachedBehaviours
{
    public static class SystemParameterProperties
    {
        public static readonly DependencyProperty HighContrastProperty =
            DependencyProperty.RegisterAttached("HighContrast", typeof(bool), typeof(SystemParameterProperties),
                new FrameworkPropertyMetadata { Inherits = true });

        public static bool GetHighContrast(DependencyObject obj)
        {
            return (bool)obj.GetValue(HighContrastProperty);
        }

        public static void SetHighContrast(DependencyObject obj, bool value)
        {
            obj.SetValue(HighContrastProperty, value);
        }
    }
}