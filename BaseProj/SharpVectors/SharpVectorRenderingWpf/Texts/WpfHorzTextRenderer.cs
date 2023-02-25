using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.Text;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

#pragma warning disable CS0618

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfHorzTextRenderer : WpfTextRenderer
    {
        #region Constructors and Destructor

        public WpfHorzTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Methods

        public override void RenderSingleLineText(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var emSize = GetComputedFontSize(element);
            var fontFamily = GetTextFontFamily(element, emSize);

            var fontStyle = GetTextFontStyle(element);
            var fontWeight = GetTextFontWeight(element);

            var fontStretch = GetTextFontStretch(element);

            var stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            var fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!string.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                var currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                var familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    currentFamily, _drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily = familyInfo.Family;
                    fontWeight = familyInfo.Weight;
                    fontStyle = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            var fillPaint = new WpfSvgPaint(_drawContext, element, "fill");
            var textBrush = fillPaint.GetBrush();

            var strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
            var textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
                return;
            if (textBrush == null)
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;

            var textDecors = GetTextDecoration(element);
            var alignment = stringFormat.Alignment;

            var hasWordSpacing = false;
            var wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing = 0;
            if (!string.IsNullOrEmpty(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && (float)wordSpacing != 0)
                hasWordSpacing = true;

            var hasLetterSpacing = false;
            var letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!string.IsNullOrEmpty(letterSpaceText) &&
                double.TryParse(letterSpaceText, out letterSpacing) && (float)letterSpacing != 0)
                hasLetterSpacing = true;

            var isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            var textPosCount = 0;
            if (placement != null && placement.HasPositions)
            {
                textPositions = placement.Positions;
                textPosCount = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                for (var i = 0; i < text.Length; i++)
                {
                    var formattedText = new FormattedText(new string(text[i], 1),
                        _drawContext.CultureInfo, stringFormat.Direction,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    formattedText.TextAlignment = stringFormat.Alignment;
                    formattedText.Trimming = stringFormat.Trimming;

                    if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && i < textPosCount) textPosition = textPositions[i];

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    var yCorrection = formattedText.Baseline;

                    var rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            var pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }

                        rotateAngle = (float)textPosition.Value.Rotation;
                    }

                    var textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (rotateAngle != 0)
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _textContext.PushTransform(rotateAt);
                    }

                    var textPoint = new Point(textStart.X, textStart.Y - yCorrection);

                    if (textPen != null || _drawContext.TextAsGeometry)
                    {
                        var textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _textContext.DrawGeometry(textBrush, textPen,
                                ExtractTextPathGeometry(textGeometry));

                            IsTextPath = true;
                        }
                        else
                        {
                            _textContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing) ctp.X += bboxWidth + letterSpacing;
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                            ctp.X += wordSpacing;
                        else
                            ctp.X += bboxWidth + wordSpacing;
                    }
                    else
                    {
                        if (!hasLetterSpacing) ctp.X += bboxWidth;
                    }

                    if (rotateAt != null) _textContext.Pop();
                }
            }
            else
            {
                var formattedText = new FormattedText(text, _drawContext.CultureInfo,
                    stringFormat.Direction, new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                    emSize, textBrush);

                formattedText.TextAlignment = stringFormat.Alignment;
                formattedText.Trimming = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                var yCorrection = formattedText.Baseline;

                var rotateAngle = (float)rotate;
                var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (rotateAngle != 0)
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _textContext.PushTransform(rotateAt);
                }

                if (textPen != null || _drawContext.TextAsGeometry)
                {
                    var textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _textContext.DrawGeometry(textBrush, textPen,
                            ExtractTextPathGeometry(textGeometry));

                        IsTextPath = true;
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _textContext.DrawText(formattedText, textPoint);
                }

                //float bboxWidth = (float)formattedText.Width;
                var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null) _textContext.Pop();
            }
        }

        public override void RenderTextRun(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var emSize = GetComputedFontSize(element);
            var fontFamily = GetTextFontFamily(element, emSize);

            var fontStyle = GetTextFontStyle(element);
            var fontWeight = GetTextFontWeight(element);

            var fontStretch = GetTextFontStretch(element);

            var stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            var fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!string.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                var currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                var familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    currentFamily, _drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily = familyInfo.Family;
                    fontWeight = familyInfo.Weight;
                    fontStyle = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            var fillPaint = new WpfSvgPaint(_drawContext, element, "fill");
            var textBrush = fillPaint.GetBrush();

            var strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
            var textPen = strokePaint.GetPen();

            if (textBrush == null && textPen == null)
                return;
            if (textBrush == null)
                // If here, then the pen is not null, and so the fill cannot be null.
                // We set this to transparent for stroke only text path...
                textBrush = Brushes.Transparent;

            var textDecors = GetTextDecoration(element);
            if (textDecors == null)
            {
                var textElement = element.ParentNode as SvgTextElement;

                if (textElement != null) textDecors = GetTextDecoration(textElement);
            }

            var alignment = stringFormat.Alignment;

            var hasWordSpacing = false;
            var wordSpaceText = element.GetAttribute("word-spacing");
            double wordSpacing = 0;
            if (!string.IsNullOrEmpty(wordSpaceText) &&
                double.TryParse(wordSpaceText, out wordSpacing) && (float)wordSpacing != 0)
                hasWordSpacing = true;

            var hasLetterSpacing = false;
            var letterSpaceText = element.GetAttribute("letter-spacing");
            double letterSpacing = 0;
            if (!string.IsNullOrEmpty(letterSpaceText) &&
                double.TryParse(letterSpaceText, out letterSpacing) && (float)letterSpacing != 0)
                hasLetterSpacing = true;

            var isRotatePosOnly = false;

            IList<WpfTextPosition> textPositions = null;
            var textPosCount = 0;
            if (placement != null && placement.HasPositions)
            {
                textPositions = placement.Positions;
                textPosCount = textPositions.Count;
                isRotatePosOnly = placement.IsRotateOnly;
            }

            if (hasLetterSpacing || hasWordSpacing || textPositions != null)
            {
                var spacing = Convert.ToDouble(letterSpacing);
                for (var i = 0; i < text.Length; i++)
                {
                    var formattedText = new FormattedText(new string(text[i], 1),
                        _drawContext.CultureInfo, stringFormat.Direction,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    if (IsMeasuring)
                    {
                        AddTextWidth(formattedText.WidthIncludingTrailingWhitespace);
                        continue;
                    }

                    formattedText.Trimming = stringFormat.Trimming;
                    formattedText.TextAlignment = stringFormat.Alignment;

                    if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                    WpfTextPosition? textPosition = null;
                    if (textPositions != null && i < textPosCount) textPosition = textPositions[i];

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    var yCorrection = formattedText.Baseline;

                    var rotateAngle = (float)rotate;
                    if (textPosition != null)
                    {
                        if (!isRotatePosOnly)
                        {
                            var pt = textPosition.Value.Location;
                            ctp.X = pt.X;
                            ctp.Y = pt.Y;
                        }

                        rotateAngle = (float)textPosition.Value.Rotation;
                    }

                    var textStart = ctp;

                    RotateTransform rotateAt = null;
                    if (rotateAngle != 0)
                    {
                        rotateAt = new RotateTransform(rotateAngle, textStart.X, textStart.Y);
                        _textContext.PushTransform(rotateAt);
                    }

                    var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    if (textPen != null || _drawContext.TextAsGeometry)
                    {
                        var textGeometry = formattedText.BuildGeometry(textPoint);
                        if (textGeometry != null && !textGeometry.IsEmpty())
                        {
                            _textContext.DrawGeometry(textBrush, textPen,
                                ExtractTextPathGeometry(textGeometry));

                            IsTextPath = true;
                        }
                        else
                        {
                            _textContext.DrawText(formattedText, textPoint);
                        }
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }

                    //float bboxWidth = (float)formattedText.Width;
                    var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    if (hasLetterSpacing) ctp.X += bboxWidth + letterSpacing;
                    if (hasWordSpacing && char.IsWhiteSpace(text[i]))
                    {
                        if (hasLetterSpacing)
                            ctp.X += wordSpacing;
                        else
                            ctp.X += bboxWidth + wordSpacing;
                    }
                    else
                    {
                        if (!hasLetterSpacing) ctp.X += bboxWidth;
                    }

                    if (rotateAt != null) _textContext.Pop();
                }
            }
            else
            {
                var formattedText = new FormattedText(text, _drawContext.CultureInfo,
                    stringFormat.Direction, new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                    emSize, textBrush);

                if (IsMeasuring)
                {
                    AddTextWidth(formattedText.WidthIncludingTrailingWhitespace);
                    return;
                }

                if (alignment == TextAlignment.Center && TextWidth > 0) alignment = TextAlignment.Left;

                formattedText.TextAlignment = alignment;
                formattedText.Trimming = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                var yCorrection = formattedText.Baseline;

                var rotateAngle = (float)rotate;
                var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                RotateTransform rotateAt = null;
                if (rotateAngle != 0)
                {
                    rotateAt = new RotateTransform(rotateAngle, ctp.X, ctp.Y);
                    _textContext.PushTransform(rotateAt);
                }

                if (textPen != null || _drawContext.TextAsGeometry)
                {
                    var textGeometry = formattedText.BuildGeometry(textPoint);
                    if (textGeometry != null && !textGeometry.IsEmpty())
                    {
                        _textContext.DrawGeometry(textBrush, textPen,
                            ExtractTextPathGeometry(textGeometry));

                        IsTextPath = true;
                    }
                    else
                    {
                        _textContext.DrawText(formattedText, textPoint);
                    }
                }
                else
                {
                    _textContext.DrawText(formattedText, textPoint);
                }

                //float bboxWidth = (float)formattedText.Width;
                var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                if (alignment == TextAlignment.Center)
                    bboxWidth /= 2f;
                else if (alignment == TextAlignment.Right)
                    bboxWidth = 0;

                //ctp.X += bboxWidth + emSize / 4;
                ctp.X += bboxWidth;

                if (rotateAt != null) _textContext.Pop();
            }
        }

        #endregion
    }
}
#pragma warning restore CS0618