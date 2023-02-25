using System.Collections.Generic;
using System.Windows;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfTextPlacement
    {
        #region Public Methods

        public void UpdatePositions(string targetText)
        {
            //if (String.IsNullOrEmpty(targetText) || (_positions == null || _positions.Count == 0))
            //{
            //    return;
            //}

            //int posCount  = _positions.Count;
            //int textCount = targetText.Length;

            //if (textCount <= posCount)
            //{
            //    return;
            //}
            //WpfTextPosition textPos = new WpfTextPosition(_location, _rotation);
            //for (int i = posCount; i < textCount; i++)
            //{
            //    _positions.Add(textPos);
            //}
        }

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public WpfTextPlacement()
        {
        }

        public WpfTextPlacement(Point location, double rotation)
        {
            if (double.IsNaN(rotation) || double.IsInfinity(rotation)) rotation = 0;

            Location = location;
            Rotation = rotation;
        }

        public WpfTextPlacement(Point location, double rotation,
            IList<WpfTextPosition> positions, bool isRotateOnly)
        {
            if (double.IsNaN(rotation) || double.IsInfinity(rotation)) rotation = 0;

            Location = location;
            Rotation = rotation;
            Positions = positions;
            IsRotateOnly = isRotateOnly;
        }

        #endregion

        #region Public Properties

        public bool HasPositions => Positions != null && Positions.Count != 0;

        public Point Location { get; }

        public double Rotation { get; }

        public bool IsRotateOnly { get; }

        public IList<WpfTextPosition> Positions { get; }

        #endregion
    }
}