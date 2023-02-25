using System.Windows;
using System.Windows.Media;
using BaseProj.RecolorableImages;

namespace BaseProj.Buttons
{
    /// <summary>
    ///     Логика взаимодействия для ButtonSVG.xaml
    /// </summary>
    public partial class ButtonSVG : BaseColoredButton
    {
        private static SVGControl _svgImage;

        public static readonly DependencyProperty DrawingProperty = DependencyProperty.Register(
            "Drawing", typeof(Drawing), typeof(ButtonSVG), new PropertyMetadata(default(Drawing)));

        public static readonly DependencyProperty ColorShiftBrushProperty = DependencyProperty.Register(
            "ColorShiftBrush", typeof(Brush), typeof(ButtonSVG), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ButtonSVG), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SVGMarginProperty = DependencyProperty.Register(
            "SVGMargin", typeof(Thickness), typeof(ButtonSVG), new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty SVGWidthProperty = DependencyProperty.Register(
            "SVGWidth", typeof(double), typeof(ButtonSVG), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty SVGHeightProperty = DependencyProperty.Register(
            "SVGHeight", typeof(double), typeof(ButtonSVG), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty DotMarginProperty = DependencyProperty.Register(
            "DotMargin", typeof(Thickness), typeof(ButtonSVG), new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty IsDotContentProperty = DependencyProperty.Register(
            "IsDotContent", typeof(bool), typeof(ButtonSVG), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DotContentProperty = DependencyProperty.Register(
            "DotContent", typeof(object), typeof(ButtonSVG), new PropertyMetadata(null));

        public static readonly DependencyProperty HighlightedBackgroundOpacityProperty = DependencyProperty.Register(
            "HighlightedBackgroundOpacity", typeof(double), typeof(ButtonSVG), new PropertyMetadata(1d));

        public ButtonSVG()
        {
            InitializeComponent();
        }

        public object DotContent
        {
            get => GetValue(DotContentProperty);
            set => SetValue(DotContentProperty, value);
        }

        public bool IsDotContent
        {
            get => (bool)GetValue(IsDotContentProperty);
            set => SetValue(IsDotContentProperty, value);
        }

        public Thickness DotMargin
        {
            get => (Thickness)GetValue(DotMarginProperty);
            set => SetValue(DotMarginProperty, value);
        }

        public Brush ColorShiftBrush
        {
            get => (Brush)GetValue(ColorShiftBrushProperty);
            set => SetValue(ColorShiftBrushProperty, value);
        }

        public Drawing Drawing
        {
            get => (Drawing)GetValue(DrawingProperty);
            set => SetValue(DrawingProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Thickness SVGMargin
        {
            get => (Thickness)GetValue(SVGMarginProperty);
            set => SetValue(SVGMarginProperty, value);
        }

        public double SVGWidth
        {
            get => (double)GetValue(SVGWidthProperty);
            set => SetValue(SVGWidthProperty, value);
        }

        public double SVGHeight
        {
            get => (double)GetValue(SVGHeightProperty);
            set => SetValue(SVGHeightProperty, value);
        }

        public double HighlightedBackgroundOpacity
        {
            get => (double)GetValue(HighlightedBackgroundOpacityProperty);
            set => SetValue(HighlightedBackgroundOpacityProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _svgImage = Template.FindName("SvgImage", this) as SVGControl;
        }
    }
}