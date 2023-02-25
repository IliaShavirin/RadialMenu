using System.Windows.Controls;

namespace BaseProj.ExtensionMethods
{
    public static class TextBlockExtensions
    {
        public static TextBlock Clone(this TextBlock textBlock)
        {
            var tb = new TextBlock();

            // Reflection is not used here intentionaly. Should add desired clone properties when needed

            tb.Name = textBlock.Name;
            tb.Text = textBlock.Text;
            tb.TextAlignment = textBlock.TextAlignment;
            tb.TextDecorations = textBlock.TextDecorations;
            tb.TextEffects = textBlock.TextEffects;
            tb.TextTrimming = textBlock.TextTrimming;
            tb.TextWrapping = textBlock.TextWrapping;
            tb.FontFamily = textBlock.FontFamily;
            tb.FontSize = textBlock.FontSize;
            tb.FontStretch = textBlock.FontStretch;
            tb.FontStyle = textBlock.FontStyle;
            tb.FontWeight = textBlock.FontWeight;
            //tb.Foreground = textBlock.Foreground;
            tb.Margin = textBlock.Margin;
            tb.Padding = textBlock.Padding;
            tb.HorizontalAlignment = textBlock.HorizontalAlignment;
            tb.VerticalAlignment = textBlock.VerticalAlignment;
            tb.Style = textBlock.Style;

            return tb;
        }
    }
}