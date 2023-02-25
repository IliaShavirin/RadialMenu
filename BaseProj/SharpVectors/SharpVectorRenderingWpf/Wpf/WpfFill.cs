using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Fills;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public abstract class WpfFill : DependencyObject
    {
        #region Constructors and Destructor

        #endregion

        #region Public Methods

        public abstract Brush GetBrush(Rect elementBounds, WpfDrawingContext context);

        public static WpfFill CreateFill(SvgDocument document, string absoluteUri)
        {
            var node = document.GetNodeByUri(absoluteUri);

            var gradientNode = node as SvgGradientElement;
            if (gradientNode != null) return new WpfGradientFill(gradientNode);

            var patternNode = node as SvgPatternElement;
            if (patternNode != null) return new WpfPatternFill(patternNode);

            return null;
        }

        #endregion
    }
}