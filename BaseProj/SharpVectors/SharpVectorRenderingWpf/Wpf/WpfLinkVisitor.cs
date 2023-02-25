using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public abstract class WpfLinkVisitor : DependencyObject
    {
        public abstract bool Aggregates { get; }

        public abstract bool IsAggregate { get; }

        public abstract string AggregatedLayerName { get; }

        public abstract bool Exists(string linkId);

        public abstract void Initialize(DrawingGroup linkGroup, WpfDrawingContext context);

        public abstract void Visit(DrawingGroup group, SvgAElement element,
            WpfDrawingContext context, float opacity);
    }
}