using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Fills
{
    public abstract class SvgGradientElement : SvgStyleableElement, ISvgGradientElement
    {
        private ISvgAnimatedTransformList gradientTransform;
        private ISvgAnimatedEnumeration gradientUnits;
        private ISvgAnimatedEnumeration spreadMethod;

        protected SvgGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #region Public Properties

        public XmlNodeList Stops
        {
            get
            {
                var stops = SelectNodes("svg:stop", OwnerDocument.NamespaceManager);
                if (stops.Count > 0) return stops;

                // check any eventually referenced gradient
                if (ReferencedElement == null)
                    // return an empty list
                    return stops;
                return ReferencedElement.Stops;
            }
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
                switch (attribute.LocalName)
                {
                    case "gradientUnits":
                        gradientUnits = null;
                        break;
                    case "gradientTransform":
                        gradientTransform = null;
                        break;
                    case "spreadMethod":
                        spreadMethod = null;
                        break;
                }

            base.HandleAttributeChange(attribute);
        }

        #endregion

        #region ISvgGradientElement Members

        public ISvgAnimatedEnumeration GradientUnits
        {
            get
            {
                if (!HasAttribute("gradientUnits") && ReferencedElement != null) return ReferencedElement.GradientUnits;

                if (gradientUnits == null)
                {
                    SvgUnitType gradUnit;
                    switch (GetAttribute("gradientUnits"))
                    {
                        case "userSpaceOnUse":
                            gradUnit = SvgUnitType.UserSpaceOnUse;
                            break;
                        default:
                            gradUnit = SvgUnitType.ObjectBoundingBox;
                            break;
                    }

                    gradientUnits = new SvgAnimatedEnumeration((ushort)gradUnit);
                }

                return gradientUnits;
            }
        }

        public ISvgAnimatedTransformList GradientTransform
        {
            get
            {
                if (!HasAttribute("gradientTransform") && ReferencedElement != null)
                    return ReferencedElement.GradientTransform;

                if (gradientTransform == null)
                    gradientTransform = new SvgAnimatedTransformList(GetAttribute("gradientTransform"));

                return gradientTransform;
            }
        }

        public ISvgAnimatedEnumeration SpreadMethod
        {
            get
            {
                if (!HasAttribute("spreadMethod") && ReferencedElement != null) return ReferencedElement.SpreadMethod;

                if (spreadMethod == null)
                {
                    SvgSpreadMethod spreadMeth;
                    switch (GetAttribute("spreadMethod"))
                    {
                        case "pad":
                            spreadMeth = SvgSpreadMethod.Pad;
                            break;
                        case "reflect":
                            spreadMeth = SvgSpreadMethod.Reflect;
                            break;
                        case "repeat":
                            spreadMeth = SvgSpreadMethod.Repeat;
                            break;
                        default:
                            spreadMeth = SvgSpreadMethod.None;
                            break;
                    }

                    spreadMethod = new SvgAnimatedEnumeration((ushort)spreadMeth);
                }

                return spreadMethod;
            }
        }

        #endregion

        #region ISvgURIReference Members

        private readonly SvgUriReference svgURIReference;

        public ISvgAnimatedString Href => svgURIReference.Href;

        public SvgGradientElement ReferencedElement => svgURIReference.ReferencedNode as SvgGradientElement;

        #endregion

        #region ISvgExternalResourcesRequired Members

        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion
    }
}