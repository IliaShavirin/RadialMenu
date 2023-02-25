using System.Windows;
using System.Windows.Media;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public class WpfFontFamilyInfo
    {
        public static readonly WpfFontFamilyInfo Empty = new WpfFontFamilyInfo(null,
            FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);

        public WpfFontFamilyInfo(FontFamily family, FontWeight weight,
            FontStyle style, FontStretch stretch)
        {
            Family = family;
            Weight = weight;
            Style = style;
            Stretch = stretch;
        }

        public bool IsEmpty => Family == null;

        public FontFamily Family { get; }

        public FontWeight Weight { get; }

        public FontStyle Style { get; }

        public FontStretch Stretch { get; }
    }
}