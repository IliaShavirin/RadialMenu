using System.Windows;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public abstract class WpfFontFamilyVisitor : DependencyObject
    {
        public abstract WpfFontFamilyInfo Visit(string fontName, WpfFontFamilyInfo familyInfo,
            WpfDrawingContext context);
    }
}