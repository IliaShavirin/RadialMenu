using System;
using System.Windows;
using BaseProj.SharpVectors.SharpVectorCore.Utils;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    /// <summary>
    ///     Defines the interface required for a rendering node to interact with the renderer and the SVG DOM
    /// </summary>
    public abstract class WpfRenderingBase : DependencyObject, IDisposable
    {
        #region Private Fields

        protected SvgElement _svgElement;

        #endregion

        #region Constructors and Destructor

        protected WpfRenderingBase(SvgElement element)
        {
            _svgElement = element;
        }

        ~WpfRenderingBase()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public SvgElement Element => _svgElement;

        public virtual bool IsRecursive => false;

        #endregion

        #region Public Methods

        public virtual bool NeedRender(WpfDrawingRenderer renderer)
        {
            if (_svgElement.GetAttribute("display") == "none") return false;

            return true;
        }

        // define empty handlers by default
        public virtual void BeforeRender(WpfDrawingRenderer renderer)
        {
        }

        public virtual void Render(WpfDrawingRenderer renderer)
        {
        }

        public virtual void AfterRender(WpfDrawingRenderer renderer)
        {
        }

        public string GetElementName()
        {
            if (_svgElement == null) return string.Empty;
            var elementId = _svgElement.Id;
            if (elementId != null) elementId = elementId.Trim();
            if (string.IsNullOrEmpty(elementId)) return string.Empty;
            if (elementId.Contains("&#x")) elementId = HttpUtility.HtmlDecode(elementId);
            if (elementId.Contains("レイヤー"))
                elementId = elementId.Replace("レイヤー", "Layer");
            else if (elementId.Equals("台紙"))
                elementId = "Mount";
            else if (elementId.Equals("キャプション")) elementId = "Caption";
            var numberId = 0;
            if (int.TryParse(elementId, out numberId)) return string.Empty;

            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(' ', '_');
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        public static string GetElementName(SvgElement element)
        {
            if (element == null) return string.Empty;
            var elementId = element.Id;
            if (elementId != null) elementId = elementId.Trim();
            if (string.IsNullOrEmpty(elementId)) return string.Empty;
            if (elementId.Contains("&#x")) elementId = HttpUtility.HtmlDecode(elementId);
            if (elementId.Contains("レイヤー"))
                elementId = elementId.Replace("レイヤー", "Layer");
            else if (elementId.Equals("イラスト"))
                elementId = "Illustration";
            else if (elementId.Equals("台紙"))
                elementId = "Mount";
            else if (elementId.Equals("キャプション"))
                elementId = "Caption";
            else if (elementId.Equals("細線")) elementId = "ThinLine";
            var numberId = 0;
            if (int.TryParse(elementId, out numberId)) return string.Empty;

            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(' ', '_');
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}