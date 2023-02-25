using System;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    public class CssAbsPrimitiveLengthValue : CssPrimitiveLengthValue
    {
        public CssAbsPrimitiveLengthValue(CssPrimitiveValue cssValue, string propertyName, XmlElement element)
        {
            if (cssValue.PrimitiveType == CssPrimitiveType.Ident)
            {
                // this is primarily to deal with font sizes.
                float absSize;
                switch (cssValue.GetStringValue())
                {
                    case "xx-small":
                        absSize = 6F;
                        break;
                    case "x-small":
                        absSize = 7.5F;
                        break;
                    case "small":
                        absSize = 8.88F;
                        break;
                    case "large":
                        absSize = 12F;
                        break;
                    case "x-large":
                        absSize = 15F;
                        break;
                    case "xx-large":
                        absSize = 20F;
                        break;
                    case "larger":
                    case "smaller":
                        float parSize;
                        if (_parentElement != null)
                        {
                            var csd = (CssStyleDeclaration)_ownerDocument.GetComputedStyle(_parentElement,
                                string.Empty);
                            var cssPrimValue = csd.GetPropertyCssValue("font-size") as CssPrimitiveValue;

                            // no default font-size set => use 10px
                            if (cssPrimValue == null)
                                parSize = 10;
                            else
                                parSize = (float)cssPrimValue.GetFloatValue(CssPrimitiveType.Px);
                        }
                        else
                        {
                            parSize = 10;
                        }

                        if (cssValue.GetStringValue() == "smaller") absSize = parSize / 1.2F;
                        else absSize = parSize * 1.2F;
                        break;
                    default:
                        absSize = 10F;
                        break;
                }

                SetFloatValue(absSize);
                SetPrimitiveType(CssPrimitiveType.Px);
            }
            else
            {
                SetFloatValue(cssValue.GetFloatValue(cssValue.PrimitiveType));
                SetPrimitiveType(cssValue.PrimitiveType);
            }

            PropertyName = propertyName;
            _element = element;
        }

        public override bool ReadOnly => false;

        #region Public members

        public string PropertyName { get; }

        #endregion

        #region Overrides of CssPrimitiveLengthValue

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            double ret;
            if (PrimitiveType == CssPrimitiveType.Percentage)
            {
                if (unitType == CssPrimitiveType.Percentage)
                    return floatValue;
                throw new NotImplementedException("Can't get absolute values from percentages");
            }

            switch (unitType)
            {
                case CssPrimitiveType.Cm:
                    ret = _getInLength() * CmPerIn;
                    break;
                case CssPrimitiveType.Mm:
                    ret = _getInLength() * CmPerIn * 10;
                    break;
                case CssPrimitiveType.In:
                    ret = _getInLength();
                    break;
                case CssPrimitiveType.Pc:
                    ret = _getInLength() * 6;
                    break;
                case CssPrimitiveType.Pt:
                    ret = _getInLength() * 72;
                    break;
                case CssPrimitiveType.Ems:
                    ret = _getPxLength() / _getFontSize();
                    break;
                case CssPrimitiveType.Exs:
                    ret = _getPxLength() / (_getFontSize() * 0.5);
                    break;
                default:
                    ret = _getPxLength();
                    break;
            }

            return ret;
        }

        #endregion

        #region Private members

        private readonly XmlElement _element;

        private XmlElement _parentElement => _element.ParentNode as XmlElement;

        private CssXmlDocument _ownerDocument => _element.OwnerDocument as CssXmlDocument;

        private double _getFontSize()
        {
            CssPrimitiveValue cssPrimValue;
            XmlElement elmToUse;
            if (PropertyName != null && PropertyName.Equals("font-size"))
                elmToUse = _parentElement;
            else
                elmToUse = _element;

            if (elmToUse == null) return 10;

            var csd = (CssStyleDeclaration)_ownerDocument.GetComputedStyle(elmToUse, string.Empty);
            cssPrimValue = csd.GetPropertyCssValue("font-size") as CssPrimitiveValue;

            // no default font-size set => use 10px
            if (cssPrimValue == null)
                return 10;
            return cssPrimValue.GetFloatValue(CssPrimitiveType.Px);
        }

        private double _getPxLength()
        {
            //double floatValue = this. _cssValue.GetFloatValue(_cssValue.PrimitiveType);
            switch (PrimitiveType)
            {
                case CssPrimitiveType.In:
                    return floatValue * Dpi;
                case CssPrimitiveType.Cm:
                    return floatValue / CmPerIn * Dpi;
                case CssPrimitiveType.Mm:
                    return floatValue / 10 / CmPerIn * Dpi;
                case CssPrimitiveType.Pt:
                    return floatValue / 72 * Dpi;
                case CssPrimitiveType.Pc:
                    return floatValue / 6 * Dpi;
                case CssPrimitiveType.Ems:
                    return floatValue * _getFontSize();
                case CssPrimitiveType.Exs:
                    return floatValue * _getFontSize() * 0.5F;
                default:
                    return floatValue;
            }
        }

        private double _getInLength()
        {
            return _getPxLength() / Dpi;
        }

        #endregion
    }
}