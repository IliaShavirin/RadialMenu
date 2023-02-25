/// <developer>niklas@protocol7.com</developer>
/// <completed>100</completed>
/// 

using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    /// <summary>
    ///     The Rect interface is used to represent any rect value. This interface reflects the values in the underlying style
    ///     property. Hence, modifications made to the CSSPrimitiveValue objects modify the style property.
    /// </summary>
    public sealed class CssRect : ICssRect
    {
        #region Constructors

        /// <summary>
        ///     Constructs a new Rect
        /// </summary>
        /// <param name="s">The string to parse that contains the Rect structure</param>
        /// <param name="readOnly">Specifies if the Rect should be read-only</param>
        public CssRect(string rectString, bool readOnly)
        {
            this.readOnly = readOnly;

            if (rectString == null) rectString = string.Empty;

            // remove leading and trailing whitespace
            // NOTE: Need to check if .NET whitespace = SVG (XML) whitespace
            rectString = rectString.Trim();

            if (rectString.Length > 0)
            {
                var parts = rectString.Split(' ');
                if (parts.Length != 4) parts = delim.Split(rectString);
                if (parts.Length == 4)
                {
                    _top = new CssPrimitiveLengthValue(parts[0], readOnly);
                    _right = new CssPrimitiveLengthValue(parts[1], readOnly);
                    _bottom = new CssPrimitiveLengthValue(parts[2], readOnly);
                    _left = new CssPrimitiveLengthValue(parts[3], readOnly);
                }
                else
                {
                    throw new DomException(DomExceptionType.SyntaxErr);
                }
            }
        }

        #endregion

        #region Private Fields

        private static readonly Regex delim = new Regex(@"\s+,?\s*|,\s*", RegexOptions.Compiled);

        private bool readOnly;

        #endregion

        #region IRect Members

        private readonly CssPrimitiveValue _left;

        /// <summary>
        ///     This attribute is used for the left of the rect.
        /// </summary>
        public ICssPrimitiveValue Left => _left;

        private readonly CssPrimitiveValue _bottom;

        /// <summary>
        ///     This attribute is used for the bottom of the rect.
        /// </summary>
        public ICssPrimitiveValue Bottom => _bottom;

        private readonly CssPrimitiveValue _right;

        /// <summary>
        ///     This attribute is used for the right of the rect.
        /// </summary>
        public ICssPrimitiveValue Right => _right;

        private readonly CssPrimitiveValue _top;

        /// <summary>
        ///     This attribute is used for the top of the rect.
        /// </summary>
        public ICssPrimitiveValue Top => _top;

        #endregion
    }
}