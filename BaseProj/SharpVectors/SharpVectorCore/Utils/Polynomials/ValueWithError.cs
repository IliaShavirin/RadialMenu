namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Polynomials
{
    /// <summary>
    ///     Stucture used to return values with associated error tolerances
    /// </summary>
    public struct ValueWithError
    {
        public double Value;
        public double Error;

        public ValueWithError(double value, double error)
        {
            Value = value;
            Error = error;
        }
    }
}