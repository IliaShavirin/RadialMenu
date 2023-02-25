using System.Windows;

namespace BaseProj.AttachedBehaviours
{
    public static class CustomResizeBorderBehaviour
    {
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.RegisterAttached(
            "AllowResize", typeof(bool), typeof(CustomResizeBorderBehaviour), new PropertyMetadata(default(bool)));

        public static void SetAllowResize(DependencyObject element, bool value)
        {
            element.SetValue(AllowResizeProperty, value);
        }

        public static bool GetAllowResize(DependencyObject element)
        {
            return (bool)element.GetValue(AllowResizeProperty);
        }
    }
}