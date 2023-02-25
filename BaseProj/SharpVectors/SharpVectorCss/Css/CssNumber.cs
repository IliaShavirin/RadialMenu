using System.Globalization;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    public sealed class CssNumber
    {
        private static NumberFormatInfo format;

        public static NumberFormatInfo Format
        {
            get
            {
                if (format == null)
                {
                    format = new NumberFormatInfo();
                    format.NumberDecimalSeparator = ".";
                }

                return format;
            }
        }
    }
}