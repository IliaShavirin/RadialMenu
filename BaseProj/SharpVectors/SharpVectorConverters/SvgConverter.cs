using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    /// <summary>
    ///     This is the <see langword="abstract" /> base class for all
    ///     SVG to WPF converters.
    /// </summary>
    public abstract class SvgConverter : DependencyObject, IDisposable
    {
        #region Private Fields

        #endregion

        #region Constructors and Destrutor

        /// <overloads>
        ///     Initializes a new instance of the <see cref="SvgConverter" /> class.
        /// </overloads>
        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgConverter" /> class
        ///     with the default parameters and settings.
        /// </summary>
        protected SvgConverter()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgConverter" /> class
        ///     with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        protected SvgConverter(WpfDrawingSettings settings)
        {
            SaveXaml = true;
            SaveZaml = false;
            DrawingSettings = settings;

            if (DrawingSettings == null) DrawingSettings = new WpfDrawingSettings();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SvgConverter" /> class
        ///     with the specified drawing or rendering settings and the saving options.
        /// </summary>
        /// <param name="saveXaml">
        ///     This specifies whether to save result object tree in XAML file.
        /// </param>
        /// <param name="saveZaml">
        ///     This specifies whether to save result object tree in ZAML file. The
        ///     ZAML is simply a G-Zip compressed XAML format, similar to the SVGZ.
        /// </param>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        protected SvgConverter(bool saveXaml, bool saveZaml,
            WpfDrawingSettings settings) : this(settings)
        {
            SaveXaml = saveXaml;
            SaveZaml = SaveZaml;
        }

        /// <summary>
        ///     This allows a converter to attempt to free resources and perform
        ///     other cleanup operations before the converter is reclaimed by
        ///     garbage collection.
        /// </summary>
        ~SvgConverter()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether to save the conversion
        ///     output to the XAML file.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the conversion output is saved
        ///     to the XAML file; otherwise, it is <see langword="false" />.
        ///     The default depends on the converter.
        /// </value>
        public bool SaveXaml { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to save the conversion
        ///     output to the ZAML file.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the conversion output is saved
        ///     to the ZAML file; otherwise, it is <see langword="false" />.
        ///     The default depends on the converter.
        /// </value>
        /// <remarks>
        ///     The ZAML is simply a G-Zip compressed XAML format, similar to the
        ///     SVGZ.
        /// </remarks>
        public bool SaveZaml { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use the .NET framework
        ///     version of the XAML writer.
        /// </summary>
        /// <value>
        ///     This is <see langword="true" /> if the .NET framework version of the
        ///     XAML writer is used; otherwise, a customized XAML writer,
        ///     <see cref="XmlXamlWriter" />, is used. The default is <see langword="false" />.
        /// </value>
        /// <remarks>
        ///     The customized XAML writer is optimized for the conversion process,
        ///     and it is recommended as the writer, unless in cases where it fails
        ///     to produce accurate result.
        /// </remarks>
        public bool UseFrameXamlWriter { get; set; }

        /// <summary>
        ///     Gets or sets a brush that describes the background of a image.
        /// </summary>
        /// <value>
        ///     The brush that is used to fill the background of the control.
        ///     The default is <see langword="null" /> or transparent.
        /// </value>
        public SolidColorBrush Background { get; set; }

        /// <summary>
        ///     Gets the settings used by the rendering or drawing engine.
        /// </summary>
        /// <value>
        ///     An instance of <see cref="WpfDrawingSettings" /> specifying all
        ///     the options for rendering or drawing.
        /// </value>
        public WpfDrawingSettings DrawingSettings { get; }

        #endregion

        #region IDisposable Members

        /// <overloads>
        ///     This releases all resources used by the <see cref="SvgConverter" /> object.
        /// </overloads>
        /// <summary>
        ///     This releases all resources used by the <see cref="SvgConverter" /> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     This releases the unmanaged resources used by the <see cref="SvgConverter" />
        ///     and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     This is <see langword="true" /> if managed resources should be
        ///     disposed; otherwise, <see langword="false" />.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}