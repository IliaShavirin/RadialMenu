// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>75</completed>

using System;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorCss.Css;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgElement.
    /// </summary>
    public class SvgElement : CssXmlElement, ISvgElement
    {
        #region Constructors and Destructors

        public SvgElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Properties

        public bool Imported { get; set; }

        public SvgElement ImportNode { get; set; }

        public SvgDocument ImportDocument { get; set; }

        #endregion

        #region ISvgElement Members

        public new SvgDocument OwnerDocument => base.OwnerDocument as SvgDocument;

        public string Id
        {
            get => GetAttribute("id");
            set => SetAttribute("id", value);
        }

        public ISvgSvgElement OwnerSvgElement
        {
            get
            {
                if (Equals(OwnerDocument.DocumentElement)) return null;

                var parent = ParentNode;
                while (parent != null && !(parent is SvgSvgElement)) parent = parent.ParentNode;
                return parent as SvgSvgElement;
            }
        }

        public ISvgElement ViewportElement
        {
            get
            {
                if (Equals(OwnerDocument.DocumentElement)) return null;

                var parent = ParentNode;
                while (parent != null && !(parent is SvgSvgElement) && !(parent is SvgSymbolElement))
                    parent = parent.ParentNode;

                return parent as SvgElement;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        ///     This is <see langword="'true" /> if the element is renderable; otherwise,
        ///     it is <see langword="false" />.
        /// </value>
        public virtual bool IsRenderable => true;

        /// <summary>
        ///     Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        ///     An enumeration of the <see cref="SvgRenderingHint" /> specifying the rendering hint.
        /// </value>
        public virtual SvgRenderingHint RenderingHint => SvgRenderingHint.None;

        #endregion

        #region ISvgLangSpace Members

        public string XmlSpace
        {
            get
            {
                var s = GetAttribute("xml:space");
                if (string.IsNullOrEmpty(s))
                {
                    var par = ParentNode as SvgElement;
                    if (par != null)
                        s = par.XmlSpace;
                    else
                        s = "default";
                }

                return s;
            }
            set => SetAttribute("xml:space", value);
        }

        public string XmlLang
        {
            get
            {
                var s = GetAttribute("xml:lang");
                if (string.IsNullOrEmpty(s))
                {
                    var par = ParentNode as SvgElement;
                    if (par != null)
                        s = par.XmlLang;
                    else
                        s = string.Empty;
                }

                return s;
            }
            set => SetAttribute("xml:lang", value);
        }

        #endregion

        #region Other public methods

        public string ResolveUri(string uri)
        {
            uri = uri.Trim();
            if (uri.StartsWith("#")) return uri;

            var baseUri = BaseURI;
            if (baseUri.Length == 0)
                return uri;
            return new Uri(new Uri(baseUri), uri).AbsoluteUri;
        }

        /// <summary>
        ///     Whenever an SvgElementInstance is created for an SvgElement this
        ///     property is set. The value of this property is used by the renderer
        ///     to dispatch events. SvgElements that are &lt;use&gt;d exist in a
        ///     conceptual "instance tree" and the target of events for those elements
        ///     is the conceptual instance node represented by the SvgElementInstance.
        ///     <see cref="http://www.w3.org/TR/SVG/struct.html#UseElement" />
        ///     <see cref="http://www.w3.org/TR/SVG/struct.html#InterfaceSVGElementInstance" />
        /// </summary>
        public ISvgElementInstance ElementInstance { get; set; }

        #endregion
    }
}