namespace BaseProj.SharpVectors.SharpVectorCore.Utils
{
    public static class TryCast
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