// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Stylesheets;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Net;
using BaseProj.SharpVectors.SharpVectorCss.Stylesheets;
using BaseProj.SharpVectors.SharpVectorDom;

namespace BaseProj.SharpVectors.SharpVectorCss.Css
{
    /// <summary>
    ///     A XmlDocument with CSS support
    /// </summary>
    public class CssXmlDocument : Document, IDocumentCss, ICssView
    {
        #region IViewCss Members

        /// <summary>
        ///     This method is used to get the computed style as it is defined in [CSS2].
        /// </summary>
        /// <param name="elt">The element whose style is to be computed. This parameter cannot be null.</param>
        /// <param name="pseudoElt">The pseudo-element or null if none.</param>
        /// <returns>The computed style. The CSSStyleDeclaration is read-only and contains only absolute values.</returns>
        public ICssStyleDeclaration GetComputedStyle(XmlElement elt, string pseudoElt)
        {
            if (elt == null) throw new NullReferenceException();
            if (!(elt is CssXmlElement))
                throw new DomException(DomExceptionType.SyntaxErr, "element must of type CssXmlElement");
            return ((CssXmlElement)elt).GetComputedStyle(pseudoElt);
        }

        #endregion

        #region IDocumentCss Members

        /// <summary>
        ///     This method is used to retrieve the override style declaration for a specified element and a specified
        ///     pseudo-element.
        /// </summary>
        /// <param name="elt">The element whose style is to be modified. This parameter cannot be null.</param>
        /// <param name="pseudoElt">The pseudo-element or null if none.</param>
        /// <returns>The override style declaration.</returns>
        public ICssStyleDeclaration GetOverrideStyle(XmlElement elt, string pseudoElt)
        {
            throw new NotImplementedException("CssXmlDocument.GetOverrideStyle()");
        }

        #endregion

        #region Private Fields

        internal List<string[]> styleElements = new List<string[]>();
        internal MediaList _currentMedia = new MediaList("all");
        public CssStyleSheet UserAgentStyleSheet;
        public CssStyleSheet UserStyleSheet;

        private StyleSheetList _StyleSheets;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        ///     Initializes a new instance of CssXmlDocument
        /// </summary>
        public CssXmlDocument()
        {
            setupNodeChangeListeners();

            //SharpVectors.Net.ExtendedHttpWebRequest.Register();
            DataWebRequest.Register();
        }

        /// <summary>
        ///     Initializes a new instance of CssXmlDocument
        /// </summary>
        /// <param name="nt">The name table to use</param>
        public CssXmlDocument(XmlNameTable nt)
            : base(nt)
        {
            setupNodeChangeListeners();

            //SharpVectors.Net.ExtendedHttpWebRequest.Register();
            DataWebRequest.Register();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CssXmlDocument" /> handles DOM dynamic changes.
        ///     Sometimes (when loading or rendering) this needs to be disabled.
        ///     See <see href="StaticSection" /> for more information about use
        /// </summary>
        /// <value><c>true</c> if static; otherwise, <c>false</c>.</value>
        public bool Static { get; set; }

        public MediaList Media
        {
            get => _currentMedia;
            set => _currentMedia = value;
        }

        public CssPropertyProfile CssPropertyProfile { get; set; } = new CssPropertyProfile();

        public string Url => BaseURI;

        /// <summary>
        ///     All the stylesheets associated with this document
        /// </summary>
        public IStyleSheetList StyleSheets
        {
            get
            {
                if (_StyleSheets == null) _StyleSheets = new StyleSheetList(this);
                return _StyleSheets;
            }
        }

        #endregion

        #region Public Methods

        public override XmlElement CreateElement(string prefix, string localName, string ns)
        {
            return new CssXmlElement(prefix, localName, ns, this);
        }

        /// <summary>
        ///     Loads a XML document, compare to XmlDocument.Load()
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename)
        {
            using (StaticSection.Use(this))
            {
                // remove any hash (won't work for local files)
                var hashStart = filename.IndexOf("#");
                if (hashStart > -1) filename = filename.Substring(0, hashStart);
                base.Load(filename);
            }
        }

        public override void LoadXml(string xml)
        {
            using (StaticSection.Use(this))
            {
                //base.LoadXml(xml);
                // we use a stream here, only not to use SvgDocument.Load(XmlReader)
                using (var xmlStream = new StringReader(xml))
                {
                    Load(xmlStream);
                }
            }
        }

        //JR added in
        public override void Load(XmlReader reader)
        {
            using (StaticSection.Use(this))
            {
                base.Load(reader);
            }
        }

        /// <summary>
        ///     Adds a element type to be used as style elements (e.g. as in the HTML style element)
        /// </summary>
        /// <param name="ns">The namespace URI of the element</param>
        /// <param name="localName">The local-name of the element</param>
        public void AddStyleElement(string ns, string localName)
        {
            styleElements.Add(new[] { ns, localName });
        }

        /// <summary>
        ///     Sets the user agent stylesheet for this document
        /// </summary>
        /// <param name="href">The URI to the stylesheet</param>
        public void SetUserAgentStyleSheet(string href)
        {
            UserAgentStyleSheet =
                new CssStyleSheet(null, href, string.Empty, string.Empty, null, CssStyleSheetType.UserAgent);
        }

        /// <summary>
        ///     Sets the user stylesheet for this document
        /// </summary>
        /// <param name="href">The URI to the stylesheet</param>
        public void SetUserStyleSheet(string href)
        {
            UserStyleSheet = new CssStyleSheet(null, href, string.Empty, string.Empty, null, CssStyleSheetType.User);
        }

        public void AddStyleSheet(string href)
        {
            UserStyleSheet = new CssStyleSheet(null, href, string.Empty, string.Empty, null, CssStyleSheetType.User);
        }

        public WebResponse GetResource(Uri absoluteUri)
        {
            //WebRequest request = WebRequest.Create(absoluteUri);
            WebRequest request = new ExtendedHttpWebRequest(absoluteUri);
            var response = request.GetResponse();

            return response;
        }

        #endregion

        #region Update handling

        private void setupNodeChangeListeners()
        {
            XmlNodeChangedEventHandler handler = NodeChangedEvent;

            NodeChanged += handler;
            NodeInserted += handler;
            //NodeRemoving += handler;
            NodeRemoved += handler;
        }

        public void NodeChangedEvent(object src, XmlNodeChangedEventArgs args)
        {
            if (!Static)
            {
                #region Attribute updates

                // xmlns:xml is auto-inserted whenever a selectNode is performed, we don't want those events
                if (args.Node is XmlText && args.NewParent is XmlAttribute && args.NewParent.Name != "xmlns:xml")
                {
                    var attr = args.NewParent as XmlAttribute;
                    var elm = attr.OwnerElement as CssXmlElement;
                    if (elm != null) elm.AttributeChange(attr, args);
                }
                else if (args.Node is XmlAttribute && args.Node.Name != "xmlns:xml")
                {
                    // the cause of the change is a XmlAttribute => happens during inserting or removing
                    var oldElm = args.OldParent as CssXmlElement;
                    if (oldElm != null) oldElm.AttributeChange(args.Node, args);

                    var newElm = args.NewParent as CssXmlElement;
                    if (newElm != null) newElm.AttributeChange(args.Node, args);
                }

                #endregion

                #region OnElementChange

                if (args.Node is XmlText && args.NewParent is XmlElement)
                {
                    var element = (CssXmlElement)args.NewParent;
                    element.ElementChange(src, args);
                }
                else if (args.Node is CssXmlElement)
                {
                    if (args.Action == XmlNodeChangedAction.Insert || args.Action == XmlNodeChangedAction.Change)
                    {
                        // Changes to a child XML node may affect the sibling offsets (for example in tspan)
                        // By calling the parent's OnElementChange, invalidation will propogate back to Node
                        var newParent = (CssXmlElement)args.NewParent;
                        newParent.ElementChange(src, args);
                    }

                    if (args.Action == XmlNodeChangedAction.Remove)
                    {
                        // Removing a child XML node may affect the sibling offsets (for example in tspan)
                        var oldParent = (CssXmlElement)args.OldParent;
                        oldParent.ElementChange(src, args);
                    }
                }

                #endregion
            }
        }

        #endregion
    }
}