using System.Windows;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    public sealed class SvgObject : DependencyObject
    {
        #region Constructors and Destructor

        #endregion

        #region Public Fields

        public const string LinksLayer = "AnimationLayer";
        public const string DrawLayer = "DrawingLayer";

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.RegisterAttached("Id", typeof(string), typeof(SvgObject),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.RegisterAttached("Type", typeof(SvgObjectType), typeof(SvgObject),
                new FrameworkPropertyMetadata(SvgObjectType.None, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(string), typeof(SvgObject),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None));

        #endregion

        #region Public Methods

        public static void SetId(DependencyObject element, string value)
        {
            element.SetValue(IdProperty, value);
        }

        public static string GetId(DependencyObject element)
        {
            return (string)element.GetValue(IdProperty);
        }

        public static void SetType(DependencyObject element, SvgObjectType value)
        {
            element.SetValue(TypeProperty, value);
        }

        public static SvgObjectType GetType(DependencyObject element)
        {
            return (SvgObjectType)element.GetValue(TypeProperty);
        }

        public static void SetTitle(DependencyObject element, string value)
        {
            element.SetValue(TitleProperty, value);
        }

        public static string GetTitle(DependencyObject element)
        {
            return (string)element.GetValue(TitleProperty);
        }

        #endregion
    }
}