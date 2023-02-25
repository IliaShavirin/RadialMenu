using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public class SvgFitToViewBox : ISvgFitToViewBox
    {
        #region Protected Fields

        protected SvgElement ownerElement;

        #endregion

        private ISvgAnimatedPreserveAspectRatio preserveAspectRatio;
        private ISvgAnimatedRect viewBox;

        public SvgFitToViewBox(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
            this.ownerElement.attributeChangeHandler += AttributeChange;
        }

        public ISvgAnimatedRect ViewBox
        {
            get
            {
                if (viewBox == null)
                {
                    var attr = ownerElement.GetAttribute("viewBox").Trim();
                    if (string.IsNullOrEmpty(attr))
                    {
                        double x = 0;
                        double y = 0;
                        double width = 0;
                        double height = 0;
                        if (ownerElement is SvgSvgElement)
                        {
                            var svgSvgElm = ownerElement as SvgSvgElement;

                            x = svgSvgElm.X.AnimVal.Value;
                            y = svgSvgElm.Y.AnimVal.Value;
                            width = svgSvgElm.Width.AnimVal.Value;
                            height = svgSvgElm.Height.AnimVal.Value;
                        }

                        viewBox = new SvgAnimatedRect(new SvgRect(x, y, width, height));
                    }
                    else
                    {
                        viewBox = new SvgAnimatedRect(attr);
                    }
                }

                return viewBox;
            }
        }

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get
            {
                if (preserveAspectRatio == null)
                    preserveAspectRatio = new SvgAnimatedPreserveAspectRatio(
                        ownerElement.GetAttribute("preserveAspectRatio"), ownerElement);
                return preserveAspectRatio;
            }
        }

        #region Update handling

        private void AttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            var attribute = src as XmlAttribute;

            if (attribute.NamespaceURI.Length == 0)
                switch (attribute.LocalName)
                {
                    case "viewBox":
                        viewBox = null;
                        break;
                    case "preserveAspectRatio":
                        preserveAspectRatio = null;
                        break;
                }
        }

        #endregion
    }
}