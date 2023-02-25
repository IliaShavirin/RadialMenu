using System.Windows;
using System.Windows.Controls;

namespace BaseProj.AttachedBehaviours
{
    public static class ButtonBehaviour
    {
        public static readonly DependencyProperty TextFormatRuleProperty = DependencyProperty.RegisterAttached(
            "TextFormatRule", typeof(string), typeof(ButtonBehaviour),
            new PropertyMetadata(default(string), TextFormatRuleChangedCallback));

        private static void TextFormatRuleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (button == null)
                return;

            var formatRule = e.NewValue as string;
            if (!string.IsNullOrEmpty(formatRule))
            {
            }
        }

        public static void SetTextFormatRule(DependencyObject element, string value)
        {
            element.SetValue(TextFormatRuleProperty, value);
        }

        public static string GetTextFormatRule(DependencyObject element)
        {
            return (string)element.GetValue(TextFormatRuleProperty);
        }
    }
}