using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BaseProj.AttachedBehaviours
{
    public class ImageRecolor
    {
        public static readonly DependencyProperty ColorShiftProperty = DependencyProperty.RegisterAttached(
            "ColorShift", typeof(Color), typeof(ImageRecolor),
            new PropertyMetadata(default(Color), OnColorShiftChanged));

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(ImageRecolor), new PropertyMetadata(default(bool), OnIsEnabledChanged));

        private static readonly Dictionary<int, ImageSource> InitialImageSources = new Dictionary<int, ImageSource>();

        public static void SetColorShift(DependencyObject element, Color value)
        {
            element.SetValue(ColorShiftProperty, value);
        }

        public static Color GetColorShift(DependencyObject element)
        {
            return (Color)element.GetValue(ColorShiftProperty);
        }


        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void OnColorShiftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;
            if (img == null) return;

            if (GetIsEnabled(img))
                ChangeColor(img);
            else
                RestoreInitialImageSource(img);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;
            if (img == null) return;

            if (GetIsEnabled(img))
                ChangeColor(img);
            //img.SourceUpdated += ImgOnSourceUpdated;
            //img.Loaded += ImgOnLoaded;
            else
                RestoreInitialImageSource(img);
            //img.SourceUpdated -= ImgOnSourceUpdated;
            //img.Unloaded -= ImgOnLoaded;
        }

        private static void ImgOnLoaded(object s, RoutedEventArgs e)
        {
            var img = s as Image;
            if (img == null) return;

            ChangeColor(img);
        }

        private static void ImgOnSourceUpdated(object s, DataTransferEventArgs e)
        {
            var img = s as Image;
            if (img == null) return;

            ChangeColor(img);
        }

        private static void ChangeColor(Image img)
        {
            var image = img.Source as BitmapSource;
            if (image == null) return;

            var imgHash = img.GetHashCode();
            if (!InitialImageSources.ContainsKey(imgHash)) InitialImageSources.Add(imgHash, img.Source);

            ChangeImageColor(img, GetColorShift(img));
        }

        public static void ChangeImageColor(Image img, Color color)
        {
            if (img == null)
                return;

            var imgAsBitMapSource = img.Source as BitmapSource;
            if (imgAsBitMapSource != null)
            {
                var inputRed = color.R;
                var inputGreen = color.G;
                var inputBlue = color.B;
                var inputAlpha = color.A;

                var pixels = new byte[imgAsBitMapSource.PixelWidth * imgAsBitMapSource.PixelHeight * 4];
                imgAsBitMapSource.CopyPixels(pixels, imgAsBitMapSource.PixelWidth * 4, 0);

                // Modify the white pixels
                for (var i = 0; i < pixels.Length / 4; ++i)
                {
                    var b = pixels[i * 4];
                    var g = pixels[i * 4 + 1];
                    var r = pixels[i * 4 + 2];
                    var a = pixels[i * 4 + 3];

                    if (a != 0)
                    {
                        // This code applies mask to existing color. This is not very useful i guess
                        //byte outputRed = (byte)((double)r / 255 * inputRed);
                        //byte outputGreen = (byte)((double)g / 255 * inputGreen);
                        //byte outputBlue = (byte)((double)b / 255 * inputBlue);

                        //pixels[i * 4 + 2] = outputRed;
                        //pixels[i * 4 + 1] = outputGreen;
                        //pixels[i * 4] = outputBlue;

                        pixels[i * 4 + 3] = (byte)(a * inputAlpha / 255.0);
                        pixels[i * 4 + 2] = inputRed;
                        pixels[i * 4 + 1] = inputGreen;
                        pixels[i * 4] = inputBlue;
                    }
                }

                // Write the modified pixels into a new bitmap and use that as the source of an Image
                var bmp = new WriteableBitmap(imgAsBitMapSource.PixelWidth, imgAsBitMapSource.PixelHeight,
                    imgAsBitMapSource.DpiX, imgAsBitMapSource.DpiY, PixelFormats.Bgra32, null);
                bmp.WritePixels(new Int32Rect(0, 0, imgAsBitMapSource.PixelWidth, imgAsBitMapSource.PixelHeight),
                    pixels,
                    imgAsBitMapSource.PixelWidth * 4, 0);
                img.Source = bmp;
            }
        }

        public static void ChangeSVGColor(Image img, Brush brush, bool isColorShiftFill = true,
            bool isColorShiftStroke = true, bool isColorShiftInvisible = true)
        {
            var imgAsDrawingGroup = img.Source as DrawingImage;
            if (imgAsDrawingGroup != null)
                ChangeColorForDrawing(imgAsDrawingGroup.Drawing, brush, isColorShiftFill, isColorShiftStroke,
                    isColorShiftInvisible);
        }

        public static void ChangeSVGColor(Image img, Color color, bool isColorShiftFill = true,
            bool isColorShiftStroke = true, bool isColorShiftInvisible = true)
        {
            ChangeSVGColor(img, new SolidColorBrush(color), isColorShiftFill, isColorShiftStroke,
                isColorShiftInvisible);
        }

        private static void SetFillForDrawingGroup(DrawingGroup group, Brush brush, bool isColorShiftFill,
            bool isColorShiftStroke, bool isColorShiftInvisible)
        {
            foreach (var drawing in group.Children)
                ChangeColorForDrawing(drawing, brush, isColorShiftFill, isColorShiftStroke, isColorShiftInvisible);
        }

        private static void ChangeColorForDrawing(Drawing drawing, Brush brush, bool isColorShiftFill,
            bool isColorShiftStroke, bool isColorShiftInvisible)
        {
            if (drawing is DrawingGroup)
            {
                SetFillForDrawingGroup(drawing as DrawingGroup, brush, isColorShiftFill, isColorShiftStroke,
                    isColorShiftInvisible);
            }

            else if (drawing is GeometryDrawing)
            {
                var geo = drawing as GeometryDrawing;
                var brushAsSolid = geo.Brush as SolidColorBrush;

                if (isColorShiftFill && (isColorShiftInvisible || (geo.Brush != null && geo.Brush.Opacity != 0 &&
                                                                   (brushAsSolid == null ||
                                                                    brushAsSolid.Color.A !=
                                                                    0)))) geo.Brush = brush; // For changing FILL

                if (isColorShiftStroke && (isColorShiftInvisible || (geo.Pen != null && geo.Pen?.Brush?.Opacity != 0)))
                {
                    if (geo.Pen == null)
                        geo.Pen = new Pen(brush, 1);
                    else
                        geo.Pen.Brush = brush;
                }
            }
            else if (drawing is GlyphRunDrawing)
            {
                ((GlyphRunDrawing)drawing).ForegroundBrush = brush;
            }
        }

        private static void RestoreInitialImageSource(Image img)
        {
            var imgHash = img.GetHashCode();
            if (InitialImageSources.ContainsKey(imgHash))
            {
                img.Source = InitialImageSources[imgHash];
                InitialImageSources.Remove(imgHash);
            }
        }
    }
}