using System.Windows;
using System.Windows.Media.Imaging;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public abstract class WpfEmbeddedImageVisitor : DependencyObject
    {
        public abstract BitmapSource Visit(SvgImageElement element,
            WpfDrawingContext context);
    }
}