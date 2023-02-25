using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

#pragma warning disable CS0618

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfTextOnPathDrawing : WpfTextOnPathBase
    {
        #region Constructors and Destructor

        public WpfTextOnPathDrawing()
        {
            _fontSize = 100;
            _formattedChars = new List<FormattedText>();
            _formattedOrigins = new List<Point>();
        }

        #endregion

        #region Public Properties

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (value >= 6) _fontSize = value;
            }
        }

        #endregion

        #region Private Fields

        private double _fontSize;

        private double _pathLength;
        private double _textLength;

        private List<Point> _formattedOrigins;
        private List<FormattedText> _formattedChars;

        #endregion

        #region Public Methods

        public void BeginTextPath()
        {
            if (_formattedChars == null || _formattedChars.Count != 0)
            {
                _formattedChars = new List<FormattedText>();
                _formattedOrigins = new List<Point>();
            }

            _formattedChars.Clear();
            _formattedOrigins.Clear();

            _textLength = 0;
            _pathLength = 0;

            _fontSize = 100;

            //typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        }

        public void AddTextPath(string text, Point origin)
        {
            if (string.IsNullOrEmpty(text)) return;
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

            foreach (var ch in text)
            {
                var formattedText =
                    new FormattedText(ch.ToString(), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, _fontSize, Foreground);

                _formattedChars.Add(formattedText);
                _formattedOrigins.Add(origin);

                _textLength += formattedText.WidthIncludingTrailingWhitespace;
            }
        }

        public void EndTextPath()
        {
            if (_formattedChars != null) _formattedChars.Clear();

            if (_formattedOrigins != null) _formattedOrigins.Clear();
        }

        public void DrawTextPath(DrawingContext dc, PathGeometry pathGeometry,
            ISvgAnimatedLength startOffset, TextAlignment alignment, SvgTextPathMethod method,
            SvgTextPathSpacing spacing)
        {
            if (alignment == TextAlignment.Right)
                DrawEndAlignedTextPath(dc, pathGeometry, startOffset);
            else
                DrawStartAlignedTextPath(dc, pathGeometry, startOffset, alignment);
        }

        #endregion

        #region Private Methods

        private void DrawStartAlignedTextPath(DrawingContext dc, PathGeometry pathGeometry,
            ISvgAnimatedLength startOffset, TextAlignment alignment)
        {
            if (pathGeometry == null || pathGeometry.Figures == null ||
                pathGeometry.Figures.Count != 1)
                return;

            _pathLength = GetPathFigureLength(pathGeometry.Figures[0]);
            if (_pathLength == 0 || _textLength == 0)
                return;

            //double scalingFactor = pathLength / textLength;
            var scalingFactor = 1.0; // Not scaling the text to fit...
            double progress = 0;
            if (startOffset != null)
            {
                var offsetLength = startOffset.AnimVal;
                if (offsetLength != null)
                    switch (offsetLength.UnitType)
                    {
                        case SvgLengthType.Percentage:
                            if ((float)offsetLength.ValueInSpecifiedUnits != 0)
                                progress += offsetLength.ValueInSpecifiedUnits / 100d;
                            break;
                    }
            }
            //PathGeometry pathGeometry =
            //    new PathGeometry(new PathFigure[] { PathFigure });

            var ptOld = new Point(0, 0);

            for (var i = 0; i < _formattedChars.Count; i++)
            {
                var formText = _formattedChars[i];

                var width = scalingFactor *
                            formText.WidthIncludingTrailingWhitespace;
                var baseline = scalingFactor * formText.Baseline;
                progress += width / 2 / _pathLength;
                Point point, tangent;

                pathGeometry.GetPointAtFractionLength(progress,
                    out point, out tangent);

                if (i != 0)
                    if (point == ptOld)
                        break;

                dc.PushTransform(
                    new TranslateTransform(point.X - width / 2,
                        point.Y - baseline));
                dc.PushTransform(
                    new RotateTransform(Math.Atan2(tangent.Y, tangent.X)
                        * 180 / Math.PI, width / 2, baseline));
                //dc.PushTransform(
                //    new ScaleTransform(scalingFactor, scalingFactor));

                dc.DrawText(formText, _formattedOrigins[i]);
                dc.Pop();
                dc.Pop();
                //dc.Pop();

                progress += width / 2 / _pathLength;

                ptOld = point;
            }
        }

        private void DrawEndAlignedTextPath(DrawingContext dc, PathGeometry pathGeometry,
            ISvgAnimatedLength startOffset)
        {
            if (pathGeometry == null || pathGeometry.Figures == null ||
                pathGeometry.Figures.Count != 1)
                return;

            _pathLength = GetPathFigureLength(pathGeometry.Figures[0]);
            if (_pathLength == 0 || _textLength == 0)
                return;

            //double scalingFactor = pathLength / textLength;
            var scalingFactor = 1.0; // Not scaling the text to fit...
            var progress = 1.0;
            //PathGeometry pathGeometry =
            //    new PathGeometry(new PathFigure[] { PathFigure });

            var ptOld = new Point(0, 0);

            var itemCount = _formattedChars.Count - 1;

            for (var i = itemCount; i >= 0; i--)
            {
                var formText = _formattedChars[i];

                var width = scalingFactor *
                            formText.WidthIncludingTrailingWhitespace;
                var baseline = scalingFactor * formText.Baseline;
                progress -= width / 2 / _pathLength;
                Point point, tangent;

                pathGeometry.GetPointAtFractionLength(progress,
                    out point, out tangent);

                if (i != itemCount)
                    if (point == ptOld)
                        break;

                dc.PushTransform(
                    new TranslateTransform(point.X - width / 2,
                        point.Y - baseline));
                dc.PushTransform(
                    new RotateTransform(Math.Atan2(tangent.Y, tangent.X)
                        * 180 / Math.PI, width / 2, baseline));
                //dc.PushTransform(
                //    new ScaleTransform(scalingFactor, scalingFactor));

                dc.DrawText(formText, _formattedOrigins[i]);
                dc.Pop();
                dc.Pop();
                //dc.Pop();

                progress -= width / 2 / _pathLength;

                ptOld = point;
            }
        }

        //protected override void OnPathPropertyChanged(DependencyPropertyChangedEventArgs args)
        //{
        //    pathLength = GetPathFigureLength(PathFigure);
        //    //TransformVisualChildren();
        //}

        protected override void OnFontPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            //OnTextPropertyChanged(args);
        }

        protected override void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            //OnTextPropertyChanged(args);
        }

        //protected override void OnTextPropertyChanged(DependencyPropertyChangedEventArgs args)
        //{             
        //    //GenerateVisualChildren();
        //}

        //protected virtual void GenerateVisualChildren()
        //{
        //    visualChildren.Clear();

        //    foreach (FormattedText formText in formattedChars)
        //    {
        //        DrawingVisual drawingVisual = new DrawingVisual();

        //        TransformGroup transformGroup = new TransformGroup();
        //        transformGroup.Children.Add(new ScaleTransform());
        //        transformGroup.Children.Add(new RotateTransform());
        //        transformGroup.Children.Add(new TranslateTransform());
        //        drawingVisual.Transform = transformGroup;

        //        DrawingContext dc = drawingVisual.RenderOpen();
        //        dc.DrawText(formText, new Point(0, 0));
        //        dc.Close();

        //        visualChildren.Add(drawingVisual);
        //    }

        //    TransformVisualChildren();
        //}

        //protected virtual void TransformVisualChildren()
        //{
        //    boundingRect = new Rect();

        //    if (pathLength == 0 || textLength == 0)
        //        return;

        //    if (formattedChars.Count != visualChildren.Count)
        //        return;

        //    double scalingFactor = pathLength / textLength;
        //    PathGeometry pathGeometry = 
        //        new PathGeometry(new PathFigure[] { PathFigure });
        //    double progress = 0;
        //    boundingRect = new Rect();

        //    for (int index = 0; index < visualChildren.Count; index++)
        //    {
        //        FormattedText formText = formattedChars[index];
        //        double width = scalingFactor * 
        //                    formText.WidthIncludingTrailingWhitespace;
        //        double baseline = scalingFactor * formText.Baseline;
        //        progress += width / 2 / pathLength;
        //        Point point, tangent;
        //        pathGeometry.GetPointAtFractionLength(progress, 
        //                                    out point, out tangent);

        //        DrawingVisual drawingVisual = 
        //            visualChildren[index] as DrawingVisual;
        //        TransformGroup transformGroup = 
        //            drawingVisual.Transform as TransformGroup;
        //        ScaleTransform scaleTransform = 
        //            transformGroup.Children[0] as ScaleTransform;
        //        RotateTransform rotateTransform = 
        //            transformGroup.Children[1] as RotateTransform;
        //        TranslateTransform translateTransform = 
        //            transformGroup.Children[2] as TranslateTransform;

        //        scaleTransform.ScaleX = scalingFactor;
        //        scaleTransform.ScaleY = scalingFactor;
        //        rotateTransform.Angle = 
        //            Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;
        //        rotateTransform.CenterX = width / 2;
        //        rotateTransform.CenterY = baseline;
        //        translateTransform.X = point.X - width / 2;
        //        translateTransform.Y = point.Y - baseline;

        //        Rect rect = drawingVisual.ContentBounds;
        //        rect.Transform(transformGroup.Value);
        //        boundingRect.Union(rect);

        //        progress += width / 2 / pathLength;
        //    }
        //    InvalidateMeasure();
        //}

        //protected override int VisualChildrenCount
        //{
        //    get
        //    {
        //        return visualChildren.Count;
        //    }
        //}

        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index < 0 || index >= visualChildren.Count)
        //        throw new ArgumentOutOfRangeException("index");

        //    return visualChildren[index];
        //}

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    return (Size)boundingRect.BottomRight;
        //}

        #endregion
    }
}
#pragma warning restore CS0618