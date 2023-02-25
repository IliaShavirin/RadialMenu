namespace BaseProj.SharpVectors.SharpVectorRuntime.Utils
{
    internal static class TryCast
    {
        public static bool Cast<B, D>(B baseObject, out D derivedObject)
            where D : class
        {
            if (baseObject == null)
            {
                derivedObject = null;
                return false;
            }

            derivedObject = baseObject as D;

            return derivedObject != null;
        }
    }
}