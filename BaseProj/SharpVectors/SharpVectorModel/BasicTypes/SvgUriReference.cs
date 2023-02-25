using System;
using System.IO;
using System.Net;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCss.Css;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgUriReference : ISvgUriReference
    {
        #region Constructors and Destructor

        public SvgUriReference(SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;
            this.ownerElement.attributeChangeHandler += AttributeChange;
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                if (href == null)
                    href = new SvgAnimatedString(ownerElement.GetAttribute("href",
                        SvgDocument.XLinkNamespace));
                return href;
            }
        }

        #endregion

        #region Public Events

        public event NodeChangeHandler NodeChanged;

        #endregion

        #region Private Methods

        private string GetBaseUrl()
        {
            if (ownerElement.HasAttribute("xml:base")) return ownerElement.GetAttribute("xml:base");
            var parentNode = ownerElement.ParentNode as XmlElement;
            if (parentNode != null && parentNode.HasAttribute("xml:base")) return parentNode.GetAttribute("xml:base");

            return null;
        }

        #endregion

        #region Private Fields

        private string absoluteUri;
        private readonly SvgElement ownerElement;
        private ISvgAnimatedString href;

        #endregion

        #region Public Properties

        public string AbsoluteUri
        {
            get
            {
                if (absoluteUri == null)
                    if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                    {
                        var href = Href.AnimVal.Trim();

                        if (href.StartsWith("#")) return href;

                        var baseUri = ownerElement.BaseURI;
                        if (baseUri.Length == 0)
                        {
                            var sourceUri = new Uri(Href.AnimVal, UriKind.RelativeOrAbsolute);
                            if (sourceUri.IsAbsoluteUri) absoluteUri = sourceUri.ToString();
                        }
                        else
                        {
                            Uri sourceUri = null;
                            var xmlBaseUrl = GetBaseUrl();
                            if (!string.IsNullOrEmpty(xmlBaseUrl))
                                sourceUri = new Uri(new Uri(ownerElement.BaseURI),
                                    Path.Combine(xmlBaseUrl, Href.AnimVal));
                            else
                                sourceUri = new Uri(new Uri(ownerElement.BaseURI), Href.AnimVal);

                            absoluteUri = sourceUri.ToString();
                        }
                    }

                return absoluteUri;
            }
        }

        public XmlNode ReferencedNode
        {
            get
            {
                if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                {
                    var referencedNode = ownerElement.OwnerDocument.GetNodeByUri(AbsoluteUri);

                    var extReqElm =
                        ownerElement as ISvgExternalResourcesRequired;

                    if (referencedNode == null && extReqElm != null)
                        if (extReqElm.ExternalResourcesRequired.AnimVal)
                            throw new SvgExternalResourcesRequiredException();

                    return referencedNode;
                }

                return null;
            }
        }

        public WebResponse ReferencedResource
        {
            get
            {
                if (ownerElement.HasAttribute("href", SvgDocument.XLinkNamespace))
                {
                    var absoluteUri = AbsoluteUri;
                    if (!string.IsNullOrEmpty(absoluteUri))
                    {
                        var referencedResource = ownerElement.OwnerDocument.GetResource(
                            new Uri(absoluteUri));

                        var extReqElm =
                            ownerElement as ISvgExternalResourcesRequired;

                        if (referencedResource == null && extReqElm != null)
                            if (extReqElm.ExternalResourcesRequired.AnimVal)
                                throw new SvgExternalResourcesRequiredException();

                        return referencedResource;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Update handling

        private void AttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            var attribute = src as XmlAttribute;

            if (attribute.NamespaceURI == SvgDocument.XLinkNamespace &&
                attribute.LocalName == "href")
            {
                href = null;
                absoluteUri = null;
            }
        }

        private void ReferencedNodeChange(object src, XmlNodeChangedEventArgs args)
        {
            if (NodeChanged != null) NodeChanged(src, args);
        }

        #endregion
    }
}