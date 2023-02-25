using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfRenderingHelper : DependencyObject
    {
        #region Constructors and Destructor

        public WpfRenderingHelper(WpfDrawingRenderer renderer)
        {
            _currentLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            _renderer = renderer;
            _rendererMap = new Dictionary<ISvgElement, WpfRenderingBase>();
        }

        #endregion

        #region Private Fields

        private readonly string _currentLang;
        private readonly WpfDrawingRenderer _renderer;
        private readonly Dictionary<ISvgElement, WpfRenderingBase> _rendererMap;

        #endregion

        #region Public Methods

        public void Render(ISvgDocument docElement)
        {
            var root = docElement.RootElement as SvgSvgElement;

            if (root != null) Render(root);
        }

        public void Render(SvgDocument docElement)
        {
            var root = docElement.RootElement as SvgSvgElement;

            if (root != null) Render(root);
        }

        public void Render(ISvgElement svgElement)
        {
            if (svgElement == null) return;

            var elementName = svgElement.LocalName;

            if (string.Equals(elementName, "use"))
                RenderUseElement(svgElement);
            else
                RenderElement(svgElement);
        }

        public void RenderChildren(ISvgElement svgElement)
        {
            if (svgElement == null) return;

            var elementName = svgElement.LocalName;

            if (string.Equals(elementName, "switch"))
                RenderSwitchChildren(svgElement);
            else
                RenderElementChildren(svgElement);
        }

        public void RenderMask(ISvgElement svgElement)
        {
            if (svgElement == null) return;

            var elementName = svgElement.LocalName;

            if (string.Equals(elementName, "switch"))
                RenderSwitchChildren(svgElement);
            else
                RenderElementChildren(svgElement);
        }

        #endregion

        #region Private Methods

        private void RenderElement(ISvgElement svgElement)
        {
            var isNotRenderable = !svgElement.IsRenderable;
            //bool isNotRenderable = !svgElement.IsRenderable || String.Equals(svgElement.LocalName, "a");

            if (string.Equals(svgElement.LocalName, "a"))
            {
            }

            if (isNotRenderable) return;

            WpfRenderingBase renderingNode = WpfRendering.Create(svgElement);
            if (renderingNode == null) return;

            if (!renderingNode.NeedRender(_renderer))
            {
                renderingNode.Dispose();
                renderingNode = null;
                return;
            }

            _rendererMap[svgElement] = renderingNode;
            renderingNode.BeforeRender(_renderer);

            renderingNode.Render(_renderer);

            if (!renderingNode.IsRecursive && svgElement.HasChildNodes) RenderChildren(svgElement);

            renderingNode = _rendererMap[svgElement];
            renderingNode.AfterRender(_renderer);

            _rendererMap.Remove(svgElement);

            renderingNode.Dispose();
            renderingNode = null;
        }

        private void RenderUseElement(ISvgElement svgElement)
        {
            var useElement = (SvgUseElement)svgElement;

            var document = useElement.OwnerDocument;

            var refEl = useElement.ReferencedElement;
            if (refEl == null)
                return;
            var isImported = false;
            // For the external node, the documents are different, and we may not be
            // able to insert this node, so we first import it...
            if (useElement.OwnerDocument != refEl.OwnerDocument)
            {
                var importedNode =
                    useElement.OwnerDocument.ImportNode(refEl, true) as XmlElement;

                if (importedNode != null)
                {
                    var importedSvgElement = importedNode as SvgElement;
                    if (importedSvgElement != null)
                    {
                        importedSvgElement.Imported = true;
                        importedSvgElement.ImportNode = refEl as SvgElement;
                        importedSvgElement.ImportDocument = refEl.OwnerDocument as SvgDocument;
                    }

                    refEl = importedNode;
                    isImported = true;
                }
            }

            var refElParent = (XmlElement)refEl.ParentNode;
            useElement.OwnerDocument.Static = true;
            useElement.CopyToReferencedElement(refEl);
            if (!isImported) // if imported, we do not need to remove it...
                refElParent.RemoveChild(refEl);
            useElement.AppendChild(refEl);

            // Now, render the use element...
            RenderElement(svgElement);

            useElement.RemoveChild(refEl);
            useElement.RestoreReferencedElement(refEl);
            if (!isImported) refElParent.AppendChild(refEl);
            useElement.OwnerDocument.Static = false;
        }

        private void RenderElementChildren(ISvgElement svgElement)
        {
            foreach (XmlNode node in svgElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;

                var element = node as SvgElement;
                if (element != null) Render(element);
            }
        }

        private void RenderSwitchChildren(ISvgElement svgElement)
        {
            // search through all child elements and find one that passes all tests
            foreach (XmlNode node in svgElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;

                var element = node as SvgElement;
                var testsElement = node as ISvgTests;
                if (element != null && testsElement != null && PassesSwitchAllTest(testsElement))
                {
                    Render(element);

                    // make sure we only render the first element that passes
                    break;
                }
            }
        }

        private bool PassesSwitchAllTest(ISvgTests element)
        {
            var ownerDocument = ((SvgElement)element).OwnerDocument;

            var requiredFeatures = true;
            if (element.RequiredFeatures.NumberOfItems > 0)
                foreach (var req in element.RequiredFeatures)
                    if (!ownerDocument.Supports(req, string.Empty))
                    {
                        requiredFeatures = false;
                        break;
                    }

            if (!requiredFeatures) return false;

            var requiredExtensions = true;
            if (element.RequiredExtensions.NumberOfItems > 0)
                foreach (var req in element.RequiredExtensions)
                    if (!ownerDocument.Supports(req, string.Empty))
                    {
                        requiredExtensions = false;
                        break;
                    }

            if (!requiredExtensions) return false;

            var systemLanguage = true;
            if (element.SystemLanguage.NumberOfItems > 0)
            {
                systemLanguage = false;
                // TODO: or if one of the languages indicated by user preferences exactly 
                // equals a prefix of one of the languages given in the value of this 
                // parameter such that the first tag character following the prefix is "-".

                foreach (var req in element.SystemLanguage)
                    if (string.Equals(req, _currentLang, StringComparison.OrdinalIgnoreCase))
                        systemLanguage = true;
            }

            return systemLanguage;
        }

        #endregion
    }
}