using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BaseProj.ExtensionMethods
{
    public static class FraneworkElementExtensions
    {
        public static RenderTargetBitmap CopyAsBitmap(this FrameworkElement frameworkElement)
        {
            var targetWidth = (int)frameworkElement.ActualWidth;
            var targetHeight = (int)frameworkElement.ActualHeight;

            // Exit if there's no 'area' to render
            if (targetWidth == 0 || targetHeight == 0)
                return null;

            // Prepare the rendering target
            var result = new RenderTargetBitmap(targetWidth, targetHeight, 96, 96, PixelFormats.Pbgra32);

            // Render the framework element into the target
            result.Render(frameworkElement);

            return result;
        }
    }
}