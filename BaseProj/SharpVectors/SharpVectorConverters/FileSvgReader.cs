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
    ///     This converts a SVG file to <see cref="DrawingGroup" /> object, and can
    ///     optionally save the result to a file as XAML.
    /// </summary>
    public sealed class FileSvgReader : SvgConverter
    {
        #region IDisposable Members

        /// <summary>
        ///     This releases the unmanaged resources used by the <see cref="SvgConverter" />
        ///     and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     This is <see langword="true" /> if managed resources should be
        ///     disposed; otherwise, <see langword="false" />.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            _drawing = null;
            _wpfWindow = null;
            _wpfRenderer = null;

            base.Dispose(disposing);
        }

        #endregion

        #region Private Fields

        private DirectoryInfo _workingDir;

        /// <summary>
        ///     This is the last drawing generated.
        /// </summary>
        private DrawingGroup _drawing;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private WpfSvgWindow _wpfWindow;

        private WpfDrawingRenderer _wpfRenderer;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        ///     Initializes a new instance of the <see cref="FileSvgReader" /> class.
        /// </overloads>
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSvgReader" /> class
        ///     with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        public FileSvgReader(WpfDrawingSettings settings)
            : this(false, false, null, settings)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSvgConverter" /> class
        ///     with the specified drawing or rendering settings, the saving options
        ///     and the working directory.
        /// </summary>
        /// <param name="saveXaml">
        ///     This specifies whether to save result object tree in XAML file.
        /// </param>
        /// <param name="saveZaml">
        ///     This specifies whether to save result object tree in ZAML file. The
        ///     ZAML is simply a G-Zip compressed XAML format, similar to the SVGZ.
        /// </param>
        /// <param name="workingDir">
        ///     The working directory, where converted outputs are saved.
        /// </param>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        public FileSvgReader(bool saveXaml, bool saveZaml,
            DirectoryInfo workingDir, WpfDrawingSettings settings)
            : base(saveXaml, saveZaml, settings)
        {
            _wpfRenderer = new WpfDrawingRenderer(DrawingSettings);
            _wpfWindow = new WpfSvgWindow(640, 480, _wpfRenderer);

            _workingDir = workingDir;

            if (_workingDir != null)
                if (!_workingDir.Exists)
                    _workingDir.Create();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether a writer error occurred when
        ///     using the custom XAML writer.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if an error occurred when using
        ///     the custom XAML writer; otherwise, it is <see langword="false" />.
        /// </value>
        public bool WriterErrorOccurred { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to fall back and use
        ///     the .NET Framework XAML writer when an error occurred in using the
        ///     custom writer.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the converter falls back to using
        ///     the system XAML writer when an error occurred in using the custom
        ///     writer; otherwise, it is <see langword="false" />. If <see langword="false" />,
        ///     an exception, which occurred in using the custom writer will be
        ///     thrown. The default is <see langword="false" />.
        /// </value>
        public bool FallbackOnWriterError { get; set; }

        /// <summary>
        ///     Gets the last created drawing.
        /// </summary>
        /// <value>
        ///     A <see cref="DrawingGroup" /> specifying the last converted drawing.
        /// </value>
        public DrawingGroup Drawing => _drawing;

        /// <summary>
        ///     Gets the output image file path if generated.
        /// </summary>
        /// <value>
        ///     A string containing the full path to the image if generated; otherwise,
        ///     it is <see langword="null" />.
        /// </value>
        public string ImageFile { get; private set; }

        /// <summary>
        ///     Gets the output XAML file path if generated.
        /// </summary>
        /// <value>
        ///     A string containing the full path to the XAML if generated; otherwise,
        ///     it is <see langword="null" />.
        /// </value>
        public string XamlFile { get; private set; }

        /// <summary>
        ///     Gets the output ZAML file path if generated.
        /// </summary>
        /// <value>
        ///     A string containing the full path to the ZAML if generated; otherwise,
        ///     it is <see langword="null" />.
        /// </value>
        public string ZamlFile { get; private set; }

        #endregion

        #region Public Methods

        /// <overloads>
        ///     Reads in the specified SVG file and converts it to WPF drawing.
        /// </overloads>
        /// <summary>
        ///     Reads in the specified SVG file and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgFileName">
        ///     The full path of the SVG source file.
        /// </param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="svgFileName" /> is empty.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgFileName" /> does not exists.
        /// </exception>
        public DrawingGroup Read(string svgFileName)
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

            if (_workingDir == null)
            {
                svgFileName = Path.GetFullPath(svgFileName);
                _workingDir = new DirectoryInfo(
                    Path.GetDirectoryName(svgFileName));
            }

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return LoadFile(svgFileName);
        }

        /// <summary>
        ///     Reads in the specified SVG file and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgUri">
        ///     A <see cref="System.Uri" /> specifying the path to the SVG file.
        /// </param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgUri" /> is <see langword="null" />.
        /// </exception>
        public DrawingGroup Read(Uri svgUri)
        {
            if (svgUri == null)
                throw new ArgumentNullException("svgUri",
                    "The SVG source file cannot be null (or Nothing).");

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return LoadFile(svgUri);
        }

        /// <summary>
        ///     Reads in the specified SVG file stream and converts it to WPF drawing.
        /// </summary>
        /// <param name="svgStream">The source SVG file stream.</param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgStream" /> is <see langword="null" />.
        /// </exception>
        public DrawingGroup Read(Stream svgStream)
        {
            if (svgStream == null)
                throw new ArgumentNullException("svgStream",
                    "The SVG source file cannot be null (or Nothing).");

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return LoadFile(svgStream);
        }

        /// <summary>
        ///     Reads in the specified source from the SVG file reader and converts
        ///     it to WPF drawing.
        /// </summary>
        /// <param name="svgTextReader">
        ///     A text reader providing access to the SVG file data.
        /// </param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgTextReader" /> is <see langword="null" />.
        /// </exception>
        public DrawingGroup Read(TextReader svgTextReader)
        {
            if (svgTextReader == null)
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return LoadFile(svgTextReader);
        }

        /// <summary>
        ///     Reads in the specified source SVG file reader and converts it to
        ///     WPF drawing.
        /// </summary>
        /// <param name="svgXmlReader">
        ///     An XML reader providing access to the SVG file data.
        /// </param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgXmlReader" /> is <see langword="null" />.
        /// </exception>
        public DrawingGroup Read(XmlReader svgXmlReader)
        {
            if (svgXmlReader == null)
                throw new ArgumentNullException("svgTextReader",
                    "The SVG source file cannot be null (or Nothing).");

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return LoadFile(svgXmlReader);
        }

        /// <summary>
        ///     Reads in the specified SVG file, converting it to WPF drawing and
        ///     saving the results to the specified directory if successful.
        /// </summary>
        /// <param name="svgFileName">
        ///     The full path of the SVG source file.
        /// </param>
        /// <param name="destinationDir">
        ///     The destination of the output XAML file, if the saving properties
        ///     are enabled.
        /// </param>
        /// <returns>
        ///     This returns the <see cref="DrawingGroup" /> representing the SVG file,
        ///     if successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If the <paramref name="svgFileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the <paramref name="svgFileName" /> is empty.
        ///     <para>-or-</para>
        ///     If the <paramref name="svgFileName" /> does not exists.
        /// </exception>
        public DrawingGroup Read(string svgFileName, DirectoryInfo destinationDir)
        {
            _workingDir = destinationDir;

            if (_workingDir != null)
                if (!_workingDir.Exists)
                    _workingDir.Create();

            ImageFile = null;
            XamlFile = null;
            ZamlFile = null;

            return Read(svgFileName);
        }

        /// <summary>
        ///     Saves the last converted file to the specified file name.
        /// </summary>
        /// <param name="fileName">
        ///     The full path of the output file.
        /// </param>
        /// <param name="asXaml">
        ///     A value indicating whether to save the output to XAML file.
        /// </param>
        /// <param name="asZaml">
        ///     A value indicating whether to save the output to ZAML file, which
        ///     is a G-zip compression of the XAML file.
        /// </param>
        /// <returns>
        ///     This returns <see langword="true" /> if either <paramref name="asXaml" />
        ///     or <paramref name="asZaml" /> is <see langword="true" /> and the operation
        ///     is successful.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the output serialization properties are not enabled, this method
        ///         can be used to save the output to a file.
        ///     </para>
        ///     <para>
        ///         This will not change the output serialization properties of this object.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     If there is no converted drawing from a previous conversion process
        ///     to be saved.
        /// </exception>
        public bool Save(string fileName, bool asXaml, bool asZaml)
        {
            if (_drawing == null)
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");

            // We save the current states and properties...
            var saveXaml = SaveXaml;
            var saveZaml = SaveZaml;

            SaveXaml = asXaml;
            SaveZaml = asZaml;

            var workingDir = _workingDir;

            fileName = Path.GetFullPath(fileName);
            _workingDir = new DirectoryInfo(Path.GetDirectoryName(fileName));

            var savedResult = SaveFile(fileName);

            // Restore the current states and properties...
            SaveXaml = saveXaml;
            SaveZaml = saveZaml;
            _workingDir = workingDir;

            return savedResult;
        }

        public bool Save(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter",
                    "The text writer parameter is required and cannot be null (or Nothing).");
            if (_drawing == null)
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");

            return SaveFile(textWriter);
        }

        public bool Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream",
                    "The stream parameter is required and cannot be null (or Nothing).");
            if (_drawing == null)
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");

            return SaveFile(stream);
        }

        public bool SaveImage(string fileName, DirectoryInfo imageFileDir,
            ImageEncoderType encoderType)
        {
            if (imageFileDir == null) return SaveImageFile(fileName, string.Empty, encoderType);

            if (!imageFileDir.Exists) imageFileDir.Create();

            var outputExt = GetImageFileExtention(encoderType);

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(
                fileName);

            var imageFileName = Path.Combine(imageFileDir.FullName,
                fileNameWithoutExt + outputExt);

            return SaveImageFile(fileName, imageFileName, encoderType);
        }

        public bool SaveImage(string fileName, FileInfo imageFileName,
            ImageEncoderType encoderType)
        {
            return SaveImageFile(fileName,
                imageFileName == null ? string.Empty : imageFileName.FullName,
                encoderType);
        }

        #endregion

        #region Load Method

        private DrawingGroup LoadFile(string fileName)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(fileName);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return null;

            SaveFile(fileName);

            return _drawing;
        }

        private DrawingGroup LoadFile(Stream stream)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(stream);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return null;

            return _drawing;
        }

        private DrawingGroup LoadFile(Uri svgUri)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(svgUri);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return null;

            return _drawing;
        }

        private DrawingGroup LoadFile(TextReader textReader)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(textReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return null;

            return _drawing;
        }

        private DrawingGroup LoadFile(XmlReader xmlReader)
        {
            _wpfRenderer.LinkVisitor = new LinkVisitor();
            _wpfRenderer.ImageVisitor = new EmbeddedImageVisitor();
            _wpfRenderer.FontFamilyVisitor = new FontFamilyVisitor();

            _wpfWindow.LoadDocument(xmlReader);

            _wpfRenderer.InvalidRect = SvgRectF.Empty;

            _wpfRenderer.Render(_wpfWindow.Document as SvgDocument);

            _drawing = _wpfRenderer.Drawing as DrawingGroup;
            if (_drawing == null) return null;

            return _drawing;
        }

        #endregion

        #region SaveFile Method

        private bool SaveFile(Stream stream)
        {
            WriterErrorOccurred = false;

            if (UseFrameXamlWriter)
            {
                var writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (var writer = XmlWriter.Create(
                           stream, writerSettings))
                {
                    XamlWriter.Save(
                        _drawing, writer);
                }
            }
            else
            {
                try
                {
                    var xamlWriter = new XmlXamlWriter(
                        DrawingSettings);

                    xamlWriter.Save(_drawing, stream);
                }
                catch
                {
                    WriterErrorOccurred = true;

                    if (FallbackOnWriterError)
                    {
                        var writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (var writer = XmlWriter.Create(
                                   stream, writerSettings))
                        {
                            XamlWriter.Save(
                                _drawing, writer);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        private bool SaveFile(TextWriter textWriter)
        {
            WriterErrorOccurred = false;

            if (UseFrameXamlWriter)
            {
                var writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = true;
                writerSettings.Encoding = Encoding.UTF8;
                using (var writer = XmlWriter.Create(
                           textWriter, writerSettings))
                {
                    XamlWriter.Save(
                        _drawing, writer);
                }
            }
            else
            {
                try
                {
                    var xamlWriter = new XmlXamlWriter(
                        DrawingSettings);

                    xamlWriter.Save(_drawing, textWriter);
                }
                catch
                {
                    WriterErrorOccurred = true;

                    if (FallbackOnWriterError)
                    {
                        var writerSettings = new XmlWriterSettings();
                        writerSettings.Indent = true;
                        writerSettings.OmitXmlDeclaration = true;
                        writerSettings.Encoding = Encoding.UTF8;
                        using (var writer = XmlWriter.Create(
                                   textWriter, writerSettings))
                        {
                            XamlWriter.Save(
                                _drawing, writer);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        private bool SaveFile(string fileName)
        {
            if (_workingDir == null || (!SaveXaml && !SaveZaml)) return false;

            WriterErrorOccurred = false;

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            var xamlFileName = Path.Combine(_workingDir.FullName,
                fileNameWithoutExt + ".xaml");

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
                            _drawing, writer);
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
                        xamlWriter.Save(_drawing, xamlFile);
                    }
                }
                catch
                {
                    WriterErrorOccurred = true;

                    if (FallbackOnWriterError)
                    {
                        // If the file exist, we back it up and save a new file...
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
                                    _drawing, writer);
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

                var zamlSourceFile = new FileStream(
                    xamlFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

                var zipStream = new GZipStream(zamlDestFile,
                    CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);

                zipStream.Close();

                zamlDestFile.Close();

                ZamlFile = zamlFileName;
            }

            XamlFile = xamlFileName;

            if (!SaveXaml && File.Exists(xamlFileName))
            {
                File.Delete(xamlFileName);
                XamlFile = null;
            }

            return true;
        }

        #endregion

        #region SaveImageFile Method

        private bool SaveImageFile(string fileName, string imageFileName,
            ImageEncoderType encoderType)
        {
            if (_drawing == null)
                throw new InvalidOperationException(
                    "There is no converted drawing for the saving operation.");

            var outputExt = GetImageFileExtention(encoderType);
            string outputFileName = null;
            if (string.IsNullOrEmpty(imageFileName))
            {
                var fileNameWithoutExt =
                    Path.GetFileNameWithoutExtension(fileName);

                var workingDir = Path.GetDirectoryName(fileName);
                outputFileName = Path.Combine(workingDir,
                    fileNameWithoutExt + outputExt);
            }
            else
            {
                var fileExt = Path.GetExtension(imageFileName);
                if (string.IsNullOrEmpty(fileExt))
                    outputFileName = imageFileName + outputExt;
                else if (!string.Equals(fileExt, outputExt,
                             StringComparison.OrdinalIgnoreCase))
                    outputFileName = Path.ChangeExtension(imageFileName, outputExt);
                else
                    outputFileName = imageFileName;
            }

            var outputFileDir = Path.GetDirectoryName(outputFileName);
            if (!Directory.Exists(outputFileDir)) Directory.CreateDirectory(outputFileDir);

            var bitampEncoder = GetBitmapEncoder(outputExt,
                encoderType);

            // The image parameters...
            var drawingBounds = _drawing.Bounds;
            var pixelWidth = (int)drawingBounds.Width;
            var pixelHeight = (int)drawingBounds.Height;
            double dpiX = 96;
            double dpiY = 96;

            // The Visual to use as the source of the RenderTargetBitmap.
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            if (Background != null) drawingContext.DrawRectangle(Background, null, _drawing.Bounds);
            drawingContext.DrawDrawing(_drawing);
            drawingContext.Close();

            // The BitmapSource that is rendered with a Visual.
            var targetBitmap = new RenderTargetBitmap(
                pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            targetBitmap.Render(drawingVisual);

            // Encoding the RenderBitmapTarget as an image file.
            bitampEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            using (var stream = File.Create(outputFileName))
            {
                bitampEncoder.Save(stream);
            }

            ImageFile = outputFileName;

            return true;
        }

        private static BitmapEncoder GetBitmapEncoder(string fileExtension,
            ImageEncoderType encoderType)
        {
            BitmapEncoder bitampEncoder = null;


            switch (encoderType)
            {
                case ImageEncoderType.BmpBitmap:
                    bitampEncoder = new BmpBitmapEncoder();
                    break;
                case ImageEncoderType.GifBitmap:
                    bitampEncoder = new GifBitmapEncoder();
                    break;
                case ImageEncoderType.JpegBitmap:
                    var jpgEncoder = new JpegBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = jpgEncoder;
                    break;
                case ImageEncoderType.PngBitmap:
                    var pngEncoder = new PngBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = pngEncoder;
                    break;
                case ImageEncoderType.TiffBitmap:
                    bitampEncoder = new TiffBitmapEncoder();
                    break;
                case ImageEncoderType.WmpBitmap:
                    var wmpEncoder = new WmpBitmapEncoder();
                    // Set the default/user options...
                    bitampEncoder = wmpEncoder;
                    break;
            }

            if (bitampEncoder == null) bitampEncoder = new PngBitmapEncoder();

            return bitampEncoder;
        }

        private static string GetImageFileExtention(ImageEncoderType encoderType)
        {
            switch (encoderType)
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
    }
}