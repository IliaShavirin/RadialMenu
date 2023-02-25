using System;
using System.IO;
using System.Security.AccessControl;
using BaseProj.SharpVectors.SharpVectorConverters.Utils;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    /// <summary>
    ///     This converts a directory (and optionally the sub-directories) of SVG
    ///     files to XAML files in a specified directory, maintaining the original
    ///     directory structure.
    /// </summary>
    public sealed class DirectorySvgConverter : SvgConverter
    {
        #region Private Fields

        private int _convertedCount;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfDrawingSettings" />
        ///     class with the specified drawing or rendering settings.
        /// </summary>
        /// <param name="settings">
        ///     This specifies the settings used by the rendering or drawing engine.
        ///     If this is <see langword="null" />, the default settings is used.
        /// </param>
        public DirectorySvgConverter(WpfDrawingSettings settings)
            : base(settings)
        {
            Overwrite = true;
            Recursive = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Convert the SVG files in the specified source directory, saving the
        ///     results in the specified destination directory.
        /// </summary>
        /// <param name="sourceInfo">
        ///     A <see cref="DirectoryInfo" /> specifying the source directory of
        ///     the SVG files.
        /// </param>
        /// <param name="destInfo">
        ///     A <see cref="DirectoryInfo" /> specifying the source directory of
        ///     the SVG files.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         If the <paramref name="sourceInfo" /> is <see langword="null" />.
        ///     </para>
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <para>
        ///         If the <paramref name="destInfo" /> is <see langword="null" />.
        ///     </para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If the directory specified by <paramref name="sourceInfo" /> does not
        ///     exists.
        /// </exception>
        public void Convert(DirectoryInfo sourceInfo, DirectoryInfo destInfo)
        {
            if (sourceInfo == null)
                throw new ArgumentNullException("sourceInfo",
                    "The source directory cannot be null (or Nothing).");
            if (destInfo == null)
                throw new ArgumentNullException("destInfo",
                    "The destination directory cannot be null (or Nothing).");
            if (!sourceInfo.Exists)
                throw new ArgumentException(
                    "The source directory must exists.", "sourceInfo");

            _convertedCount = 0;
            SourceDir = sourceInfo;
            DestinationDir = destInfo;
            DirectorySecurity dirSecurity = null;
            if (IncludeSecurity) dirSecurity = sourceInfo.GetAccessControl();
            if (!destInfo.Exists)
            {
                if (dirSecurity != null)
                    destInfo.Create(dirSecurity);
                else
                    destInfo.Create();
                destInfo.Attributes = sourceInfo.Attributes;
            }
            else
            {
                if (dirSecurity != null) destInfo.SetAccessControl(dirSecurity);
            }

            ProcessConversion(SourceDir, DestinationDir);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the directory copying is
        ///     recursive, that is includes the sub-directories.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if the sub-directories are
        ///     included in the directory copy; otherwise, it is <see langword="false" />.
        ///     The default is <see langword="true" />.
        /// </value>
        public bool Recursive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether an existing file is overwritten.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if existing file is overwritten;
        ///     otherwise, it is <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        public bool Overwrite { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the security settings of the
        ///     copied file is retained.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if the security settings of the
        ///     file is also copied; otherwise, it is <see langword="false" />. The
        ///     default is <see langword="false" />.
        /// </value>
        public bool IncludeSecurity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the copy operation includes
        ///     hidden directories and files.
        /// </summary>
        /// <value>
        ///     This property is <see langword="true" /> if hidden directories and files
        ///     are included in the copy operation; otherwise, it is
        ///     <see langword="false" />. The default is <see langword="false" />.
        /// </value>
        public bool IncludeHidden { get; set; }

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
        ///     Gets the source directory of the SVG files to be converted.
        /// </summary>
        /// <value>
        ///     A <see cref="DirectoryInfo" /> specifying the source directory of
        ///     the SVG files.
        /// </value>
        public DirectoryInfo SourceDir { get; private set; }

        /// <summary>
        ///     Gets the destination directory of the converted XAML files.
        /// </summary>
        /// <value>
        ///     A <see cref="DirectoryInfo" /> specifying the destination directory of
        ///     the converted XAML files.
        /// </value>
        public DirectoryInfo DestinationDir { get; private set; }

        /// <summary>
        ///     Gets the full path of the last SVG file not successfully converted.
        /// </summary>
        /// <value>
        ///     A string containing the full path of the last SVG file not
        ///     successfully converted to the XAML
        /// </value>
        /// <remarks>
        ///     Whenever an error occurred in the conversion of a file, the
        ///     conversion process will stop. Use this property to retrieve the full
        ///     path of the SVG file causing the error.
        /// </remarks>
        public string ErrorFile { get; private set; }

        #endregion

        #region Private Methods

        private void ProcessConversion(DirectoryInfo source, DirectoryInfo target)
        {
            // Convert the files in the specified directory...
            ConvertFiles(source, target);

            if (!Recursive) return;

            // If recursive, process any sub-directory...
            var arrSourceInfo = source.GetDirectories();

            var dirCount = arrSourceInfo == null ? 0 : arrSourceInfo.Length;

            for (var i = 0; i < dirCount; i++)
            {
                var sourceInfo = arrSourceInfo[i];
                var fileAttr = sourceInfo.Attributes;
                if (!IncludeHidden)
                    if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                DirectoryInfo targetInfo = null;
                if (IncludeSecurity)
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name,
                        sourceInfo.GetAccessControl());
                else
                    targetInfo = target.CreateSubdirectory(sourceInfo.Name);
                targetInfo.Attributes = fileAttr;

                ProcessConversion(sourceInfo, targetInfo);
            }
        }

        private void ConvertFiles(DirectoryInfo source, DirectoryInfo target)
        {
            ErrorFile = null;

            var fileConverter = new FileSvgConverter(SaveXaml,
                SaveZaml, DrawingSettings);
            fileConverter.Background = Background;
            fileConverter.FallbackOnWriterError = FallbackOnWriterError;

            var targetDirName = target.ToString();
            string xamlFilePath;

            var fileIterator = DirectoryUtils.FindFiles(
                source, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var svgFileName in fileIterator)
            {
                var fileExt = Path.GetExtension(svgFileName);
                if (string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                    try
                    {
                        var fileAttr = File.GetAttributes(svgFileName);
                        if (!IncludeHidden)
                            if ((fileAttr & FileAttributes.Hidden) == FileAttributes.Hidden)
                                continue;

                        xamlFilePath = Path.Combine(targetDirName,
                            Path.GetFileNameWithoutExtension(svgFileName) + ".xaml");

                        fileConverter.Convert(svgFileName, xamlFilePath);

                        File.SetAttributes(xamlFilePath, fileAttr);
                        // if required to set the security or access control
                        if (IncludeSecurity) File.SetAccessControl(xamlFilePath, File.GetAccessControl(svgFileName));

                        _convertedCount++;

                        if (fileConverter.WriterErrorOccurred) WriterErrorOccurred = true;
                    }
                    catch
                    {
                        ErrorFile = svgFileName;

                        throw;
                    }
            }
        }

        #endregion
    }
}