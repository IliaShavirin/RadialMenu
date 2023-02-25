namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public struct CalculatedArcValues
    {
        public double CorrRx;
        public double CorrRy;
        public double Cx;
        public double Cy;
        public double AngleStart;
        public double AngleExtent;

        public CalculatedArcValues(double rx, double ry, double cx, double cy,
            double angleStart, double angleExtent)
        {
            CorrRx = rx;
            CorrRy = ry;
            Cx = cx;
            Cy = cy;
            AngleStart = angleStart;
            AngleExtent = angleExtent;
        }
    }
}