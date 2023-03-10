using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Fills
{
    public sealed class SvgLinearGradientElement : SvgGradientElement, ISvgLinearGradientElement
    {
        private ISvgAnimatedLength x1;
        private ISvgAnimatedLength x2;
        private ISvgAnimatedLength y1;
        private ISvgAnimatedLength y2;

        public SvgLinearGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #region ISvgURIReference Members

        public new SvgLinearGradientElement ReferencedElement => base.ReferencedElement as SvgLinearGradientElement;

        #endregion

        #region ISvgLinearGradientElement Members

        public ISvgAnimatedLength X1
        {
            get
            {
                if (!HasAttribute("x1") && ReferencedElement != null) return ReferencedElement.X1;

                if (x1 == null) x1 = new SvgAnimatedLength(this, "x1", SvgLengthDirection.Horizontal, "0%");

                return x1;
            }
        }

        public ISvgAnimatedLength Y1
        {
            get
            {
                if (!HasAttribute("y1") && ReferencedElement != null) return ReferencedElement.Y1;

                if (y1 == null) y1 = new SvgAnimatedLength(this, "y1", SvgLengthDirection.Vertical, "0%");

                return y1;
            }
        }

        public ISvgAnimatedLength X2
        {
            get
            {
                if (!HasAttribute("x2") && ReferencedElement != null) return ReferencedElement.X2;

                if (x2 == null) x2 = new SvgAnimatedLength(this, "x2", SvgLengthDirection.Horizontal, "100%");

                return x2;
            }
        }

        public ISvgAnimatedLength Y2
        {
            get
            {
                if (!HasAttribute("y2") && ReferencedElement != null) return ReferencedElement.Y2;

                if (y2 == null) y2 = new SvgAnimatedLength(this, "y2", SvgLengthDirection.Vertical, "0%");

                return y2;
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
                    case "x1":
                        x1 = null;
                        break;
                    case "y1":
                        y1 = null;
                        break;
                    case "x2":
                        x2 = null;
                        break;
                    case "y2":
                        y2 = null;
                        break;
                }
            }
        }*/

        #endregion
    }
}