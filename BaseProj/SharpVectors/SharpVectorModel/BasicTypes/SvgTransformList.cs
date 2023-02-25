using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgTransformList.
    /// </summary>
    public sealed class SvgTransformList : SvgList<ISvgTransform>, ISvgTransformList
    {
        #region Private Fields

        //Regex re = new Regex("([A-Za-z]+)\\s*\\(([\\-0-9\\.\\,\\seE]+)\\)");

        private static readonly Regex _regExtract = new Regex(
            "([A-Za-z]+)\\s*\\(([\\-0-9\\.eE\\,\\s]+)\\)", RegexOptions.Compiled);

        #endregion

        #region Public Properties

        public SvgMatrix TotalMatrix
        {
            get
            {
                if (NumberOfItems == 0) return SvgMatrix.Identity;

                var matrix = GetItem(0).Matrix;

                for (uint i = 1; i < NumberOfItems; i++) matrix = matrix.Multiply(GetItem(i).Matrix);

                return (SvgMatrix)matrix;
            }
        }

        #endregion

        #region Public Methods

        public void FromString(string listString)
        {
            Clear();

            if (!string.IsNullOrEmpty(listString))
            {
                var match = _regExtract.Match(listString);
                while (match.Success)
                {
                    AppendItem(new SvgTransform(match.Value));
                    match = match.NextMatch();
                }
            }
        }

        #endregion

        #region Constructors

        public SvgTransformList()
        {
        }

        public SvgTransformList(string listString)
        {
            FromString(listString);
        }

        #endregion

        #region ISvgTransformList Members

        public ISvgTransform CreateSvgTransformFromMatrix(ISvgMatrix matrix)
        {
            return new SvgTransform((SvgMatrix)matrix);
        }

        public ISvgTransform Consolidate()
        {
            var result = CreateSvgTransformFromMatrix(TotalMatrix);

            Initialize(result);

            return result;
        }

        #endregion
    }
}