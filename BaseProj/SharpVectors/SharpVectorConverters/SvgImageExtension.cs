using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    /// <summary>
    ///     This implements a markup extension that enables the creation
    ///     of <see cref="DrawingImage" /> from SVG files.
    /// </summary>
    /// <remarks>
    ///     The SVG source file can be:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>From the web</description>
    ///         </item>
    ///         <item>
    ///             <description>From the local computer (relative or absolute paths)</description>
    ///         </item>
    ///         <item>
    ///             <description>From the resources.</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         The rendering settings are provided as properties for customizations.
    ///     </para>
    /// </remarks>
    [MarkupExtensionReturnType(typeof(DrawingImage))]
    public sealed class SvgImageExtension : MarkupExtension
    {
        #region Private Fields

        private CultureInfo _culture;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Performs the conversion of a valid SVG source file to the
        ///     <see cref="DrawingImage" /> that is set as the value of the target
        ///     property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        ///     Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        ///     This returns <see cref="DrawingImage" /> if successful; otherwise, it
        ///     returns <see langword="null" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var svgSource = GetUri(serviceProvider);

            if (svgSource == null) return null;

            try
            {
                var scheme = svgSource.Scheme;
                if (string.IsNullOrEmpty(scheme)) return null;

                var settings = new WpfDrawingSettings();
                settings.IncludeRuntime = IncludeRuntime;
                settings.TextAsGeometry = TextAsGeometry;
                settings.OptimizePath = OptimizePath;
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
                            var drawGroup = reader.Read(svgSource);

                            if (drawGroup != null) return new DrawingImage(drawGroup);
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
                                            var drawGroup = reader.Read(
                                                zipStream);

                                            if (drawGroup != null) return new DrawingImage(drawGroup);
                                        }
                                    }
                                }
                            else
                                using (svgStreamInfo.Stream)
                                {
                                    using (var reader =
                                           new FileSvgReader(settings))
                                    {
                                        var drawGroup = reader.Read(
                                            svgStreamInfo.Stream);

                                        if (drawGroup != null) return new DrawingImage(drawGroup);
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
                    return null;

                throw;
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Converts the SVG source file to <see cref="Uri" />
        /// </summary>
        /// <param name="serviceProvider">
        ///     Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        ///     Returns the valid <see cref="Uri" /> of the SVG source path if
        ///     successful; otherwise, it returns <see langword="null" />.
        /// </returns>
        private Uri GetUri(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Source)) return null;

            Uri svgSource;
            if (Uri.TryCreate(Source, UriKind.RelativeOrAbsolute, out svgSource))
            {
                if (svgSource.IsAbsoluteUri) return svgSource;

                // Try getting a local file in the same directory....
                var svgPath = Source;
                if (Source[0] == '\\' || Source[0] == '/') svgPath = Source.Substring(1);
                svgPath = svgPath.Replace('/', '\\');

                var assembly = Assembly.GetExecutingAssembly();
                var localFile = Path.Combine(Path.GetDirectoryName(
                    assembly.Location), svgPath);

                if (File.Exists(localFile)) return new Uri(localFile);

                // Try getting it as resource file...
                var uriContext = serviceProvider.GetService(
                    typeof(IUriContext)) as IUriContext;
                if (uriContext != null && uriContext.BaseUri != null) return new Uri(uriContext.BaseUri, svgSource);

                var asmName = assembly.GetName().Name;
                var uriString = string.Format(
                    "pack://application:,,,/{0};component/{1}",
                    asmName, Source);

                return new Uri(uriString);
            }

            return null;
        }

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        ///     Initializes a new instance of the <see cref="SvgImageExtension" /> class.
        /// </overloads>
        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgImageExtension" />
        ///     class with the default parameters.
        /// </summary>
        public SvgImageExtension()
        {
            TextAsGeometry = false;
            IncludeRuntime = true;
            OptimizePath = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgImageExtension" />
        ///     class with the specified SVG file path.
        /// </summary>
        /// <param name="svgSource"></param>
        public SvgImageExtension(string svgPath)
            : this()
        {
            Source = svgPath;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the SVG source file.
        /// </summary>
        /// <value>
        ///     A string specifying the path of the SVG source file.
        ///     The default is <see langword="null" />.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the path geometry is
        ///     optimized using the <see cref="StreamGeometry" />.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the path geometry is optimized
        ///     using the <see cref="StreamGeometry" />; otherwise, it is
        ///     <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        public bool OptimizePath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the texts are rendered as
        ///     path geometry.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if texts are rendered as path
        ///     geometries; otherwise, this is <see langword="false" />. The default
        ///     is <see langword="false" />.
        /// </value>
        public bool TextAsGeometry { get; set; }

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
        public bool IncludeRuntime { get; set; }

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
                if (value != null) _culture = value;
            }
        }

        #endregion
    }
}