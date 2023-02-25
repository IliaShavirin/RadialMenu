using System.Windows;
using System.Windows.Controls;

namespace BaseProj.AttachedBehaviours
{
    public static class ToolTipHelper
    {
        public static readonly DependencyProperty DisabledToolTipProperty = DependencyProperty.RegisterAttached(
            "DisabledToolTip", typeof(object), typeof(ToolTipHelper),
            new PropertyMetadata(default, DisabledToolTipChanged));

        public static readonly DependencyProperty BackupToolTipProperty = DependencyProperty.RegisterAttached(
            "BackupToolTip", typeof(object), typeof(ToolTipHelper), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty StayOpenProperty = DependencyProperty.RegisterAttached(
            "StayOpen", typeof(bool), typeof(ToolTipHelper), new PropertyMetadata(default(bool), StayOpenChanged));

        private static void DisabledToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element == null)
                return;

            if (!Equals(e.OldValue, element.ToolTip)) SetBackupToolTip(element, element.ToolTip);

            if (e.NewValue != null)
            {
                element.SetValue(ToolTipService.ShowOnDisabledProperty, true);
                if (!element.IsEnabled) element.ToolTip = e.NewValue;

                element.IsEnabledChanged += ElementOnIsEnabledChanged;

                ToolTipService.SetShowOnDisabled(element, true);
            }
            else
            {
                element.SetValue(ToolTipService.ShowOnDisabledProperty, false);
                element.ToolTip = GetBackupToolTip(element);

                element.IsEnabledChanged -= ElementOnIsEnabledChanged;

                ToolTipService.SetShowOnDisabled(element, false);
            }
        }

        private static void ElementOnIsEnabledChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            var element = s as FrameworkElement;
            if (element == null)
                return;

            if ((bool)e.NewValue)
                element.ToolTip = GetBackupToolTip(element);
            else
                element.ToolTip = GetDisabledToolTip(element);
        }

        public static void SetDisabledToolTip(DependencyObject element, object value)
        {
            element.SetValue(DisabledToolTipProperty, value);
        }

        public static object GetDisabledToolTip(DependencyObject element)
        {
            return element.GetValue(DisabledToolTipProperty);
        }

        public static void SetBackupToolTip(DependencyObject element, object value)
        {
            element.SetValue(BackupToolTipProperty, value);
        }

        public static object GetBackupToolTip(DependencyObject element)
        {
            return element.GetValue(BackupToolTipProperty);
        }

        private static void StayOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (GetStayOpen(d))
                ToolTipService.SetShowDuration(d, int.MaxValue);
            else
                ToolTipService.SetShowDuration(d, 5000);
        }

        public static void SetStayOpen(DependencyObject element, bool value)
        {
            element.SetValue(StayOpenProperty, value);
        }

        public static bool GetStayOpen(DependencyObject element)
        {
            return (bool)element.GetValue(StayOpenProperty);
        }
    }
}