using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Fills
{
    /// <summary>
    ///     Summary description for SvgRadialGradientElement.
    /// </summary>
    public sealed class SvgRadialGradientElement : SvgGradientElement, ISvgRadialGradientElement
    {
        #region Constructors and Destructor

        public SvgRadialGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgURIReference Members

        public new SvgRadialGradientElement ReferencedElement => base.ReferencedElement as SvgRadialGradientElement;

        #endregion

        #region Private Fields

        private ISvgAnimatedLength cx;
        private ISvgAnimatedLength cy;
        private ISvgAnimatedLength r;
        private ISvgAnimatedLength fx;
        private ISvgAnimatedLength fy;

        #endregion

        #region ISvgRadialGradientElement Members

        public ISvgAnimatedLength Cx
        {
            get
            {
                if (!HasAttribute("cx") && ReferencedElement != null) return ReferencedElement.Cx;

                if (cx == null) cx = new SvgAnimatedLength(this, "cx", SvgLengthDirection.Horizontal, "50%");
                return cx;
            }
        }

        public ISvgAnimatedLength Cy
        {
            get
            {
                if (!HasAttribute("cy") && ReferencedElement != null) return ReferencedElement.Cy;

                if (cy == null) cy = new SvgAnimatedLength(this, "cy", SvgLengthDirection.Vertical, "50%");
                return cy;
            }
        }

        public ISvgAnimatedLength R
        {
            get
            {
                if (!HasAttribute("r") && ReferencedElement != null) return ReferencedElement.R;

                if (r == null) r = new SvgAnimatedLength(this, "r", SvgLengthDirection.Viewport, "50%");
                return r;
            }
        }

        public ISvgAnimatedLength Fx
        {
            get
            {
                if (!HasAttribute("fx") && HasAttribute("fy")) return Fy;

                if (!HasAttribute("fx") && ReferencedElement != null) return ReferencedElement.Fx;

                if (fx == null) fx = new SvgAnimatedLength(this, "fx", SvgLengthDirection.Horizontal, "50%");
                return fx;
            }
        }

        public ISvgAnimatedLength Fy
        {
            get
            {
                if (!HasAttribute("fy") && HasAttribute("fx")) return Fx;

                if (!HasAttribute("fy") && ReferencedElement != null) return ReferencedElement.Fy;

                if (fy == null) fy = new SvgAnimatedLength(this, "fy", SvgLengthDirection.Vertical, "50%");
                return fy;
            }
        }

        #endregion

        #region Update handling

        /*public override void OnAttributeChange(XmlNodeChangedAction action, XmlAttribute attribute)
        {
            base.OnAttributeChange(action, attribute);

            if(attribute.NamespaceURI.Length == 0)
            {
                switch(attribute.LocalName)
                {
                    case "cx":
                        cx = null;
                        break;
                    case "cy":
                        cy = null;
                        break;
                    case "r":
                        r = null;
                        break;
                    case "fx":
                        fx = null;
                        break;
                    case "fy":
                        fy = null;
                        break;
                }
            }
        }*/

        #endregion
    }
}