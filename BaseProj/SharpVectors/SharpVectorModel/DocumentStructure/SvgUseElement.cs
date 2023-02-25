using System;
using System.Globalization;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgUseElement : SvgTransformableElement, ISvgUseElement
    {
        #region Constructors and Destructor

        public SvgUseElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgURIReference.NodeChanged += ReferencedNodeChange;
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;
        private ISvgElementInstance instanceRoot;

        private readonly SvgTests svgTests;
        private readonly SvgUriReference svgURIReference;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

        // For rendering support...
        private string saveTransform;
        private string saveWidth;
        private string saveHeight;

        #endregion

        #region ISvgUseElement Members

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

        public ISvgAnimatedLength Width
        {
            get
            {
                if (width == null)
                    width = new SvgAnimatedLength(this, "width",
                        SvgLengthDirection.Horizontal, string.Empty);
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null)
                    height = new SvgAnimatedLength(this, "height",
                        SvgLengthDirection.Vertical, string.Empty);
                return height;
            }
        }

        public ISvgElementInstance InstanceRoot
        {
            get
            {
                if (instanceRoot == null) instanceRoot = new SvgElementInstance(ReferencedElement, this, null);
                return instanceRoot;
            }
        }

        public ISvgElementInstance AnimatedInstanceRoot => InstanceRoot;

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => svgURIReference.Href;

        public XmlElement ReferencedElement => svgURIReference.ReferencedNode as XmlElement;

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

        #region Update Handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
                switch (attribute.LocalName)
                {
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
            else if (attribute.NamespaceURI == SvgDocument.XLinkNamespace)
                switch (attribute.LocalName)
                {
                    case "href":
                        instanceRoot = null;
                        break;
                }

            base.HandleAttributeChange(attribute);
        }

        public void ReferencedNodeChange(object src, XmlNodeChangedEventArgs args)
        {
            //TODO - This is getting called too often!
            //instanceRoot = null;
        }

        #endregion

        #region Rendering

        public void CopyToReferencedElement(XmlElement refEl)
        {
            // X and Y become a translate portion of any transform, width and height may get passed on
            if (X.AnimVal.Value != 0 || Y.AnimVal.Value != 0)
            {
                saveTransform = GetAttribute("transform");
                //this.SetAttribute("transform", saveTransform + " translate(" 
                //    + X.AnimVal.Value + "," + Y.AnimVal.Value + ")");
                var transform = string.Format(CultureInfo.InvariantCulture,
                    "{0} translate({1},{2})", saveTransform, X.AnimVal.Value, Y.AnimVal.Value);
                SetAttribute("transform", transform);
            }

            // if (refEl is SvgSymbolElement)
            if (string.Equals(refEl.Name, "symbol", StringComparison.OrdinalIgnoreCase))
            {
                refEl.SetAttribute("width", HasAttribute("width") ? GetAttribute("width") : "100%");
                refEl.SetAttribute("height", HasAttribute("height") ? GetAttribute("height") : "100%");
            }

            // if (refEl is SvgSymbolElement)
            if (string.Equals(refEl.Name, "symbol", StringComparison.OrdinalIgnoreCase))
            {
                saveWidth = refEl.GetAttribute("width");
                saveHeight = refEl.GetAttribute("height");
                if (HasAttribute("width"))
                    refEl.SetAttribute("width", GetAttribute("width"));
                if (HasAttribute("height"))
                    refEl.SetAttribute("height", GetAttribute("height"));
            }
        }

        public void RestoreReferencedElement(XmlElement refEl)
        {
            if (saveTransform != null)
                SetAttribute("transform", saveTransform);
            if (saveWidth != null)
            {
                refEl.SetAttribute("width", saveWidth);
                refEl.SetAttribute("height", saveHeight);
            }
        }

        #endregion
    }
}