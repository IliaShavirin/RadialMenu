// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com.com</developer>
// <completed>100</completed>

using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     This interface defines a list of String objects
    /// </summary>
    public sealed class SvgStringList : SvgList<string>, ISvgStringList
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

                        AppendItem(item);
                    }
                }
                else
                {
                    AppendItem(string.Empty);
                }
            }
        }

        #region Constructors

        public SvgStringList()
        {
        }

        public SvgStringList(string listString)
        {
            FromString(listString);
        }

        #endregion
    }
}