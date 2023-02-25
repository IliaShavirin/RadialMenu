// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com.com</developer>
// <completed>100</completed>

using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     This interface defines a list of SvgLength objects
    /// </summary>
    public sealed class SvgLengthList : SvgList<ISvgLength>, ISvgLengthList
    {
        public void FromString(string listString)
        {
            // remove existing list items
            Clear();

            if (listString != null)
            {
                // remove leading and trailing whitespace
                // NOTE: Need to check if .NET whitespace = SVG (XML) whitespace
                listString = listString.Trim();

                if (listString.Length > 0)
                {
                    var delim = new Regex(@"\s+,?\s*|,\s*");
                    foreach (var item in delim.Split(listString))
                    {
                        // the following test is needed to catch consecutive commas
                        // for example, "one,two,,three"
                        if (item.Length == 0)
                            throw new DomException(DomExceptionType.SyntaxErr);

                        AppendItem(new SvgLength(ownerElement, propertyName,
                            direction, item, string.Empty));
                    }
                }
            }
        }

        #region Fields

        private readonly SvgElement ownerElement;
        private readonly SvgLengthDirection direction;
        private readonly string propertyName;

        #endregion

        #region Constructors

        public SvgLengthList()
        {
        }

        public SvgLengthList(string listString)
        {
            FromString(listString);
        }

        public SvgLengthList(string propertyName, string listString, SvgElement ownerElement,
            SvgLengthDirection direction)
        {
            this.propertyName = propertyName;
            this.ownerElement = ownerElement;
            this.direction = direction;

            FromString(listString);
        }

        #endregion
    }
}