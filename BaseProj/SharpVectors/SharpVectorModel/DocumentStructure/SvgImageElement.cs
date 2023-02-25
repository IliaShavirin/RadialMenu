using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgImageElement : SvgTransformableElement, ISvgImageElement
    {
        #region Constructors and Destructor

        public SvgImageElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
            UriReference = new SvgUriReference(this);
            svgFitToViewBox = new SvgFitToViewBox(this);
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        ///     Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        ///     An enumeration of the <see cref="SvgRenderingHint" /> specifying the rendering hint.
        ///     This will always return <see cref="SvgRenderingHint.Image" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Image;

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio => svgFitToViewBox.PreserveAspectRatio;

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region ISvgImageElement Members from SVG 1.2

        public SvgDocument GetImageDocument()
        {
            var window = SvgWindow;
            if (window == null)
                return null;
            return (SvgDocument)window.Document;
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                // This list may be too long to be useful...
                switch (attribute.LocalName)
                {
                    // Additional attributes
                    case "x":
                        x = null;
                        return;
                    case "y":
                        y = null;
                        return;
                    case "width":
                        width = null;
                        return;
                    case "height":
                        height = null;
                        return;
                }

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion

        #region Private Fields

        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private readonly SvgTests svgTests;
        private readonly SvgFitToViewBox svgFitToViewBox;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Public properties

        //public SvgRect CalculatedViewbox
        //{
        //    get
        //    {
        //        SvgRect viewBox = null;

        //        if (IsSvgImage)
        //        {
        //            SvgDocument doc = GetImageDocument();
        //            SvgSvgElement outerSvg = (SvgSvgElement)doc.DocumentElement;

        //            if (outerSvg.HasAttribute("viewBox"))
        //            {
        //                viewBox = (SvgRect)outerSvg.ViewBox.AnimVal;
        //            }
        //            else
        //            {
        //                viewBox = SvgRect.Empty;
        //            }
        //        }
        //        else
        //        {
        //            viewBox = new SvgRect(0, 0, Bitmap.Size.Width, Bitmap.Size.Height);
        //        }

        //        return viewBox;
        //    }
        //}

        public bool IsSvgImage
        {
            get
            {
                if (!Href.AnimVal.StartsWith("data:"))
                    try
                    {
                        var absoluteUri = UriReference.AbsoluteUri;
                        if (!string.IsNullOrEmpty(absoluteUri))
                        {
                            var svgUri = new Uri(absoluteUri, UriKind.Absolute);
                            if (svgUri.IsFile)
                                return absoluteUri.EndsWith(".svg",
                                           StringComparison.OrdinalIgnoreCase) ||
                                       absoluteUri.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase);
                        }

                        var resource = UriReference.ReferencedResource;
                        if (resource == null) return false;

                        // local files are returning as binary/octet-stream
                        // this "fix" tests the file extension for .svg and .svgz
                        var name = resource.ResponseUri.ToString().ToLower(CultureInfo.InvariantCulture);
                        return resource.ContentType.StartsWith("image/svg+xml") ||
                               name.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                               name.EndsWith(".svgz", StringComparison.OrdinalIgnoreCase);
                    }
                    catch (WebException)
                    {
                        return false;
                    }
                    catch (IOException)
                    {
                        return false;
                    }

                return false;
            }
        }

        public SvgWindow SvgWindow
        {
            get
            {
                if (IsSvgImage)
                {
                    var parentWindow = (SvgWindow)OwnerDocument.Window;

                    if (parentWindow != null)
                    {
                        var wnd = parentWindow.CreateOwnedWindow(
                            (long)Width.AnimVal.Value, (long)Height.AnimVal.Value);

                        var doc = new SvgDocument(wnd);
                        wnd.Document = doc;

                        var absoluteUri = UriReference.AbsoluteUri;

                        var svgUri = new Uri(absoluteUri, UriKind.Absolute);
                        if (svgUri.IsFile)
                        {
                            Stream resStream = File.OpenRead(svgUri.LocalPath);
                            doc.Load(absoluteUri, resStream);
                        }
                        else
                        {
                            var resStream = UriReference.ReferencedResource.GetResponseStream();
                            doc.Load(absoluteUri, resStream);
                        }

                        return wnd;
                    }
                }

                return null;
            }
        }

        #endregion

        #region ISvgImageElement Members

        public ISvgAnimatedLength Width
        {
            get
            {
                if (width == null) width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null) height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
                return height;
            }
        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (x == null) x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                return x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (y == null) y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                return y;
            }
        }

        public ISvgColorProfileElement ColorProfile
        {
            get
            {
                var colorProfile = GetAttribute("color-profile");

                if (string.IsNullOrEmpty(colorProfile)) return null;

                var profileElement = OwnerDocument.GetElementById(colorProfile);
                if (profileElement == null)
                {
                    var root = OwnerDocument.DocumentElement;
                    var elemList = root.GetElementsByTagName("color-profile");
                    if (elemList != null && elemList.Count != 0)
                        for (var i = 0; i < elemList.Count; i++)
                        {
                            var elementNode = elemList[i] as XmlElement;
                            if (elementNode != null && string.Equals(colorProfile,
                                    elementNode.GetAttribute("id")))
                            {
                                profileElement = elementNode;
                                break;
                            }
                        }
                }

                return profileElement as SvgColorProfileElement;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => UriReference.Href;

        public SvgUriReference UriReference { get; }

        public XmlElement ReferencedElement => UriReference.ReferencedNode as XmlElement;

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures => svgTests.RequiredFeatures;

        public ISvgStringList RequiredExtensions => svgTests.RequiredExtensions;

        public ISvgStringList SystemLanguage => svgTests.SystemLanguage;

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion
    }
}