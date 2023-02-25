using System.Windows;
using System.Windows.Controls;

namespace BaseProj.AttachedBehaviours
{
    public class PasswordBoxMonitor : DependencyObject
    {
        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor),
                new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor),
                new UIPropertyMetadata(0));

        public static readonly DependencyProperty WaterMarkTextProperty = DependencyProperty.RegisterAttached(
            "WaterMarkText", typeof(string), typeof(PasswordBoxMonitor), new PropertyMetadata(default(string)));

        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }


        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;
            if (pb == null) return;
            if ((bool)e.NewValue)
                pb.PasswordChanged += PasswordChanged;
            else
                pb.PasswordChanged -= PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb == null) return;
            SetPasswordLength(pb, pb.Password.Length);
        }

        public static void SetWaterMarkText(DependencyObject element, string value)
        {
            element.SetValue(WaterMarkTextProperty, value);
        }

        public static string GetWaterMarkText(DependencyObject element)
        {
            return (string)element.GetValue(WaterMarkTextProperty);
        }
    }
}