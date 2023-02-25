using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgZoomAndPan
    {
        #region Private fields

        private readonly SvgElement ownerElement;

        #endregion

        public SvgZoomAndPan(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
        }

        public SvgZoomAndPanType ZoomAndPan
        {
            get
            {
                if (ownerElement != null && ownerElement.HasAttribute("zoomAndPan"))
                    switch (ownerElement.GetAttribute("zoomAndPan").Trim())
                    {
                        case "magnify": return SvgZoomAndPanType.Magnify;
                        case "disable": return SvgZoomAndPanType.Disable;
                    }

                return SvgZoomAndPanType.Unknown;
            }
        }
    }
}