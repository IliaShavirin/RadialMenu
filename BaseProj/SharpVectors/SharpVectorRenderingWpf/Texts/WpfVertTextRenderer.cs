using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.Text;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

#pragma warning disable CS0618

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfVertTextRenderer : WpfTextRenderer
    {
        #region Constructors and Destructor

        public WpfVertTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Methods

        #region RenderSingleLineText Method

        public override void RenderSingleLineText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var vertOrientation = -1;
            var horzOrientation = -1;
            var orientationText = element.GetPropertyValue("glyph-orientation-vertical");
            if (!string.IsNullOrEmpty(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue)) vertOrientation = (int)orientationValue;
            }

            orientationText = element.GetPropertyValue("glyph-orientation-horizontal");
            if (!string.IsNullOrEmpty(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue)) horzOrientation = (int)orientationValue;
            }

            var startPoint = ctp;
            var textRunList = WpfTextRun.BreakWords(text,
                vertOrientation, horzOrientation);

            for (var tr = 0; tr < textRunList.Count; tr++)
            {
                // For unknown reasons, FormattedText will split a text like "-70%" into two parts "-"
                // and "70%". We provide a shift to account for the split...
                double baselineShiftX = 0;
                double baselineShiftY = 0;

                var textRun = textRunList[tr];

                var verticalGroup = new DrawingGroup();

                var verticalContext = verticalGroup.Open();
                var currentContext = _textContext;

                _textContext = verticalContext;

                DrawSingleLineText(element, ref ctp, textRun, rotate, placement);

                verticalContext.Close();

                _textContext = currentContext;

                if (verticalGroup.Children.Count == 1)
                {
                    var textGroup = verticalGroup.Children[0] as DrawingGroup;
                    if (textGroup != null) verticalGroup = textGroup;
                }

                var runText = textRun.Text;
                var charCount = runText.Length;

                double totalHeight = 0;
                var drawings = verticalGroup.Children;
                var itemCount = drawings != null ? drawings.Count : 0;
                for (var i = 0; i < itemCount; i++)
                {
                    var textDrawing = drawings[i];
                    var textGroup = textDrawing as DrawingGroup;

                    if (vertOrientation == -1)
                    {
                        if (textGroup != null)
                        {
                            for (var j = 0; j < textGroup.Children.Count; j++)
                            {
                                var glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    if (textRun.IsLatin)
                                    {
                                        var glyphRun = glyphDrawing.GlyphRun;

                                        var glyphIndices = glyphRun.GlyphIndices;
                                        var allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                        var lastAdvanceWeight =
                                            allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] *
                                            glyphRun.FontRenderingEmSize;

                                        totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                    }
                                    else
                                    {
                                        totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                            baselineShiftX, baselineShiftY, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                            {
                                if (textRun.IsLatin)
                                {
                                    var glyphRun = glyphDrawing.GlyphRun;

                                    var glyphIndices = glyphRun.GlyphIndices;
                                    var allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                    var lastAdvanceWeight =
                                        allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] *
                                        glyphRun.FontRenderingEmSize;

                                    totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                }
                                else
                                {
                                    totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, false);
                                }
                            }
                        }
                    }
                    else if (vertOrientation == 0)
                    {
                        if (textGroup != null)
                        {
                            for (var j = 0; j < textGroup.Children.Count; j++)
                            {
                                var glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    baselineShiftX = ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, textRun.IsLatin);
                                    totalHeight += baselineShiftX;
                                }
                            }
                        }
                        else
                        {
                            var glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (textDrawing != null)
                                totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                    baselineShiftX, baselineShiftY, textRun.IsLatin);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                if (!IsMeasuring) _textContext.DrawDrawing(verticalGroup);

                if (tr < textRunList.Count)
                {
                    ctp.X = startPoint.X;
                    ctp.Y = startPoint.Y + totalHeight;
                    startPoint.Y += totalHeight;
                }
            }
        }

        #endregion

        #region RenderTextRun Method

        public override void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var vertOrientation = -1;
            var horzOrientation = -1;

            var orientationText = element.GetPropertyValue("glyph-orientation-vertical");
            if (!string.IsNullOrEmpty(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue)) vertOrientation = (int)orientationValue;
            }

            orientationText = element.GetPropertyValue("glyph-orientation-horizontal");
            if (!string.IsNullOrEmpty(orientationText))
            {
                double orientationValue = 0;
                if (double.TryParse(orientationText, out orientationValue)) horzOrientation = (int)orientationValue;
            }

            var startPoint = ctp;

            var textRunList = WpfTextRun.BreakWords(text,
                vertOrientation, horzOrientation);

            for (var tr = 0; tr < textRunList.Count; tr++)
            {
                // For unknown reasons, FormattedText will split a text like "-70%" into two parts "-"
                // and "70%". We provide a shift to account for the split...
                double baselineShiftX = 0;
                double baselineShiftY = 0;
                var textRun = textRunList[tr];

                if (!textRun.IsLatin) textRun = textRunList[tr];

                var verticalGroup = new DrawingGroup();

                var verticalContext = verticalGroup.Open();
                var currentContext = _textContext;

                _textContext = verticalContext;

                DrawTextRun(element, ref ctp, textRun, rotate, placement);

                verticalContext.Close();

                _textContext = currentContext;

                if (verticalGroup.Children.Count == 1)
                {
                    var textGroup = verticalGroup.Children[0] as DrawingGroup;
                    if (textGroup != null) verticalGroup = textGroup;
                }

                double totalHeight = 0;
                var drawings = verticalGroup.Children;
                var itemCount = drawings != null ? drawings.Count : 0;
                for (var i = 0; i < itemCount; i++)
                {
                    var textDrawing = drawings[i];
                    var textGroup = textDrawing as DrawingGroup;

                    if (vertOrientation == -1)
                    {
                        if (textGroup != null)
                        {
                            for (var j = 0; j < textGroup.Children.Count; j++)
                            {
                                var glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    if (textRun.IsLatin)
                                    {
                                        var glyphRun = glyphDrawing.GlyphRun;

                                        var glyphIndices = glyphRun.GlyphIndices;
                                        var allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                        var lastAdvanceWeight =
                                            allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] *
                                            glyphRun.FontRenderingEmSize;

                                        totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                    }
                                    else
                                    {
                                        totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                            baselineShiftX, baselineShiftY, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                            {
                                if (textRun.IsLatin)
                                {
                                    var glyphRun = glyphDrawing.GlyphRun;

                                    var glyphIndices = glyphRun.GlyphIndices;
                                    var allGlyphWeights = glyphRun.GlyphTypeface.AdvanceWidths;
                                    var lastAdvanceWeight =
                                        allGlyphWeights[glyphIndices[glyphIndices.Count - 1]] *
                                        glyphRun.FontRenderingEmSize;

                                    totalHeight += glyphRun.ComputeAlignmentBox().Width + lastAdvanceWeight / 2d;
                                }
                                else
                                {
                                    totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, false);
                                }
                            }
                        }
                    }
                    else if (vertOrientation == 0)
                    {
                        if (textGroup != null)
                        {
                            for (var j = 0; j < textGroup.Children.Count; j++)
                            {
                                var glyphDrawing = textGroup.Children[j] as GlyphRunDrawing;
                                if (glyphDrawing != null)
                                {
                                    baselineShiftX = ChangeGlyphOrientation(glyphDrawing,
                                        baselineShiftX, baselineShiftY, textRun.IsLatin);
                                    totalHeight += baselineShiftX;
                                }
                            }
                        }
                        else
                        {
                            var glyphDrawing = textDrawing as GlyphRunDrawing;
                            if (glyphDrawing != null)
                                totalHeight += ChangeGlyphOrientation(glyphDrawing,
                                    baselineShiftX, baselineShiftY, textRun.IsLatin);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                if (!IsMeasuring) _textContext.DrawDrawing(verticalGroup);

                if (tr < textRunList.Count)
                {
                    ctp.X = startPoint.X;
                    ctp.Y = startPoint.Y + totalHeight;
                    startPoint.Y += totalHeight;
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region DrawSingleLineText Method

        private void DrawSingleLineText(SvgTextContentElement element, ref Point ctp,
            WpfTextRun textRun, double rotate, WpfTextPlacement placement)
        {
            if (textRun == null || textRun.IsEmpty)
                return;

            var text = textRun.Text;
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

            var letterSpacing = element.GetAttribute("letter-spacing");
            if (string.IsNullOrEmpty(letterSpacing))
            {
                var formattedText = new FormattedText(text,
                    textRun.IsLatin ? _drawContext.EnglishCultureInfo : _drawContext.CultureInfo,
                    stringFormat.Direction, new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                    emSize, textBrush);

                if (IsMeasuring)
                {
                    AddTextWidth(formattedText.WidthIncludingTrailingWhitespace);
                    return;
                }

                formattedText.TextAlignment = stringFormat.Alignment;
                formattedText.Trimming = stringFormat.Trimming;

                if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                //float xCorrection = 0;
                //if (alignment == TextAlignment.Left)
                //    xCorrection = emSize * 1f / 6f;
                //else if (alignment == TextAlignment.Right)
                //    xCorrection = -emSize * 1f / 6f;

                var yCorrection = formattedText.Baseline;
                //float yCorrection = 0;

                if (textRun.IsLatin && textRun.VerticalOrientation == -1)
                    yCorrection = yCorrection + formattedText.OverhangAfter * 1.5;

                var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                var rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _textContext.PushTransform(rotateAt);

                _textContext.DrawText(formattedText, textPoint);

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
            else
            {
                var rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _textContext.PushTransform(rotateAt);

                var spacing = Convert.ToDouble(letterSpacing);
                for (var i = 0; i < text.Length; i++)
                {
                    var formattedText = new FormattedText(new string(text[i], 1),
                        textRun.IsLatin ? _drawContext.EnglishCultureInfo : _drawContext.CultureInfo,
                        stringFormat.Direction,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    if (IsMeasuring)
                    {
                        AddTextWidth(formattedText.WidthIncludingTrailingWhitespace);
                        continue;
                    }

                    formattedText.TextAlignment = stringFormat.Alignment;
                    formattedText.Trimming = stringFormat.Trimming;

                    if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    var yCorrection = formattedText.Baseline;

                    var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    _textContext.DrawText(formattedText, textPoint);

                    //float bboxWidth = (float)formattedText.Width;
                    var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    ctp.X += bboxWidth + spacing;
                }

                if (rotateAt != null) _textContext.Pop();
            }
        }

        #endregion

        #region DrawTextRun Method

        private void DrawTextRun(SvgTextContentElement element, ref Point ctp,
            WpfTextRun textRun, double rotate, WpfTextPlacement placement)
        {
            if (textRun == null || textRun.IsEmpty)
                return;

            var text = textRun.Text;
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

            var letterSpacing = element.GetAttribute("letter-spacing");
            if (string.IsNullOrEmpty(letterSpacing))
            {
                var formattedText = new FormattedText(text,
                    textRun.IsLatin ? _drawContext.EnglishCultureInfo : _drawContext.CultureInfo,
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

                var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                var rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _textContext.PushTransform(rotateAt);

                _textContext.DrawText(formattedText, textPoint);

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
            else
            {
                var rotateAt = new RotateTransform(90, ctp.X, ctp.Y);
                _textContext.PushTransform(rotateAt);

                var spacing = Convert.ToSingle(letterSpacing);
                for (var i = 0; i < text.Length; i++)
                {
                    var formattedText = new FormattedText(new string(text[i], 1),
                        textRun.IsLatin ? _drawContext.EnglishCultureInfo : _drawContext.CultureInfo,
                        stringFormat.Direction,
                        new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                        emSize, textBrush);

                    formattedText.Trimming = stringFormat.Trimming;
                    formattedText.TextAlignment = stringFormat.Alignment;

                    if (textDecors != null && textDecors.Count != 0) formattedText.SetTextDecorations(textDecors);

                    //float xCorrection = 0;
                    //if (alignment == TextAlignment.Left)
                    //    xCorrection = emSize * 1f / 6f;
                    //else if (alignment == TextAlignment.Right)
                    //    xCorrection = -emSize * 1f / 6f;

                    var yCorrection = formattedText.Baseline;

                    var textPoint = new Point(ctp.X, ctp.Y - yCorrection);

                    _textContext.DrawText(formattedText, textPoint);

                    //float bboxWidth = (float)formattedText.Width;
                    var bboxWidth = formattedText.WidthIncludingTrailingWhitespace;
                    if (alignment == TextAlignment.Center)
                        bboxWidth /= 2f;
                    else if (alignment == TextAlignment.Right)
                        bboxWidth = 0;

                    //ctp.X += bboxWidth + emSize / 4 + spacing;
                    ctp.X += bboxWidth + spacing;
                }

                if (rotateAt != null) _textContext.Pop();
            }
        }

        #endregion

        #region ChangeGlyphOrientation Method

        private double ChangeGlyphOrientation(GlyphRunDrawing glyphDrawing,
            double baselineShiftX, double baselineShiftY, bool isLatin)
        {
            if (glyphDrawing == null) return 0;
            var glyphRun = glyphDrawing.GlyphRun;

            var verticalRun = new GlyphRun();
            ISupportInitialize glyphInit = verticalRun;
            glyphInit.BeginInit();

            verticalRun.IsSideways = true;

            List<double> advancedHeights = null;

            double totalHeight = 0;

            var glyphIndices = glyphRun.GlyphIndices;
            var advancedCount = glyphIndices.Count;
            //{
            //    double textHeight = glyphRun.ComputeInkBoundingBox().Height + glyphRun.ComputeAlignmentBox().Height;
            //    textHeight = textHeight / 2.0d;

            //    totalHeight = advancedCount * textHeight;
            //    advancedHeights = new List<double>(advancedCount);
            //    for (int k = 0; k < advancedCount; k++)
            //    {
            //        advancedHeights.Add(textHeight);
            //    }
            //}
            advancedHeights = new List<double>(advancedCount);
            var allGlyphHeights = glyphRun.GlyphTypeface.AdvanceHeights;
            var fontSize = glyphRun.FontRenderingEmSize;
            for (var k = 0; k < advancedCount; k++)
            {
                var tempValue = allGlyphHeights[glyphIndices[k]] * fontSize;
                advancedHeights.Add(tempValue);
                totalHeight += tempValue;
            }

            var baselineOrigin = glyphRun.BaselineOrigin;
            if (isLatin)
            {
                baselineOrigin.X += baselineShiftX;
                baselineOrigin.Y += baselineShiftY;
            }

            //verticalRun.AdvanceWidths     = glyphRun.AdvanceWidths;
            verticalRun.AdvanceWidths = advancedHeights;
            verticalRun.BaselineOrigin = baselineOrigin;
            verticalRun.BidiLevel = glyphRun.BidiLevel;
            verticalRun.CaretStops = glyphRun.CaretStops;
            verticalRun.Characters = glyphRun.Characters;
            verticalRun.ClusterMap = glyphRun.ClusterMap;
            verticalRun.DeviceFontName = glyphRun.DeviceFontName;
            verticalRun.FontRenderingEmSize = glyphRun.FontRenderingEmSize;
            verticalRun.GlyphIndices = glyphRun.GlyphIndices;
            verticalRun.GlyphOffsets = glyphRun.GlyphOffsets;
            verticalRun.GlyphTypeface = glyphRun.GlyphTypeface;
            verticalRun.Language = glyphRun.Language;

            glyphInit.EndInit();

            glyphDrawing.GlyphRun = verticalRun;

            return totalHeight;
        }

        #endregion

        #endregion
    }
}
#pragma warning restore CS0618