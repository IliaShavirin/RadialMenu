using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BaseProj.AttachedBehaviours
{
    public static class InsertControlIntoTextBlock
    {
        public static readonly DependencyProperty ElementProperty = DependencyProperty.RegisterAttached(
            "Element", typeof(TextBlock), typeof(InsertControlIntoTextBlock),
            new PropertyMetadata(default(TextBlock), OnElementChanged));

        private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = e.NewValue as TextBlock;
            var element = d as FrameworkElement;

            if (tb == null || element == null) return;

            var parent = element.Parent;
            if (parent == null) return;

            VisualTreeHelperExt.RemoveChild(parent, element);

            var iuc = new InlineUIContainer(element);
            iuc.BaselineAlignment = BaselineAlignment.Center;

            var oldText = tb.Text;
            tb.Text = null;

            tb.Inlines.Add(iuc);
            var run = new Run { Text = oldText };

            tb.Inlines.Add(run);
        }

        public static void SetElement(DependencyObject element, TextBlock value)
        {
            element.SetValue(ElementProperty, value);
        }

        public static TextBlock GetElement(DependencyObject element)
        {
            return (TextBlock)element.GetValue(ElementProperty);
        }
    }
}