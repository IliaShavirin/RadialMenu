using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    public class CssAbsValue : CssValue
    {
        private readonly CssValue _cssValue;
        private XmlElement _element;
        private string _propertyName;

        public CssAbsValue(CssValue cssValue, string propertyName, XmlElement element)
        {
            _cssValue = cssValue;
            _propertyName = propertyName;
            _element = element;
        }

        public override string CssText => _cssValue.CssText;

        public override CssValueType CssValueType => _cssValue.CssValueType;
    }
}