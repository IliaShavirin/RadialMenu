using System;
using System.IO;
using System.Windows.Media.Imaging;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    public sealed class EmbeddedImageVisitor : WpfEmbeddedImageVisitor
    {
        public override BitmapSource Visit(SvgImageElement element,
            WpfDrawingContext context)
        {
            var sURI = element.Href.AnimVal;
            var nColon = sURI.IndexOf(":");
            var nSemiColon = sURI.IndexOf(";");
            var nComma = sURI.IndexOf(",");

            var sMimeType = sURI.Substring(nColon + 1, nSemiColon - nColon - 1);

            var sContent = sURI.Substring(nComma + 1);
            var imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(),
                0, sContent.Length);

            //BitmapImage imageSource = new BitmapImage();
            //imageSource.BeginInit();
            //imageSource.StreamSource = new MemoryStream(imageBytes);
            //imageSource.EndInit();

            return new EmbeddedBitmapSource(new MemoryStream(imageBytes));
        }
    }
}