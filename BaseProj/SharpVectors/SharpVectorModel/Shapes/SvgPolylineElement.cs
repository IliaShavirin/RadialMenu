// <developer>niklas@protocol7.com</developer>
// <completed>90</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.Shapes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Shapes
{
    /// <summary>
    /// </summary>
    public sealed class SvgPolylineElement : SvgPolyElement, ISvgPolylineElement
    {
        #region Constructors and Destructor

        public SvgPolylineElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion
    }
}