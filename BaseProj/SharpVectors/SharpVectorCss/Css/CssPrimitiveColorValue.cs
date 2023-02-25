using System;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    public sealed class CssPrimitiveColorValue : CssPrimitiveValue
    {
        public CssPrimitiveColorValue(double color, bool readOnly)
            : base(color.ToString(CssNumber.Format), readOnly)
        {
            SetFloatValue(color);
            SetPrimitiveType(CssPrimitiveType.Number);
        }

        public CssPrimitiveColorValue(int color, bool readOnly)
            : base(color.ToString(CssNumber.Format), readOnly)
        {
            SetFloatValue(color);
            SetPrimitiveType(CssPrimitiveType.Number);
        }

        public CssPrimitiveColorValue(string cssText, bool readOnly)
            : base(cssText, readOnly)
        {
            OnSetCssText(cssText);
        }

        public override string CssText
        {
            get
            {
                var dbl = GetFloatValue(CssPrimitiveType.Number);
                return Convert.ToInt32(dbl).ToString();
            }
        }

        protected override void OnSetCssText(string cssText)
        {
            if (cssText.EndsWith("%"))
            {
                cssText = cssText.Remove(cssText.Length - 1, 1);
                SetPrimitiveType(CssPrimitiveType.Percentage);
            }
            else
            {
                SetPrimitiveType(CssPrimitiveType.Number);
            }

            SetFloatValue(cssText);
        }

        public override double GetFloatValue(CssPrimitiveType unitType)
        {
            var ret = double.NaN;
            switch (unitType)
            {
                case CssPrimitiveType.Number:
                    if (PrimitiveType == CssPrimitiveType.Number) ret = floatValue;
                    else if (PrimitiveType == CssPrimitiveType.Percentage) ret = floatValue / 100 * 255D;
                    break;
                case CssPrimitiveType.Percentage:
                    if (PrimitiveType == CssPrimitiveType.Percentage) ret = floatValue;
                    else if (PrimitiveType == CssPrimitiveType.Number) ret = floatValue * 255D;
                    break;
            }

            if (double.IsNaN(ret))
                throw new DomException(DomExceptionType.InvalidAccessErr);
            return ret;
        }
    }
}