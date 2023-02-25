using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgExternalResourcesRequired
    {
        private readonly SvgElement ownerElement;
        private ISvgAnimatedBoolean externalResourcesRequired;

        public SvgExternalResourcesRequired(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;

            this.ownerElement.attributeChangeHandler += AttributeChange;
        }

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                if (externalResourcesRequired == null)
                    externalResourcesRequired =
                        new SvgAnimatedBoolean(ownerElement.GetAttribute("externalResourcesRequired"), false);
                return externalResourcesRequired;
            }
        }


        private void AttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            var attribute = src as XmlAttribute;

            if (attribute.LocalName == "externalResourcesRequired") externalResourcesRequired = null;
        }
    }
}