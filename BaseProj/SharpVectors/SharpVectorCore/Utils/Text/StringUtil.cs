namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Text
{
    public class StringUtil
    {
        public static bool IsInArray(string[] arrayToLookIn, string strToFind)
        {
            foreach (var item in arrayToLookIn)
                if (item == strToFind)
                    return true;
            return false;
        }
    }
}