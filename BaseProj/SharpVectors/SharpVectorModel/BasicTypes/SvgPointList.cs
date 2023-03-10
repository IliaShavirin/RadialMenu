// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     This class defines a list of SvgPoint objects.
    /// </summary>
    public sealed class SvgPointList : SvgList<ISvgPoint>, ISvgPointList
    {
        public void FromString(string listString)
        {
            // remove existing list items
            Clear();

            if (listString != null)
            {
                var len = listString.Length; // temp

                if (len > 0)
                {
                    var p = 0; // pos
                    char c; // temp
                    var sNum = -1; // start of the number
                    var eNum = -1; // end of the number
                    var seenComma = false; // to handle 123,,123
                    var tempSNum = -1; // start of the number (held in temp until two numbers are found)
                    var tempENum = -1; // end of the number (held in temp until two numbers are found)

                    // This used to be a regex-- it is *much* faster this way
                    while (p < len)
                    {
                        // Get the char in a temp
                        c = listString[p];

                        // TODO: worry about NEL?
                        if (c == '\t' || c == '\r' || c == '\n' || c == 0x20 || c == ',')
                        {
                            // Special handling for two commas
                            if (c == ',')
                            {
                                if (seenComma && sNum < 0)
                                    throw new SvgException(SvgExceptionType.SvgInvalidValueErr);
                                seenComma = true;
                            }

                            // Are we in a number?
                            if (sNum >= 0)
                            {
                                // The end of the number is the previous char
                                eNum = p - 1;

                                // Is this the x or y?
                                if (tempSNum == -1)
                                {
                                    // must be the x, hang onto it for a second
                                    tempSNum = sNum;
                                    tempENum = eNum;
                                }
                                else
                                {
                                    // must be the y, use temp as x and append the item
                                    AppendItem(new SvgPoint(
                                        SvgNumber.ParseNumber(listString.Substring(tempSNum, tempENum - tempSNum + 1)),
                                        SvgNumber.ParseNumber(listString.Substring(sNum, eNum - sNum + 1))));
                                    tempSNum = -1;
                                    tempENum = -1;
                                }

                                // Reset
                                sNum = -1;
                                eNum = -1;
                                seenComma = false;
                            }
                        }
                        else if (sNum == -1)
                        {
                            sNum = p;
                        }
                        // OPTIMIZE: Right here we could check for [Ee] to save some time in IndexOfAny later

                        // Move to next char
                        p++;
                    }

                    // We need to handle the end of the buffer as a delimiter
                    if (sNum >= 0)
                    {
                        if (tempSNum == -1)
                            throw new SvgException(SvgExceptionType.SvgInvalidValueErr);

                        // The end of the number is the previous char
                        eNum = p - 1;
                        // must be the y, use temp as x and append the item
                        AppendItem(new SvgPoint(
                            SvgNumber.ParseNumber(listString.Substring(tempSNum, tempENum - tempSNum + 1)),
                            SvgNumber.ParseNumber(listString.Substring(sNum, eNum - sNum + 1))));
                    }
                    else if (tempSNum != -1)
                    {
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr);
                    }
                }
            }
        }

        #region Constructors

        public SvgPointList()
        {
        }

        public SvgPointList(string listString)
        {
            FromString(listString);
        }

        #endregion
    }
}