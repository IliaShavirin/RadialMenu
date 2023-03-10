using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgElementInstanceList : ISvgElementInstanceList
    {
        #region Private Fields

        private readonly SvgElementInstance[] items;

        #endregion

        #region Constructors

        public SvgElementInstanceList(SvgUseElement useElement, SvgElementInstance parent)
        {
            if (parent.CorrespondingElement == null)
            {
                // Handle non SVGElement cases
                items = new SvgElementInstance[0];
            }
            else if (parent.CorrespondingElement is ISvgUseElement)
            {
                // Handle recursive SVGUseElement cases
                items = new SvgElementInstance[1];
                var iUseElement = (ISvgUseElement)parent.CorrespondingElement;
                items[0] = (SvgElementInstance)iUseElement.InstanceRoot;
            }
            else
            {
                var xmlChildNodes = parent.CorrespondingElement.ChildNodes;
                for (var i = 0; i < xmlChildNodes.Count; i++)
                {
                    items[i] = new SvgElementInstance(xmlChildNodes[i], useElement, parent);
                    if (i > 0)
                    {
                        items[i].SetPreviousSibling(items[i - 1]);
                        items[i - 1].SetNextSibling(items[i]);
                    }
                }
            }
        }

        #endregion

        #region ISvgElementInstanceList Members

        public ulong Length => (ulong)items.GetLength(0);

        public ISvgElementInstance Item(ulong index)
        {
            if (index < Length)
                return (ISvgElementInstance)items.GetValue((int)index);
            return null;
        }

        #endregion
    }
}