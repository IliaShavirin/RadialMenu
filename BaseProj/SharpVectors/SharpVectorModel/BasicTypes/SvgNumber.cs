using System;
using System.Globalization;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;
using BaseProj.SharpVectors.SharpVectorCss.Css;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgNumber : ISvgNumber
    {
        #region Public Static Properties

        public static NumberFormatInfo Format => CssNumber.Format;

        #endregion

        #region ISvgNumber Nembers

        public double Value { get; set; }

        #endregion

        #region Private Fields

        private static readonly string numberPattern = @"(?<number>(\+|-)?\d*\.?\d+((e|E)(\+|-)?\d+)?)";
        private static Regex reNumber = new Regex("^" + numberPattern + "$");

        private static readonly Regex reUnit = new Regex("[a-z]+$");

        #endregion

        #region Constructors and Destructor

        public SvgNumber(float val)
        {
            Value = val;
        }

        public SvgNumber(string str)
        {
            Value = ParseNumber(str);
        }

        #endregion

        #region Public Static Methods

        public static string ScientificToDec(string sc)
        {
            if (sc.IndexOfAny(new[] { 'e', 'E' }) > -1)
            {
                sc = sc.Trim();
                // remove the unit
                var match = reUnit.Match(sc);
                return ParseNumber(sc.Substring(0, sc.Length - match.Length)).ToString(Format) + match.Value;
            }

            return sc;
        }

        public static double ParseNumber(string str)
        {
            try
            {
                return double.Parse(str, Format);
            }
            catch (Exception e)
            {
                throw new DomException(DomExceptionType.SyntaxErr,
                    "Input string was not in a correct format: " + str, e);
            }
        }

        //public static double ParseNumber(string str)
        //{
        //    double val;
        //    int index = str.IndexOfAny(new Char[] { 'E', 'e' });
        //    if (index > -1)
        //    {
        //        double number = SvgNumber.ParseNumber(str.Substring(0, index));
        //        double power = SvgNumber.ParseNumber(str.Substring(index + 1));

        //        val = Math.Pow(10, power) * number;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            val = Double.Parse(str, SvgNumber.Format);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new DomException(DomExceptionType.SyntaxErr,
        //                "Input string was not in a correct format: " + str, e);
        //        }
        //    }

        //    return val;
        //}

        //public static float ParseNumber(string str)
        //{
        //    float val;
        //    int index = str.IndexOfAny(new Char[]{'E','e'});
        //    if (index>-1)
        //    {
        //        float number = SvgNumber.ParseNumber(str.Substring(0, index));
        //        float power  = SvgNumber.ParseNumber(str.Substring(index+1));

        //        val = (float) Math.Pow(10, power) * number;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            val = Single.Parse(str, SvgNumber.Format);
        //        }
        //        catch(Exception e)
        //        {
        //            throw new DomException(DomExceptionType.SyntaxErr, 
        //                "Input string was not in a correct format: " + str, e);
        //        }
        //    }

        //    return val;
        //}

        public static double CalcAngleDiff(double a1, double a2)
        {
            while (a1 < 0) a1 += 360;
            a1 %= 360;

            while (a2 < 0) a2 += 360;
            a2 %= 360;

            var diff = a1 - a2;

            while (diff < 0) diff += 360;
            diff %= 360;

            return diff;
        }

        public static double CalcAngleBisection(double a1, double a2)
        {
            var diff = CalcAngleDiff(a1, a2);
            var bisect = a1 - diff / 2F;

            while (bisect < 0)
                bisect += 360;

            bisect %= 360;
            return bisect;
        }

        #endregion
    }
}