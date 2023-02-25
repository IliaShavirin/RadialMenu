using System;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    // TODO:  This class does not yet support custom views
    public sealed class SvgViewSpec : SvgFitToViewBox, ISvgViewSpec
    {
        #region Constructors and Destructor

        public SvgViewSpec(SvgElement ownerElement)
            : base(ownerElement)
        {
            // only use the base... 
        }

        #endregion

        #region ISvgZoomAndPan Members

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
            set => throw new NotImplementedException();
        }

        #endregion

        #region ISvgViewSpec Members

        public string TransformString => null;

        public ISvgElement ViewTarget => null;

        public string PreserveAspectRatioString => ownerElement.GetAttribute("preserveAspectRatio");

        public string ViewBoxString => ownerElement.GetAttribute("viewBox");

        public string ViewTargetString => null;

        public ISvgTransformList Transform => null;

        #endregion
    }
}