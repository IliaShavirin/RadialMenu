using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    public class CssAbsPrimitiveValue : CssPrimitiveValue
    {
        private readonly CssPrimitiveValue _cssValue;
        private readonly XmlElement _element;
        private string _propertyName;

        public CssAbsPrimitiveValue(CssPrimitiveValue cssValue, string propertyName,
            XmlElement element)
        {
            _cssValue = cssValue;
            _propertyName = propertyName;
            _element = element;
        }

        public override string CssText => _cssValue.CssText;

        public override CssPrimitiveType PrimitiveType => _cssValue.PrimitiveType;

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            return _cssValue.GetFloatValue(unitType);
        }

        public override string GetStringValue()
        {
            switch (PrimitiveType)
            {
                case CssPrimitiveType.Attr:
                    return _element.GetAttribute(_cssValue.GetStringValue(), string.Empty);
                default:
                    return _cssValue.GetStringValue();
            }
        }

        public override ICssRect GetRectValue()
        {
            return _cssValue.GetRectValue();
        }

        public override ICssColor GetRgbColorValue()
        {
            return _cssValue.GetRgbColorValue();
        }
    }
}