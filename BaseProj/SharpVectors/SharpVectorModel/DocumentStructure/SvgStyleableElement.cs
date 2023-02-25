using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCss.Css;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    /// <summary>
    ///     SvgStyleableElement is an extension to the Svg DOM to create a class for all elements that are styleable.
    /// </summary>
    public abstract class SvgStyleableElement : SvgElement, ISvgStylable
    {
        #region Private static fields

        private static readonly Regex isImportant = new Regex(@"!\s*important$");

        #endregion

        #region Constructors

        internal SvgStyleableElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                var localName = attribute.LocalName;
                if (presentationAttributes.ContainsKey(localName)) presentationAttributes.Remove(localName);

                switch (attribute.LocalName)
                {
                    case "class":
                        className = null;
                        // class changes need to propagate to children and invalidate CSS
                        break;
                }
            }

            base.HandleAttributeChange(attribute);
        }

        #endregion

        #region Private Fields

        #endregion

        #region ISvgStylable Members

        #region ClassName

        private ISvgAnimatedString className;

        public ISvgAnimatedString ClassName
        {
            get
            {
                if (className == null) className = new SvgAnimatedString(GetAttribute("class", string.Empty));
                return className;
            }
        }

        #endregion

        #region PresentationAttributes

        private readonly Dictionary<string, ICssValue> presentationAttributes = new Dictionary<string, ICssValue>();

        public ICssValue GetPresentationAttribute(string name)
        {
            if (!presentationAttributes.ContainsKey(name))
            {
                ICssValue result;
                var attValue = GetAttribute(name, string.Empty).Trim();
                if (attValue != null && attValue.Length > 0)
                {
                    if (isImportant.IsMatch(attValue))
                        result = null;
                    else
                        result = CssValue.GetCssValue(attValue, false);
                }
                else
                {
                    result = null;
                }

                presentationAttributes[name] = result;
            }

            return presentationAttributes[name];
        }

        #endregion

        #endregion

        #region GetValues

        public string GetPropertyValue(string name)
        {
            return GetComputedStyle(string.Empty).GetPropertyValue(name);
        }

        public string GetPropertyValue(string name1, string name2)
        {
            var cssString = GetComputedStyle(string.Empty).GetPropertyValue(name1);
            if (cssString == null) cssString = GetComputedStyle(string.Empty).GetPropertyValue(name2);

            return cssString;
        }

        public override ICssStyleDeclaration GetComputedStyle(string pseudoElt)
        {
            if (cachedCSD == null)
            {
                var csd = (CssCollectedStyleDeclaration)base.GetComputedStyle(pseudoElt);

                var cssPropNames = OwnerDocument.CssPropertyProfile.GetAllPropertyNames().GetEnumerator();
                while (cssPropNames.MoveNext())
                {
                    var cssPropName = cssPropNames.Current;
                    var cssValue = (CssValue)GetPresentationAttribute(cssPropName);
                    if (cssValue != null)
                        csd.CollectProperty(cssPropName, 0, cssValue,
                            CssStyleSheetType.NonCssPresentationalHints, string.Empty);
                }

                cachedCSD = csd;
            }

            return cachedCSD;
        }

        #endregion
    }
}