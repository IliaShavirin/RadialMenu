// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>

using System;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgTransform.
    /// </summary>
    public sealed class SvgTransform : ISvgTransform
    {
        #region Enum SvgTransformType

        private enum SvgTransformType : short
        {
            Unknown,

            Matrix,

            Translate,

            Scale,

            Rotate,

            SkewX,

            SkewY
        }

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public SvgTransform()
        {
        }

        public SvgTransform(ISvgMatrix matrix)
        {
            Type = (short)SvgTransformType.Matrix;
            Matrix = matrix;
        }

        public SvgTransform(string str)
        {
            var start = str.IndexOf("(");
            var type = str.Substring(0, start);
            var valuesList = str.Substring(start + 1, str.Length - start - 2).Trim(); //JR added trim
            var re = new Regex("[\\s\\,]+");
            valuesList = re.Replace(valuesList, ",");
            var valuesStr = valuesList.Split(',');
            var len = valuesStr.GetLength(0);
            var values = new double[len];

            for (var i = 0; i < len; i++)
                //values.SetValue(SvgNumber.ParseToFloat(valuesStr[i]), i);
                values[i] = SvgNumber.ParseNumber(valuesStr[i]);

            switch (type)
            {
                case "translate":
                    switch (len)
                    {
                        case 1:
                            SetTranslate(values[0], 0);
                            break;
                        case 2:
                            SetTranslate(values[0], values[1]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in translate transform");
                    }

                    break;
                case "rotate":
                    switch (len)
                    {
                        case 1:
                            SetRotate(values[0]);
                            break;
                        case 3:
                            SetRotate(values[0], values[1], values[2]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in rotate transform");
                    }

                    break;
                case "scale":
                    switch (len)
                    {
                        case 1:
                            SetScale(values[0], values[0]);
                            break;
                        case 2:
                            SetScale(values[0], values[1]);
                            break;
                        default:
                            throw new ApplicationException("Wrong number of arguments in scale transform");
                    }

                    break;
                case "skewX":
                    if (len != 1)
                        throw new ApplicationException("Wrong number of arguments in skewX transform");
                    SetSkewX(values[0]);
                    break;
                case "skewY":
                    if (len != 1)
                        throw new ApplicationException("Wrong number of arguments in skewY transform");
                    SetSkewY(values[0]);
                    break;
                case "matrix":
                    if (len != 6)
                        throw new ApplicationException("Wrong number of arguments in matrix transform");
                    SetMatrix(
                        new SvgMatrix(
                            values[0],
                            values[1],
                            values[2],
                            values[3],
                            values[4],
                            values[5]
                        ));
                    break;
                default:
                    Type = (short)SvgTransformType.Unknown;
                    break;
            }
        }

        #endregion

        #region ISvgTransform Members

        public short Type { get; private set; }

        public ISvgMatrix Matrix { get; private set; }

        public double Angle { get; private set; }

        public void SetMatrix(ISvgMatrix matrix)
        {
            Type = (short)SvgTransformType.Matrix;
            Matrix = matrix;
        }

        public void SetTranslate(double tx, double ty)
        {
            Type = (short)SvgTransformType.Translate;
            Matrix = new SvgMatrix().Translate(tx, ty);
        }

        public void SetScale(double sx, double sy)
        {
            Type = (short)SvgTransformType.Scale;
            Matrix = new SvgMatrix().ScaleNonUniform(sx, sy);
        }

        public void SetRotate(double angle)
        {
            Type = (short)SvgTransformType.Rotate;
            Angle = angle;
            Matrix = new SvgMatrix().Rotate(angle);
        }

        public void SetRotate(double angle, double cx, double cy)
        {
            Type = (short)SvgTransformType.Rotate;
            Angle = angle;
            Matrix = new SvgMatrix().Translate(cx, cy).Rotate(angle).Translate(-cx, -cy);
        }

        public void SetSkewX(double angle)
        {
            Type = (short)SvgTransformType.SkewX;
            Angle = angle;
            Matrix = new SvgMatrix().SkewX(angle);
        }

        public void SetSkewY(double angle)
        {
            Type = (short)SvgTransformType.SkewY;
            Angle = angle;
            Matrix = new SvgMatrix().SkewY(angle);
        }

        #endregion
    }
}