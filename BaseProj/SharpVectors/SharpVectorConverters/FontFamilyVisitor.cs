using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    public sealed class FontFamilyVisitor : WpfFontFamilyVisitor
    {
        private readonly FontFamily _arialFamily;

        public FontFamilyVisitor()
        {
            _arialFamily = new FontFamily("Arial");
        }

        public override WpfFontFamilyInfo Visit(string fontName, WpfFontFamilyInfo familyInfo,
            WpfDrawingContext context)
        {
            if (string.IsNullOrEmpty(fontName)) return null;

            if (fontName.StartsWith("Arial", StringComparison.OrdinalIgnoreCase) &&
                fontName.Length > 5)
            {
                if (string.Equals(fontName, "ArialMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "Arial-BoldMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "Arial-ItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        FontStyles.Italic, familyInfo.Stretch);
                if (string.Equals(fontName, "Arial-BoldItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        FontStyles.Italic, familyInfo.Stretch);
                if (string.Equals(fontName, "Arial Unicode MS",
                        StringComparison.OrdinalIgnoreCase))
                    return null;

                return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            }

            if (fontName.StartsWith("Helvetica", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(fontName, "Helvetica", StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "Helvetica-Bold",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "Helvetica-Oblique",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        FontStyles.Italic, familyInfo.Stretch);
                if (string.Equals(fontName, "Helvetica-BoldOblique",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        FontStyles.Italic, familyInfo.Stretch);

                return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            }

            if (fontName.StartsWith("TimesNewRomanPS", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(fontName, "TimesNewRomanPSMT", StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "TimesNewRomanPS-BoldMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "TimesNewRomanPS-ItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        FontStyles.Italic, familyInfo.Stretch);
                if (string.Equals(fontName, "TimesNewRomanPS-BoldItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        FontStyles.Italic, familyInfo.Stretch);

                return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            }

            if (fontName.StartsWith("CourierNewPS", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(fontName, "CourierNewPSMT", StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "CourierNewPS-BoldMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        familyInfo.Style, familyInfo.Stretch);
                if (string.Equals(fontName, "CourierNewPS-ItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                        FontStyles.Italic, familyInfo.Stretch);
                if (string.Equals(fontName, "CourierNewPS-BoldItalicMT",
                        StringComparison.OrdinalIgnoreCase))
                    return new WpfFontFamilyInfo(_arialFamily, FontWeights.Bold,
                        FontStyles.Italic, familyInfo.Stretch);

                return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            }

            if (fontName.Equals("MS-Gothic", StringComparison.OrdinalIgnoreCase))
                return new WpfFontFamilyInfo(new FontFamily("MS Gothic"), familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            //return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
            //    familyInfo.Style, familyInfo.Stretch);
            if (fontName.Equals("MS-PGothic", StringComparison.OrdinalIgnoreCase))
                return new WpfFontFamilyInfo(new FontFamily("MS PGothic"), familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);
            if (fontName.Equals("MS Pゴシック", StringComparison.OrdinalIgnoreCase))
                return new WpfFontFamilyInfo(new FontFamily("MS PGothic"), familyInfo.Weight,
                    familyInfo.Style, familyInfo.Stretch);

            return null;
            //return new WpfFontFamilyInfo(_arialFamily, familyInfo.Weight,
            //    familyInfo.Style, familyInfo.Stretch);
        }
    }
}