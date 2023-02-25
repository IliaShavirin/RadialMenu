using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaseProj.RecolorableImages
{
    public abstract class BaseRecolorableImage : UserControl
    {
        protected abstract void ShiftColor();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Image = Template.FindName("img", this) as Image;
            ResetToDefaultImgeSource();
            ShiftColor();
        }

        protected void ResetToDefaultImgeSource()
        {
            if (Image != null) Image.Source = Source;
        }

        #region Image dpd

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(Image), typeof(BaseRecolorableImage), new PropertyMetadata(default(Image)));

        public Image Image
        {
            get => (Image)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        #endregion

        #region Source dpd w OnChanged

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(ImageSource), typeof(BaseRecolorableImage),
            new PropertyMetadata(default(ImageSource), OnSourceChanged));

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zis = d as BaseRecolorableImage;
            if (zis == null) return;

            zis.ShiftColor();
        }

        #endregion

        #region Stretch dpd w static ChangedCallback and Virtual OnChanged

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch", typeof(Stretch?), typeof(BaseRecolorableImage),
            new PropertyMetadata(null, StretchChangedCallback));

        public Stretch? Stretch
        {
            get => (Stretch?)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        private static void StretchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zis = d as BaseRecolorableImage;
            if (zis?.Stretch == null) return;

            zis.OnStretchChanged(e);
        }

        protected virtual void OnStretchChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region IsColorShift dpd w OnChanged

        public static readonly DependencyProperty IsColorShiftProperty = DependencyProperty.Register(
            "IsColorShift", typeof(bool), typeof(BaseRecolorableImage),
            new PropertyMetadata(default(bool), OnIsColorShift));

        public bool IsColorShift
        {
            get => (bool)GetValue(IsColorShiftProperty);
            set => SetValue(IsColorShiftProperty, value);
        }

        private static void OnIsColorShift(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zis = d as BaseRecolorableImage;
            if (zis == null) return;

            zis.ShiftColor();
        }

        #endregion
    }
}