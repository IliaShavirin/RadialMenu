using System.Windows;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public struct WpfTextPosition
    {
        private double _rotation;

        public WpfTextPosition(Point location, double rotation)
        {
            if (double.IsNaN(rotation) || double.IsInfinity(rotation)) rotation = 0;
            Location = location;
            _rotation = rotation;
        }

        public Point Location { get; set; }

        public double Rotation
        {
            get => _rotation;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                    _rotation = 0;
                else
                    _rotation = value;
            }
        }
    }
}