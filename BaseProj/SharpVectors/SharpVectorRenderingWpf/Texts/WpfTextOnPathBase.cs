// TextOnPathBase.cs by Charles Petzold, September 2008

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public abstract class WpfTextOnPathBase : DependencyObject
    {
        // Dependency properties
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnFontPropertyChanged));

        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(WpfTextOnPathBase),
                new FrameworkPropertyMetadata(OnForegroundPropertyChanged));

        //public static readonly DependencyProperty TextProperty =
        //    TextBlock.TextProperty.AddOwner(typeof(TextOnPathBase),
        //        new FrameworkPropertyMetadata(OnTextPropertyChanged));

        //public static readonly DependencyProperty PathFigureProperty =
        //    DependencyProperty.Register("PathFigure",
        //        typeof(PathFigure),
        //        typeof(TextOnPathBase),
        //        new FrameworkPropertyMetadata(OnPathPropertyChanged));

        // Properties
        public FontFamily FontFamily
        {
            set => SetValue(FontFamilyProperty, value);
            get => (FontFamily)GetValue(FontFamilyProperty);
        }

        public FontStyle FontStyle
        {
            set => SetValue(FontStyleProperty, value);
            get => (FontStyle)GetValue(FontStyleProperty);
        }

        public FontWeight FontWeight
        {
            set => SetValue(FontWeightProperty, value);
            get => (FontWeight)GetValue(FontWeightProperty);
        }

        public FontStretch FontStretch
        {
            set => SetValue(FontStretchProperty, value);
            get => (FontStretch)GetValue(FontStretchProperty);
        }

        public Brush Foreground
        {
            set => SetValue(ForegroundProperty, value);
            get => (Brush)GetValue(ForegroundProperty);
        }

        //public string Text
        //{
        //    set { SetValue(TextProperty, value); }
        //    get { return (string)GetValue(TextProperty); }
        //}

        //public PathFigure PathFigure
        //{
        //    set { SetValue(PathFigureProperty, value); }
        //    get { return (PathFigure)GetValue(PathFigureProperty); }
        //}

        // Property changed handlers
        private static void OnFontPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as WpfTextOnPathBase).OnFontPropertyChanged(args);
        }

        protected abstract void OnFontPropertyChanged(DependencyPropertyChangedEventArgs args);

        private static void OnForegroundPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as WpfTextOnPathBase).OnForegroundPropertyChanged(args);
        }

        protected abstract void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args);

        //static void OnTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    (obj as TextOnPathBase).OnTextPropertyChanged(args);
        //}

        //protected abstract void OnTextPropertyChanged(DependencyPropertyChangedEventArgs args);

        //static void OnPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    (obj as TextOnPathBase).OnPathPropertyChanged(args);
        //}

        //protected abstract void OnPathPropertyChanged(DependencyPropertyChangedEventArgs args);

        // Utility method
        public static double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            var isAlreadyFlattened = true;

            foreach (var pathSegment in pathFigure.Segments)
                if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
                {
                    isAlreadyFlattened = false;
                    break;
                }

            var pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();

            double length = 0;
            var pt1 = pathFigureFlattened.StartPoint;

            foreach (var pathSegment in pathFigureFlattened.Segments)
                if (pathSegment is LineSegment)
                {
                    var pt2 = (pathSegment as LineSegment).Point;
                    length += (pt2 - pt1).Length;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment)
                {
                    var pointCollection = (pathSegment as PolyLineSegment).Points;
                    foreach (var pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        pt1 = pt2;
                    }
                }

            return length;
        }
    }
}