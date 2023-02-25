// <developer>niklas@protocol7.com</developer>
// <completed>90</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     This interface defines a list of SvgPathSeg objects.
    /// </summary>
    public interface ISvgPathSegList
    {
        int NumberOfItems { get; }
        void Clear();
        ISvgPathSeg Initialize(ISvgPathSeg newItem);
        ISvgPathSeg GetItem(int index);
        ISvgPathSeg InsertItemBefore(ISvgPathSeg newItem, int index);
        ISvgPathSeg ReplaceItem(ISvgPathSeg newItem, int index);
        ISvgPathSeg RemoveItem(int index);
        ISvgPathSeg AppendItem(ISvgPathSeg newItem);
    }
}