using System;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;
using BaseProj.SharpVectors.SharpVectorCss.Css;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Fills;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgLength : ISvgLength
    {
        #region Private Methods

        private void GetCssXmlValue()
        {
            if (source == SvgLengthSource.Css)
            {
                var csd = ownerElement.GetComputedStyle(string.Empty);
                var cssValue = csd.GetPropertyCssValue(PropertyName) as CssPrimitiveLengthValue;

                if (cssValue != null)
                    cssLength = new CssAbsPrimitiveLengthValue(cssValue, PropertyName, ownerElement);
                else
                    throw new DomException(DomExceptionType.SyntaxErr, "Not a length value");
            }
            else
            {
                var baseVal = ownerElement.GetAttribute(PropertyName);

                if (baseVal == null || baseVal.Length == 0) baseVal = defaultValue;
                baseVal = SvgNumber.ScientificToDec(baseVal);
                cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                    PropertyName, ownerElement);
            }
        }

        #endregion

        #region Private Fields

        private static Regex reUnit = new Regex(CssValue.LengthUnitPattern + "$");

        private readonly string defaultValue;

        private readonly SvgLengthSource source;
        private readonly SvgElement ownerElement;
        private readonly SvgLengthDirection direction;
        private CssAbsPrimitiveLengthValue cssLength;

        #endregion

        #region Constructors

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthSource source,
            SvgLengthDirection direction, string defaultValue)
        {
            this.ownerElement = ownerElement;
            PropertyName = propertyName;
            this.direction = direction;
            this.defaultValue = defaultValue;
            this.source = source;

            if (this.source == SvgLengthSource.Xml || this.source == SvgLengthSource.Css) GetCssXmlValue();
        }

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthDirection direction,
            string baseVal)
            : this(ownerElement, propertyName, direction, baseVal, string.Empty)
        {
        }

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthDirection direction,
            string baseVal, string defaultValue)
            : this(ownerElement, propertyName,
                SvgLengthSource.String, direction, defaultValue)
        {
            if (baseVal == null || baseVal.Length == 0) baseVal = defaultValue;

            baseVal = SvgNumber.ScientificToDec(baseVal);
            cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                propertyName, ownerElement);
        }


        /// <summary>
        ///     Creates a SvgLength value
        /// </summary>
        /// <param name="baseVal">String to be parsed into a length</param>
        /// <param name="ownerElement">The associated element</param>
        /// <param name="direction">Direction of the length, used for percentages</param>
        public SvgLength(string propertyName, string baseVal, SvgElement ownerElement, SvgLengthDirection direction)
        {
            this.ownerElement = ownerElement;
            this.direction = direction;

            baseVal = SvgNumber.ScientificToDec(baseVal);

            cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false), propertyName,
                ownerElement);
        }

        public SvgLength(string propertyName, string baseVal, string defaultValue, SvgElement ownerElement,
            SvgLengthDirection direction)
        {
            this.ownerElement = ownerElement;
            this.direction = direction;

            if (baseVal == null || baseVal.Length == 0) baseVal = defaultValue;

            baseVal = SvgNumber.ScientificToDec(baseVal);

            cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false), propertyName,
                ownerElement);
        }

        public SvgLength(string propertyName, SvgStyleableElement ownerElement, SvgLengthDirection direction,
            string defaultValue)
        {
            this.ownerElement = ownerElement;
            this.direction = direction;

            var baseVal = ownerElement.GetPropertyValue(propertyName);
            if (baseVal == null || baseVal == "") baseVal = defaultValue;

            baseVal = SvgNumber.ScientificToDec(baseVal);

            cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false), propertyName,
                ownerElement);
        }

        #endregion

        #region Public Properties

        public string PropertyName { get; }

        /// <summary>
        ///     The type of the value as specified by one of the constants specified above.
        /// </summary>
        public SvgLengthType UnitType => (SvgLengthType)cssLength.PrimitiveType;

        /// <summary>
        ///     The value as an floating point value, in user units. Setting this attribute will cause valueInSpecifiedUnits and
        ///     valueAsString to be updated automatically to reflect this setting.
        /// </summary>
        /// <exception cref="DomException">
        ///     NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly
        ///     attribute.
        /// </exception>
        public double Value
        {
            get
            {
                double ret = 0;
                switch (UnitType)
                {
                    case SvgLengthType.Number:
                    case SvgLengthType.Px:
                    case SvgLengthType.Cm:
                    case SvgLengthType.Mm:
                    case SvgLengthType.In:
                    case SvgLengthType.Pt:
                    case SvgLengthType.Pc:
                    case SvgLengthType.Ems:
                    case SvgLengthType.Exs:
                        ret = cssLength.GetFloatValue(CssPrimitiveType.Px);
                        break;
                    case SvgLengthType.Percentage:
                        var valueInSpecifiedUnits = cssLength.GetFloatValue(CssPrimitiveType.Percentage);
                        if (ownerElement is SvgGradientElement)
                        {
                            ret = valueInSpecifiedUnits / 100F;
                        }
                        else
                        {
                            double w = 0;
                            double h = 0;
                            if (ownerElement.ViewportElement != null)
                            {
                                var ftv = (ISvgFitToViewBox)ownerElement.ViewportElement;
                                w = ftv.ViewBox.AnimVal.Width;
                                h = ftv.ViewBox.AnimVal.Height;
                            }
                            else
                            {
                                w = ownerElement.OwnerDocument.Window.InnerWidth;
                                h = ownerElement.OwnerDocument.Window.InnerHeight;
                            }

                            if (direction == SvgLengthDirection.Horizontal)
                                ret = valueInSpecifiedUnits * w / 100;
                            else if (direction == SvgLengthDirection.Vertical)
                                ret = valueInSpecifiedUnits * h / 100;
                            else
                                ret = Math.Sqrt(w * w + h * h) / Math.Sqrt(2) * valueInSpecifiedUnits / 100;
                        }

                        break;
                    case SvgLengthType.Unknown:
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Bad length unit");
                }

                if (double.IsNaN(ret)) ret = 10;
                return ret;
            }
            set
            {
                var oldType = cssLength.PrimitiveType;
                cssLength.SetFloatValue(CssPrimitiveType.Px, value);
                ConvertToSpecifiedUnits((SvgLengthType)oldType);
            }
        }

        /// <summary>
        ///     The value as an floating point value, in the units expressed by unitType. Setting this attribute will cause value
        ///     and valueAsString to be updated automatically to reflect this setting.
        /// </summary>
        /// <exception cref="DomException">
        ///     NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly
        ///     attribute.
        /// </exception>
        public double ValueInSpecifiedUnits
        {
            get => cssLength.GetFloatValue(cssLength.PrimitiveType);
            set => cssLength.SetFloatValue(cssLength.PrimitiveType, value);
        }

        /// <summary>
        ///     The value as a string value, in the units expressed by unitType. Setting this attribute will cause value and
        ///     valueInSpecifiedUnits to be updated automatically to reflect this setting.
        /// </summary>
        /// <exception cref="DomException">
        ///     NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly
        ///     attribute.
        /// </exception>
        public string ValueAsString
        {
            get => cssLength.CssText;
            set => cssLength =
                new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(value, false), "", ownerElement);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Reset the value as a number with an associated unitType, thereby replacing the values for all of the attributes on
        ///     the object.
        /// </summary>
        /// <param name="unitType">The unitType for the value (e.g., MM). </param>
        /// <param name="valueInSpecifiedUnits">The new value</param>
        public void NewValueSpecifiedUnits(SvgLengthType unitType, double valueInSpecifiedUnits)
        {
            cssLength.SetFloatValue((CssPrimitiveType)unitType, valueInSpecifiedUnits);
        }

        /// <summary>
        ///     Preserve the same underlying stored value, but reset the stored unit identifier to the given unitType. Object
        ///     attributes unitType, valueAsSpecified and valueAsString might be modified as a result of this method. For example,
        ///     if the original value were "0.5cm" and the method was invoked to convert to millimeters, then the unitType would be
        ///     changed to MM, valueAsSpecified would be changed to the numeric value 5 and valueAsString would be changed to
        ///     "5mm".
        /// </summary>
        /// <param name="unitType">The unitType to switch to (e.g., MM).</param>
        public void ConvertToSpecifiedUnits(SvgLengthType unitType)
        {
            var newValue = cssLength.GetFloatValue((CssPrimitiveType)unitType);
            cssLength.SetFloatValue((CssPrimitiveType)unitType, newValue);
        }

        #endregion
    }
}