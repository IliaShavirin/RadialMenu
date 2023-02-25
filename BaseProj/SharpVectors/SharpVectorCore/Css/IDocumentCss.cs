// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>	

using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Stylesheets;

namespace BaseProj.SharpVectors.SharpVectorCore.Css
{
    /// <summary>
    ///     This interface represents a document with a CSS view.
    /// </summary>
    public interface IDocumentCss : IDocumentStyle
    {
        /// <summary>
        ///     This method is used to retrieve the override style declaration for a specified element and a specified
        ///     pseudo-element.
        /// </summary>
        ICssStyleDeclaration GetOverrideStyle(XmlElement elt, string pseudoElt);
    }
}