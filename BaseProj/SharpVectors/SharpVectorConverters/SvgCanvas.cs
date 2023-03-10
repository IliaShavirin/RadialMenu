using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    /// <summary>
    ///     This is a <see cref="Canvas" /> control for viewing SVG file in WPF
    ///     applications.
    /// </summary>
    /// <remarks>
    ///     It extends the drawing canvas, <see cref="SvgDrawingCanvas" />, instead of
    ///     generic <see cref="Canvas" /> control, therefore any interactivity support
    ///     implemented in the drawing canvas will be available in the
    ///     <see cref="Canvas" />.
    /// </remarks>
    public class SvgCanvas : SvgDrawingCanvas, IUriContext
    {
        #region Constructors and Destructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgCanvas" /> class.
        /// </summary>
        public SvgCanvas()
        {
            _textAsGeometry = false;
            _includeRuntime = true;
            _optimizePath = true;
        }

        #endregion

        #region IUriContext Members

        /// <summary>
        ///     Gets or sets the base URI of the current application context.
        /// </summary>
        /// <value>
        ///     The base URI of the application context.
        /// </value>
        public Uri BaseUri { get; set; }

        #endregion

        #region Private Fields

        private bool _isAutoSized;
        private bool _autoSize;
        private bool _textAsGeometry;
        private bool _includeRuntime;
        private bool _optimizePath;

        private DrawingGroup _svgDrawing;

        private CultureInfo _culture;

        private Uri _sourceUri;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the path to the SVG file to load into this
        ///     <see cref="Canvas" />.
        /// </summary>
        /// <value>
        ///     A <see cref="System.Uri" /> specifying the path to the SVG source file.
        ///     The file can be located on a computer, network or assembly resources.
        ///     Settings this to <see langword="null" /> will close any opened diagram.
        /// </value>
        public Uri Source
        {
            get => _sourceUri;
            set
            {
                _sourceUri = value;

                if (_sourceUri == null)
                    OnUnloadDiagram();
                else
                    OnSettingsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically resize this
        ///     <see cref="Canvas" /> based on the size of the loaded drawing.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if this <see cref="Canvas" /> is
        ///     automatically resized based on the size of the loaded drawing;
        ///     otherwise, it is <see langword="false" />. The default is
        ///     <see langword="false" />, and the user-defined size or the parent assigned
        ///     layout size is used.
        /// </value>
        public bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;

                OnAutoSizeChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the path geometry is
        ///     optimized using the <see cref="StreamGeometry" />.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the path geometry is optimized
        ///     using the <see cref="StreamGeometry" />; otherwise, it is
        ///     <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        public bool OptimizePath
        {
            get => _optimizePath;
            set
            {
                _optimizePath = value;

                OnSettingsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the texts are rendered as
        ///     path geometry.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if texts are rendered as path
        ///     geometries; otherwise, this is <see langword="false" />. The default
        ///     is <see langword="false" />.
        /// </value>
        public bool TextAsGeometry
        {
            get => _textAsGeometry;
            set
            {
                _textAsGeometry = value;

                OnSettingsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the <c>SharpVectors.Runtime.dll</c>
        ///     classes are used in the generated output.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the <c>SharpVectors.Runtime.dll</c>
        ///     classes and types are used in the generated output; otherwise, it is
        ///     <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        /// <remarks>
        ///     The use of the <c>SharpVectors.Runtime.dll</c> prevents the hard-coded
        ///     font path generated by the <see cref="FormattedText" /> class, support
        ///     for embedded images etc.
        /// </remarks>
        public bool IncludeRuntime
        {
            get => _includeRuntime;
            set
            {
                _includeRuntime = value;

                OnSettingsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the main culture information used for rendering texts.
        /// </summary>
        /// <value>
        ///     An instance of the <see cref="CultureInfo" /> specifying the main
        ///     culture information for texts. The default is the English culture.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         This is the culture information passed to the <see cref="FormattedText" />
        ///         class instance for the text rendering.
        ///     </para>
        ///     <para>
        ///         The library does not currently provide any means of splitting texts
        ///         into its multi-language parts.
        ///     </para>
        /// </remarks>
        public CultureInfo CultureInfo
        {
            get => _culture;
            set
            {
                if (value != null)
                {
                    _culture = value;

                    OnSettingsChanged();
                }
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the Initialized event. This method is invoked whenever IsInitialized is set to true.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_sourceUri != null)
                if (_svgDrawing == null)
                {
                    var drawing = CreateDrawing();
                    if (drawing != null) OnLoadDrawing(drawing);
                }
        }

        /// <summary>
        ///     This handles changes in the rendering settings of this control.
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
            if (!IsInitialized || _sourceUri == null) return;

            var drawing = CreateDrawing();
            if (drawing != null) OnLoadDrawing(drawing);
        }

        /// <summary>
        ///     This handles changes in the automatic resizing property of this control.
        /// </summary>
        protected virtual void OnAutoSizeChanged()
        {
            if (_autoSize)
            {
                if (IsInitialized && _svgDrawing != null)
                {
                    var rectDrawing = _svgDrawing.Bounds;
                    if (!rectDrawing.IsEmpty)
                    {
                        Width = rectDrawing.Width;
                        Height = rectDrawing.Height;

                        _isAutoSized = true;
                    }
                }
            }
            else
            {
                if (_isAutoSized)
                {
                    Width = double.NaN;
                    Height = double.NaN;
                }
            }
        }

        /// <summary>
        ///     Performs the conversion of a valid SVG source file to the
        ///     <see cref="DrawingGroup" />.
        /// </summary>
        /// <returns>
        ///     This returns <see cref="DrawingGroup" /> if successful; otherwise, it
        ///     returns <see langword="null" />.
        /// </returns>
        protected virtual DrawingGroup CreateDrawing()
        {
            var svgSource = GetAbsoluteUri();

            DrawingGroup drawing = null;

            if (svgSource == null) return drawing;

            try
            {
                var scheme = svgSource.Scheme;
                if (string.IsNullOrEmpty(scheme)) return null;

                var settings = new WpfDrawingSettings();
                settings.IncludeRuntime = _includeRuntime;
                settings.TextAsGeometry = _textAsGeometry;
                settings.OptimizePath = _optimizePath;
                if (_culture != null) settings.CultureInfo = _culture;

                switch (scheme)
                {
                    case "file":
                    //case "ftp":
                    //case "https":
                    case "http":
                        using (var reader =
                               new FileSvgReader(settings))
                        {
                            drawing = reader.Read(svgSource);
                        }

                        break;
                    case "pack":
                        StreamResourceInfo svgStreamInfo = null;
                        if (svgSource.ToString().IndexOf("siteoforigin",
                                StringComparison.OrdinalIgnoreCase) >= 0)
                            svgStreamInfo = Application.GetRemoteStream(svgSource);
                        else
                            svgStreamInfo = Application.GetResourceStream(svgSource);

                        var svgStream = svgStreamInfo != null ? svgStreamInfo.Stream : null;

                        if (svgStream != null)
                        {
                            var fileExt = Path.GetExtension(svgSource.ToString());
                            var isCompressed = !string.IsNullOrEmpty(fileExt) &&
                                               string.Equals(fileExt, ".svgz",
                                                   StringComparison.OrdinalIgnoreCase);

                            if (isCompressed)
                                using (svgStream)
                                {
                                    using (var zipStream =
                                           new GZipStream(svgStream, CompressionMode.Decompress))
                                    {
                                        using (var reader =
                                               new FileSvgReader(settings))
                                        {
                                            drawing = reader.Read(zipStream);
                                        }
                                    }
                                }
                            else
                                using (svgStream)
                                {
                                    using (var reader =
                                           new FileSvgReader(settings))
                                    {
                                        drawing = reader.Read(svgStream);
                                    }
                                }
                        }

                        break;
                }
            }
            catch
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return drawing;

                throw;
            }

            return drawing;
        }

        #endregion

        #region Private Methods

        private void OnLoadDrawing(DrawingGroup drawing)
        {
            if (drawing == null) return;

            OnUnloadDiagram();

            RenderDiagrams(drawing);

            _svgDrawing = drawing;

            OnAutoSizeChanged();
        }

        private void OnUnloadDiagram()
        {
            UnloadDiagrams();

            if (_isAutoSized)
            {
                Width = double.NaN;
                Height = double.NaN;
            }
        }

        private Uri GetAbsoluteUri()
        {
            if (_sourceUri == null) return null;
            var svgSource = _sourceUri;

            if (svgSource.IsAbsoluteUri) return svgSource;

            // Try getting a local file in the same directory....
            var svgPath = svgSource.ToString();
            if (svgPath[0] == '\\' || svgPath[0] == '/') svgPath = svgPath.Substring(1);
            svgPath = svgPath.Replace('/', '\\');

            var assembly = Assembly.GetExecutingAssembly();
            var localFile = Path.Combine(Path.GetDirectoryName(
                assembly.Location), svgPath);

            if (File.Exists(localFile)) return new Uri(localFile);

            // Try getting it as resource file...
            if (BaseUri != null) return new Uri(BaseUri, svgSource);

            var asmName = assembly.GetName().Name;
            var uriString = string.Format(
                "pack://application:,,,/{0};component/{1}",
                asmName, svgPath);

            return new Uri(uriString);
        }

        #endregion
    }
}