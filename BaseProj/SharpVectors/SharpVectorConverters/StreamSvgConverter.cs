using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    /// <summary>
    ///     This converts the SVG file to static or bitmap image, which is
    ///     saved to a file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The image is save with the <see cref="PixelFormats.Pbgra32" /> format,
    ///         since that is the only pixel format which does not throw an exception
    ///         with the <see cref="RenderTargetBitmap" />.
    ///     </para>
    ///     <para>
    ///         The DPI used is 96.
    ///     </para>
    /// </remarks>
    public sealed class StreamSvgConverter : SvgConverter
    {
        #region Private Fields

        /// <summary>
        ///     This is the last drawing generated.
        /// </summary>
        private DrawingGroup _drawing;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly WpfSvgWindow _wpfWindow;

        private readonly WpfDrawingRenderer _wpfRenderer;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        ///     Initializes a new instance of the <see cref="StreamSvgConverter" /> class.
        /// </overloads>
        /// <summary>
        ///     Initializes a new instance of the <see cref="StreamSvgConverter" /> class
        ///     with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        public StreamSvgConverter(WpfDrawingSettings settings)
            : this(false, false, settings)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StreamSvgConverter" /> class
        ///     with the specified drawing or rendering settings and the saving options.
        /// </summary>
        /// <param name="saveXaml">
        ///     This specifies whether to save result object tree in image file.
        /// </param>
        /// <param name="saveZaml">
        ///     This specifies whether to save result object tree in ZAML file. The
        ///     ZAML is simply a G-Zip compressed image format, similar to the SVGZ.
        /// </param>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        public StreamSvgConverter(bool saveXaml, bool saveZaml,
            WpfDrawingSettings settings)
            : base(saveXaml, saveZaml, settings)
        {
            EncoderType = ImageEncoderType.PngBitmap;

            _wpfRenderer = new WpfDrawingRenderer(DrawingSettings);
            _wpfWindow = new WpfSvgWindow(640, 480, _wpfRenderer);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether a writer error occurred when
        ///     using the custom image writer.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if an error occurred when using
        ///     the custom image writer; otherwise, it is <see langword="false" />.
        /// </value>
        public bool WriterErrorOccurred { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to fall back and use
        ///     the .NET Framework image writer when an error occurred in using the
        ///     custom writer.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the converter falls back to using
        ///     the system image writer when an error occurred in using the custom
        ///     writer; otherwise, it is <see langword="false" />. If <see langword="false" />,
        ///     an exception, which occurred in using the custom writer will be
        ///     thrown. The default is <see langword="false" />.
        /// </value>
        public bool FallbackOnWriterError { get; set; }

        /// <summary>
        ///     Gets or set the bitmap encoder type to use in encoding the drawing
        ///     to an image file.
        /// </summary>
        /// <value>
        ///     An enumeration of the type <see cref="ImageEncoderType" /> specifying
        ///     the bitmap encoder. The default is the <see cref="ImageEncoderType.PngBitmap" />.
        /// </value>
        public ImageEncoderType EncoderType { get; set; }

        /// <summary>
        ///     Gets or sets a custom bitmap encoder to use in encoding the drawing
        ///     to an image file.
        /// </summary>
        /// <value>
        ///     A derived <see cref="BitmapEncoder" /> object specifying the bitmap
        ///     encoder for encoding the images. The default is <see langword="null" />,
        ///     and the <see cref="EncoderType" /> property determines the encoder used.
        /// </value>
        /// <remarks>
        ///     If the value of this is set, it must match the MIME type or file
        ///     extension defined by the <see cref="EncoderType" /> property for it
        ///     to be used.
        /// </remarks>
        public BitmapEncoder Encoder { get; set; }

        /// <summary>
        ///     Gets the last created drawing.
        /// </summary>
        /// <value>
        ///     A <see cref="DrawingGroup" /> specifying the last converted drawing.
        /// </value>
        public DrawingGroup Drawing => _drawing;

        #endregion

        #region Public Methods

        /// <overloads>
        ///     This performs the conversion of the specified SVG file, and saves
        ///     the output to an image file.
        /// </overloads>
        /// <summary>
        ///     This performs the conversion of the specified SVG file, and saves
        ///     the output to the specified image file.
        /// </summary>
        /// <param name="svgFileName">
        ///     The full path of the SVG source file.
        /// </param>
        /// <param name="imageStream">
        ///     The output image file. This is optional. If not specified, an image
        ///     file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        ///     This returns <see langword="true" /> if the conversion is successful;
        ///     otherwise, it return <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="svgFileName" /> is empty.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgFileName" /> does not exists.
        /// </exception>
        public bool Convert(string svgFileName, Stream imageStream)
        {
            if (svgFileName == null)
                throw new ArgumentNullException("svgFileName",
                    "The SVG source file cannot be null (or Nothing).");
            if (svgFileName.Length == 0)
                throw new ArgumentException(
                    "The SVG source file cannot be empty.", "svgFileName");
            if (!File.Exists(svgFileName))
                throw new ArgumentException(
                    "The SVG source file must exists.", "svgFileName");

            if (string.IsNullOrEmpty(svgFileName) || !File.Exists(svgFileName)) return false;

            return ProcessFile(svgFileName, imageStream);
        }

        /// <summary>
        ///     This performs the conversion of the specified SVG source, and saves
        ///     the output to the specified image file.
        /// </summary>
        /// <param name="svgStream">
        ///     A stream providing access to the SVG source data.
        /// </param>
        /// <param name="imageStream">
        ///     The output image file. This is optional. If not specified, an image
        ///     file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        ///     This returns <see langword="true" /> if the conversion is successful;
        ///     otherwise, it return <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="imageStream" /> is <see langword="null" />.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgStream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="imageStream" /> is empty.
        /// </exception>
        public bool Convert(Stream svgStream, Stream imageStream)
        {
            if (svgStream == null)
                throw new ArgumentNullException("svgStream",
                    "The SVG source file cannot be null (or Nothing).");
            if (imageStream == null)
                throw new ArgumentNullException("imageStream",
                    "The image destination file path cannot be null (or Nothing).");

            return ProcessFile(svgStream, imageStream);
        }

        /// <summary>
        ///     This performs the conversion of the specified SVG source, and saves
        ///     the output to the specified image file.
        /// </summary>
        /// <param name="svgTextReader">
        ///     A text reader providing access to the SVG source data.
        /// </param>
        /// <param name="imageStream">
        ///     The output image file. This is optional. If not specified, an image
        ///     file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        ///     This returns <see langword="true" /> if the conversion is successful;
        ///     otherwise, it return <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="imageStream" /> is <see langword="null" />.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgTextReader" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="imageStream" /> is empty.
        /// </exception>
        public bool Convert(TextReader svgTextReader, Stream imageStream)
        {
            if (svgTextReader == null)
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");
            if (imageStream == null)
                throw new ArgumentNullException("imageStream",
                    "The image destination file path cannot be null (or Nothing).");

            return ProcessFile(svgTextReader, imageStream);
        }

        /// <summary>
        ///     This performs the conversion of the specified SVG source, and saves
        ///     the output to the specified image file.
        /// </summary>
        /// <param name="svgXmlReader">
        ///     An XML reader providing access to the SVG source data.
        /// </param>
        /// <param name="imageStream">
        ///     The output image file. This is optional. If not specified, an image
        ///     file is created in the same directory as the SVG file.
        /// </param>
        /// <returns>
        ///     This returns <see langword="true" /> if the conversion is successful;
        ///     otherwise, it return <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="imageStream" /> is <see langword="null" />.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgXmlReader" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="imageStream" /> is empty.
        /// </exception>
        public bool Convert(XmlReader svgXmlReader, Stream imageStream)
        {
            if (svgXmlReader == null)
                throw new ArgumentNullException("svgXmlReader",
                    "The SVG source file cannot be null (or Nothing).");
            if (imageStream == null)
                throw new ArgumentNullException("imageStream",
                    "The image destination file path cannot be null (or Nothing).");

            return ProcessFile(svgXmlReader, imageStream);
        }

        #endregion

        #region Private Methods

        #region ProcessFile Method

        private bool ProcessFile(string fileName, Stream imageStream)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(fileName);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return false;

            // Save to the image file...
            SaveImageFile(_drawing, imageStream);

            // Save to image and/or ZAML file if required...
            if (SaveXaml || SaveZaml) SaveXamlFile(_drawing, fileName, null);

            return true;
        }

        private bool ProcessFile(Stream svgStream, Stream imageStream)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgStream);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return false;

            // Save to the image file...
            SaveImageFile(_drawing, imageStream);

            return true;
        }

        private bool ProcessFile(TextReader svgTextReader, Stream imageStream)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgTextReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return false;

            // Save to the image file...
            SaveImageFile(_drawing, imageStream);

            return true;
        }

        private bool ProcessFile(XmlReader svgXmlReader, Stream imageStream)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgXmlReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return false;

            // Save to the image file...
            SaveImageFile(_drawing, imageStream);

            return true;
        }

        #endregion

        #region SaveImageFile Method

        private bool SaveImageFile(Drawing drawing, Stream imageStream)
        {
            var bitmapEncoder = GetBitmapEncoder(
                GetImageFileExtention());

            // The image parameters...
            //Rect drawingBounds = drawing.Bounds;
            //int pixelWidth  = (int)drawingBounds.Width;
            //int pixelHeight = (int)drawingBounds.Height;
            double dpiX = 96;
            double dpiY = 96;

            // The Visual to use as the source of the RenderTargetBitmap.
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            if (Background != null) drawingContext.DrawRectangle(Background, null, drawing.Bounds);
            drawingContext.DrawDrawing(drawing);
            drawingContext.Close();

            /// get bound of the visual
            var drawingBounds = VisualTreeHelper.GetDescendantBounds(drawingVisual);
            var pixelWidth = (int)drawingBounds.Width;
            var pixelHeight = (int)drawingBounds.Height;

            // The BitmapSource that is rendered with a Visual.
            var targetBitmap = new RenderTargetBitmap(
                pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            targetBitmap.Render(drawingVisual);

            // Encoding the RenderBitmapTarget as an image file.
            bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            bitmapEncoder.Save(imageStream);

            return true;
        }

        private BitmapEncoder GetBitmapEncoder(string fileExtension)
        {
            BitmapEncoder bitmapEncoder = null;

            if (Encoder != null && Encoder.CodecInfo != null)
            {
                var mimeType = string.Empty;
                var codecInfo = Encoder.CodecInfo;
                var mimeTypes = codecInfo.MimeTypes;
                var fileExtensions = codecInfo.FileExtensions;
                switch (EncoderType)
                {
                    case ImageEncoderType.BmpBitmap:
                        mimeType = "image/bmp";
                        break;
                    case ImageEncoderType.GifBitmap:
                        mimeType = "image/gif";
                        break;
                    case ImageEncoderType.JpegBitmap:
                        mimeType = "image/jpeg,image/jpe,image/jpg";
                        break;
                    case ImageEncoderType.PngBitmap:
                        mimeType = "image/png";
                        break;
                    case ImageEncoderType.TiffBitmap:
                        mimeType = "image/tiff,image/tif";
                        break;
                    case ImageEncoderType.WmpBitmap:
                        mimeType = "image/vnd.ms-photo";
                        break;
                }

                if (!string.IsNullOrEmpty(fileExtensions) &&
                    fileExtensions.IndexOf(fileExtension,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bitmapEncoder = Encoder;
                }
                else if (!string.IsNullOrEmpty(mimeTypes) &&
                         !string.IsNullOrEmpty(mimeType))
                {
                    var arrayMimeTypes = mimeType.Split(',');
                    for (var i = 0; i < arrayMimeTypes.Length; i++)
                        if (mimeTypes.IndexOf(arrayMimeTypes[i],
                                StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            bitmapEncoder = Encoder;
                            break;
                        }
                }
            }

            if (bitmapEncoder == null)
                switch (EncoderType)
                {
                    case ImageEncoderType.BmpBitmap:
                        bitmapEncoder = new BmpBitmapEncoder();
                        break;
                    case ImageEncoderType.GifBitmap:
                        bitmapEncoder = new GifBitmapEncoder();
                        break;
                    case ImageEncoderType.JpegBitmap:
                        var jpgEncoder = new JpegBitmapEncoder();
                        // Set the default/user options...
                        bitmapEncoder = jpgEncoder;
                        break;
                    case ImageEncoderType.PngBitmap:
                        var pngEncoder = new PngBitmapEncoder();
                        // Set the default/user options...
                        bitmapEncoder = pngEncoder;
                        break;
                    case ImageEncoderType.TiffBitmap:
                        bitmapEncoder = new TiffBitmapEncoder();
                        break;
                    case ImageEncoderType.WmpBitmap:
                        var wmpEncoder = new WmpBitmapEncoder();
                        // Set the default/user options...
                        bitmapEncoder = wmpEncoder;
                        break;
                }

            if (bitmapEncoder == null) bitmapEncoder = new PngBitmapEncoder();

            return bitmapEncoder;
        }

        private string GetImageFileExtention()
        {
            switch (EncoderType)
            {
                case ImageEncoderType.BmpBitmap:
                    return ".bmp";
                case ImageEncoderType.GifBitmap:
                    return ".gif";
                case ImageEncoderType.JpegBitmap:
                    return ".jpg";
                case ImageEncoderType.PngBitmap:
                    return ".png";
                case ImageEncoderType.TiffBitmap:
                    return ".tif";
                case ImageEncoderType.WmpBitmap:
                    return ".wdp";
            }

            return ".png";
        }

        #endregion

        #region SaveXamlFile Method

        private bool SaveXamlFile(Drawing drawing, string fileName, string imageFileName)
        {
            WriterErrorOccurred = false;

            string xamlFileName = null;
            if (string.IsNullOrEmpty(imageFileName))
            {
                var fileNameWithoutExt =
                    Path.GetFileNameWithoutExtension(fileName);

                var workingDir = Path.GetDirectoryName(fileName);
                xamlFileName = Path.Combine(workingDir,
                    fileNameWithoutExt + ".xaml");
            }
            else
            {
                var fileExt = Path.GetExtension(imageFileName);
                if (string.IsNullOrEmpty(fileExt))
                    xamlFileName = imageFileName + ".xaml";
                else if (!string.Equals(fileExt, ".xaml",
                             StringComparison.OrdinalIgnoreCase))
                    xamlFileName = Path.ChangeExtension(imageFileName, ".xaml");
            }

            if (File.Exists(xamlFileName))
            {
                File.SetAttributes(xamlFileName, FileAttributes.Normal);
                File.Delete(xamlFileName);
            }

            if (UseFrameXamlWriter)
            {
                var writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (var xamlFile = File.Create(xamlFileName))
                {
                    using (var writer = XmlWriter.Create(
                               xamlFile, writerSettings))
                    {
                        XamlWriter.Save(
                            drawing, writer);
                    }
                }
            }
            else
            {
                try
                {
                    var xamlWriter = new XmlXamlWriter(
                        DrawingSettings);

                    using (var xamlFile = File.Create(xamlFileName))
                    {
                        xamlWriter.Save(drawing, xamlFile);
                    }
                }
                catch
                {
                    WriterErrorOccurred = true;

                    if (FallbackOnWriterError)
                    {
                        if (File.Exists(xamlFileName)) File.Move(xamlFileName, xamlFileName + ".bak");

                        var writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (var xamlFile = File.Create(xamlFileName))
                        {
                            using (var writer = XmlWriter.Create(
                                       xamlFile, writerSettings))
                            {
                                XamlWriter.Save(
                                    drawing, writer);
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (SaveZaml)
            {
                var zamlFileName = Path.ChangeExtension(xamlFileName, ".zaml");

                if (File.Exists(zamlFileName))
                {
                    File.SetAttributes(zamlFileName, FileAttributes.Normal);
                    File.Delete(zamlFileName);
                }

                var zamlSourceFile = new FileStream(xamlFileName, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                var buffer = new byte[zamlSourceFile.Length];
                // Read the file to ensure it is readable.
                var count = zamlSourceFile.Read(buffer, 0, buffer.Length);
                if (count != buffer.Length)
                {
                    zamlSourceFile.Close();
                    return false;
                }

                zamlSourceFile.Close();

                var zamlDestFile = File.Create(zamlFileName);

                var zipStream = new GZipStream(zamlDestFile, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);

                zipStream.Close();

                zamlDestFile.Close();
            }

            if (!SaveXaml && File.Exists(xamlFileName)) File.Delete(xamlFileName);

            return true;
        }

        #endregion

        #endregion
    }
}