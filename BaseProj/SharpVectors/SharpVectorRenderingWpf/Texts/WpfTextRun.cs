using System.Collections.Generic;
using System.Text;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfTextRun
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public WpfTextRun()
        {
            VerticalOrientation = -1;
            HorizontalOrientation = -1;
            Text = string.Empty;
            IsLatin = true;
        }

        public WpfTextRun(string text, bool isLatin, int vertOrientation,
            int horzOrientation)
        {
            Text = text;
            IsLatin = isLatin;
            VerticalOrientation = vertOrientation;
            HorizontalOrientation = horzOrientation;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty => string.IsNullOrEmpty(Text);

        public bool IsLatin { get; set; }

        public int VerticalOrientation { get; set; }

        public int HorizontalOrientation { get; set; }

        public string Text { get; set; }

        #endregion

        #region Public Methods

        public static bool IsLatinGlyph(char ch)
        {
            if (ch < 256) return true;

            return false;
        }

        public static IList<WpfTextRun> BreakWords(string text)
        {
            return BreakWords(text, -1, -1);
        }

        public static IList<WpfTextRun> BreakWords(string text, int vertOrientation,
            int horzOrientation)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var textRunList = new List<WpfTextRun>();

            var builder = new StringBuilder();

            var textLength = text.Length;
            var isLatinStart = IsLatinGlyph(text[0]);
            for (var i = 0; i < textLength; i++)
            {
                var nextChar = text[i];
                if (IsLatinGlyph(nextChar) == isLatinStart)
                {
                    builder.Append(nextChar);
                }
                else
                {
                    textRunList.Add(new WpfTextRun(builder.ToString(), isLatinStart,
                        vertOrientation, horzOrientation));

                    builder.Length = 0;
                    isLatinStart = IsLatinGlyph(nextChar);

                    builder.Append(nextChar);
                }
            }

            if (builder.Length != 0)
                textRunList.Add(new WpfTextRun(builder.ToString(), isLatinStart,
                    vertOrientation, horzOrientation));

            return textRunList;
        }

        #endregion
    }
}