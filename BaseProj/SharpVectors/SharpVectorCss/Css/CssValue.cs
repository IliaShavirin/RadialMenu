// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Text.RegularExpressions;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    /// <summary>
    ///     The CSSValue interface represents a simple or a complex value. A CSSValue object only occurs in a context of a CSS
    ///     property
    /// </summary>
    public class CssValue : ICssValue
    {
        public virtual bool ReadOnly { get; }

        #region Public methods

        public virtual CssValue GetAbsoluteValue(string propertyName, XmlElement elm)
        {
            return new CssAbsValue(this, propertyName, elm);
        }

        #endregion

        #region Static members

        private static readonly string numberPattern = @"[\-\+]?[0-9]*\.?[0-9]+";
        public static string LengthUnitPattern = "(?<lengthUnit>in|cm|mm|px|em|ex|pc|pt|%)?";
        public static string AngleUnitPattern = "(?<angleUnit>deg|rad|grad)?";
        public static string LengthPattern = @"(?<lengthNumber>" + numberPattern + ")" + LengthUnitPattern;
        public static string AnglePattern = @"(?<angleNumber>" + numberPattern + ")" + AngleUnitPattern;

        private static readonly string cssPrimValuePattern = @"^(?<primitiveValue>"
                                                             + @"(?<func>(?<funcname>attr|url|counter|rect|rgb)\((?<funcvalue>[^\)]+)\))"
                                                             + @"|(?<length>" + LengthPattern + ")"
                                                             + @"|(?<angle>" + AnglePattern + ")"
                                                             + @"|(?<freqTimeNumber>(?<numberValue2>" + numberPattern +
                                                             ")(?<unit2>Hz|kHz|in|s|ms|%)?)"
                                                             + @"|(?<string>[""'](?<stringvalue>(.|\n)*?)[""'])"
                                                             + @"|(?<colorIdent>([A-Za-z]+)|(\#[A-Fa-f0-9]{6})|(\#[A-Fa-f0-9]{3}))"
                                                             + @")";

        private static readonly Regex reCssPrimitiveValue = new Regex(cssPrimValuePattern + "$");
        private static readonly Regex reCssValueList = new Regex(cssPrimValuePattern + @"(\s*,\s*)+$");

        /// <summary>
        ///     Detects what kind of value cssText contains and returns an instance of the correct CssValue class
        /// </summary>
        /// <param name="cssText">The text to parse for a CSS value</param>
        /// <param name="readOnly">Specifies if this instance is read-only</param>
        /// <returns>The correct type of CSS value</returns>
        public static CssValue GetCssValue(string cssText, bool readOnly)
        {
            if (cssText == "inherit")
                // inherit
                return new CssValue(CssValueType.Inherit, cssText, readOnly);

            var match = reCssPrimitiveValue.Match(cssText);
            if (match.Success)
                // single primitive value
                return CssPrimitiveValue.Create(match, readOnly);

            match = reCssValueList.Match(cssText);
            if (match.Success)
                // list of primitive values
                throw new NotImplementedException("Value lists not implemented");
            // custom value
            return new CssValue(CssValueType.Custom, cssText, readOnly);
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for CssValue
        /// </summary>
        /// <param name="type">The type of value</param>
        /// <param name="cssText">The entire content of the value</param>
        /// <param name="readOnly">Specifies if the instance is read-only</param>
        public CssValue(CssValueType type, string cssText, bool readOnly)
        {
            _cssText = cssText.Trim();
            _cssValueType = type;
            ReadOnly = readOnly;
        }

        /// <summary>
        ///     Only for internal use
        /// </summary>
        protected CssValue()
        {
            ReadOnly = true;
        }

        #endregion

        #region ICssValue Members

        private string _cssText;
        protected CssValueType _cssValueType;

        /// <summary>
        ///     A string representation of the current value.
        /// </summary>
        /// <exception cref="DomException">
        ///     SYNTAX_ERR: Raised if the specified CSS string value has a syntax error (according to
        ///     the attached property) or is unparsable.
        /// </exception>
        /// <exception cref="DomException">
        ///     INVALID_MODIFICATION_ERR: Raised if the specified CSS string value represents a
        ///     different type of values than the values allowed by the CSS property
        /// </exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this value is readonly.</exception>
        public virtual string CssText
        {
            get => _cssText;
            set
            {
                if (ReadOnly)
                    throw new DomException(DomExceptionType.InvalidModificationErr, "The CssValue is read-only");
                _cssText = value;
            }
        }

        /// <summary>
        ///     A code defining the type of the value as defined above
        /// </summary>
        public virtual CssValueType CssValueType => _cssValueType;

        #endregion
    }
}