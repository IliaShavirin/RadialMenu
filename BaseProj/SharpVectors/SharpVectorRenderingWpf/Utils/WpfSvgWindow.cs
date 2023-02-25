using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils
{
    public class WpfSvgWindow : SvgWindow
    {
        #region Private fields

        #endregion

        #region Contructors and Destructor

        public WpfSvgWindow(long innerWidth, long innerHeight, ISvgRenderer renderer)
            : base(innerWidth, innerHeight, renderer)
        {
        }

        public WpfSvgWindow(SvgWindow parentWindow, long innerWidth, long innerHeight)
            : base(parentWindow, innerWidth, innerHeight)
        {
        }

        #endregion

        #region ISvgWindow Members

        public XmlReaderSettings CustomSettings { get; set; }

        public override long InnerWidth
        {
            get => base.InnerWidth;
            set => base.InnerWidth = value;
        }

        public override long InnerHeight
        {
            get => base.InnerHeight;
            set => base.InnerHeight = value;
        }

        public override string Source
        {
            get
            {
                var document = (SvgDocument)Document;
                return document != null ? document.Url : string.Empty;
            }
            set
            {
                var uri = new Uri(new Uri(
                    Assembly.GetExecutingAssembly().Location), value);

                LoadDocument(uri);
            }
        }

        public override DirectoryInfo WorkingDir => WpfApplicationContext.ExecutableDirectory;

        public void LoadDocument(Uri documentUri)
        {
            if (documentUri == null || !documentUri.IsAbsoluteUri) return;

            var document = new SvgDocument(this);
            if (CustomSettings != null) document.CustomSettings = CustomSettings;
            document.Load(documentUri.AbsoluteUri);

            Document = document;
        }

        public void LoadDocument(string documentSource)
        {
            if (string.IsNullOrEmpty(documentSource)) return;

            var uri = new Uri(new Uri(
                    Assembly.GetExecutingAssembly().Location),
                documentSource);

            LoadDocument(uri);
        }

        public void LoadDocument(Stream documentStream)
        {
            if (documentStream == null) return;

            var document = new SvgDocument(this);
            if (CustomSettings != null) document.CustomSettings = CustomSettings;
            document.Load(documentStream);

            Document = document;
        }

        public void LoadDocument(TextReader textReader)
        {
            if (textReader == null) return;

            var document = new SvgDocument(this);
            if (CustomSettings != null) document.CustomSettings = CustomSettings;
            document.Load(textReader);

            Document = document;
        }

        public void LoadDocument(XmlReader xmlReader)
        {
            if (xmlReader == null) return;

            var document = new SvgDocument(this);
            if (CustomSettings != null) document.CustomSettings = CustomSettings;
            document.Load(xmlReader);

            Document = document;
        }

        public override void Alert(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            MessageBox.Show(message);
        }

        public override SvgWindow CreateOwnedWindow(long innerWidth, long innerHeight)
        {
            return new WpfSvgWindow(this, innerWidth, innerHeight);
        }

        #endregion
    }
}