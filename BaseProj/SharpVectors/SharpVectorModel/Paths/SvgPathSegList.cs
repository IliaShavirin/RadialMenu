using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    // TODO: should we check that the list starts with a M/m since that's required by the spec?
    public class SvgPathSegList : ISvgPathSegList
    {
        #region Constructors

        public SvgPathSegList(string d, bool readOnly)
        {
            if (d == null) d = string.Empty;

            PathScript = d;

            if (!string.IsNullOrEmpty(d)) ParseString(d);

            this.readOnly = readOnly;
        }

        #endregion

        #region Public Properties

        public string PathScript { get; }

        #endregion

        #region Private Fields

        private readonly bool readOnly;
        private readonly List<ISvgPathSeg> segments = new List<ISvgPathSeg>();

        private static readonly Regex rePathCmd = new Regex(@"(?=[A-DF-Za-df-z])");

        private static readonly Regex coordSplit =
            new Regex(@"(\s*,\s*)|(\s+)|((?<=[0-9])(?=-))", RegexOptions.ExplicitCapture);

        #endregion

        #region Private Methods

        private void ParseString(string d)
        {
            ISvgPathSeg seg;
            var segs = rePathCmd.Split(d);

            foreach (var s in segs)
            {
                var segment = s.Trim();
                if (segment.Length > 0)
                {
                    var cmd = segment.ToCharArray(0, 1)[0];
                    var coords = getCoords(segment);
                    var length = coords.Length;
                    switch (cmd)
                    {
                        #region moveto

                        case 'M':
                            for (var i = 0; i < length; i += 2)
                            {
                                if (i == 0)
                                    seg = new SvgPathSegMovetoAbs(coords[i], coords[i + 1]);
                                else
                                    seg = new SvgPathSegLinetoAbs(coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'm':
                            for (var i = 0; i < length; i += 2)
                            {
                                if (i == 0)
                                    seg = new SvgPathSegMovetoRel(coords[i], coords[i + 1]);
                                else
                                    seg = new SvgPathSegLinetoRel(coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;

                        #endregion

                        #region lineto

                        case 'L':
                            for (var i = 0; i < length; i += 2)
                            {
                                seg = new SvgPathSegLinetoAbs(coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'l':
                            for (var i = 0; i < length; i += 2)
                            {
                                seg = new SvgPathSegLinetoRel(coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'H':
                            for (var i = 0; i < length; i++)
                            {
                                seg = new SvgPathSegLinetoHorizontalAbs(coords[i]);
                                AppendItem(seg);
                            }

                            break;
                        case 'h':
                            for (var i = 0; i < length; i++)
                            {
                                seg = new SvgPathSegLinetoHorizontalRel(coords[i]);
                                AppendItem(seg);
                            }

                            break;
                        case 'V':
                            for (var i = 0; i < length; i++)
                            {
                                seg = new SvgPathSegLinetoVerticalAbs(coords[i]);
                                AppendItem(seg);
                            }

                            break;
                        case 'v':
                            for (var i = 0; i < length; i++)
                            {
                                seg = new SvgPathSegLinetoVerticalRel(coords[i]);
                                AppendItem(seg);
                            }

                            break;

                        #endregion

                        #region beziers

                        case 'C':
                            for (var i = 0; i < length; i += 6)
                            {
                                seg = new SvgPathSegCurvetoCubicAbs(
                                    coords[i + 4],
                                    coords[i + 5],
                                    coords[i],
                                    coords[i + 1],
                                    coords[i + 2],
                                    coords[i + 3]);
                                AppendItem(seg);
                            }

                            break;
                        case 'c':
                            for (var i = 0; i < length; i += 6)
                            {
                                seg = new SvgPathSegCurvetoCubicRel(
                                    coords[i + 4],
                                    coords[i + 5],
                                    coords[i],
                                    coords[i + 1],
                                    coords[i + 2],
                                    coords[i + 3]);

                                AppendItem(seg);
                            }

                            break;
                        case 'S':
                            for (var i = 0; i < length; i += 4)
                            {
                                seg = new SvgPathSegCurvetoCubicSmoothAbs(
                                    coords[i + 2],
                                    coords[i + 3],
                                    coords[i],
                                    coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 's':
                            for (var i = 0; i < length; i += 4)
                            {
                                seg = new SvgPathSegCurvetoCubicSmoothRel(
                                    coords[i + 2],
                                    coords[i + 3],
                                    coords[i],
                                    coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'Q':
                            for (var i = 0; i < length; i += 4)
                            {
                                seg = new SvgPathSegCurvetoQuadraticAbs(
                                    coords[i + 2],
                                    coords[i + 3],
                                    coords[i],
                                    coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'q':
                            for (var i = 0; i < length; i += 4)
                            {
                                seg = new SvgPathSegCurvetoQuadraticRel(
                                    coords[i + 2],
                                    coords[i + 3],
                                    coords[i],
                                    coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 'T':
                            for (var i = 0; i < length; i += 2)
                            {
                                seg = new SvgPathSegCurvetoQuadraticSmoothAbs(
                                    coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;
                        case 't':
                            for (var i = 0; i < length; i += 2)
                            {
                                seg = new SvgPathSegCurvetoQuadraticSmoothRel(
                                    coords[i], coords[i + 1]);
                                AppendItem(seg);
                            }

                            break;

                        #endregion

                        #region arcs

                        case 'A':
                        case 'a':
                            for (var i = 0; i < length; i += 7)
                            {
                                if (cmd == 'A')
                                    seg = new SvgPathSegArcAbs(
                                        coords[i + 5],
                                        coords[i + 6],
                                        coords[i],
                                        coords[i + 1],
                                        coords[i + 2],
                                        coords[i + 3] != 0,
                                        coords[i + 4] != 0);
                                else
                                    seg = new SvgPathSegArcRel(
                                        coords[i + 5],
                                        coords[i + 6],
                                        coords[i],
                                        coords[i + 1],
                                        coords[i + 2],
                                        coords[i + 3] != 0,
                                        coords[i + 4] != 0);
                                AppendItem(seg);
                            }

                            break;

                        #endregion

                        #region close

                        case 'z':
                        case 'Z':
                            seg = new SvgPathSegClosePath();
                            AppendItem(seg);
                            break;

                        #endregion

                        #region Unknown path command

                        default:
                            throw new ApplicationException(string.Format("Unknown path command - ({0})", cmd));

                        #endregion
                    }
                }
            }
        }

        private double[] getCoords(string segment)
        {
            var coords = new double[0];

            segment = segment.Substring(1);
            segment = segment.Trim();
            segment = segment.Trim(',');

            if (segment.Length > 0)
            {
                var sCoords = coordSplit.Split(segment);

                coords = new double[sCoords.Length];
                for (var i = 0; i < sCoords.Length; i++) coords[i] = SvgNumber.ParseNumber(sCoords[i]);
            }

            return coords;
        }

        private void setListAndIndex(SvgPathSeg newItem, int index)
        {
            if (newItem != null)
            {
                newItem.SetList(this);
                newItem.SetIndex(index);
            }
            else
            {
                throw new SvgException(SvgExceptionType.SvgWrongTypeErr,
                    "Can only add SvgPathSeg subclasses to ISvgPathSegList");
            }
        }

        private void changeIndexes(int startAt, int diff)
        {
            var count = segments.Count;
            for (var i = startAt; i < count; i++)
            {
                var seg = segments[i] as SvgPathSeg;
                if (seg != null) seg.SetIndexWithDiff(diff);
            }
        }

        #endregion

        #region ISvgPathSegList Members

        public int NumberOfItems => segments.Count;

        public void Clear()
        {
            if (readOnly)
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            segments.Clear();
        }

        public ISvgPathSeg Initialize(ISvgPathSeg newItem)
        {
            Clear();

            return AppendItem(newItem);
        }

        public ISvgPathSeg GetItem(int index)
        {
            if (index < 0 || index >= NumberOfItems) throw new DomException(DomExceptionType.IndexSizeErr);

            return segments[index];
        }

        public ISvgPathSeg this[int index]
        {
            get => GetItem(index);
            set => ReplaceItem(value, index);
        }

        public ISvgPathSeg InsertItemBefore(ISvgPathSeg newItem, int index)
        {
            if (readOnly) throw new DomException(DomExceptionType.NoModificationAllowedErr);

            segments.Insert(index, newItem);
            setListAndIndex(newItem as SvgPathSeg, index);
            changeIndexes(index + 1, 1);

            return newItem;
        }

        public ISvgPathSeg ReplaceItem(ISvgPathSeg newItem, int index)
        {
            if (readOnly) throw new DomException(DomExceptionType.NoModificationAllowedErr);

            var replacedItem = GetItem(index);
            segments[index] = newItem;
            setListAndIndex(newItem as SvgPathSeg, index);

            return replacedItem;
        }

        public ISvgPathSeg RemoveItem(int index)
        {
            if (readOnly) throw new DomException(DomExceptionType.NoModificationAllowedErr);

            var result = GetItem(index);
            segments.RemoveAt(index);
            changeIndexes(index, -1);

            return result;
        }

        public ISvgPathSeg AppendItem(ISvgPathSeg newItem)
        {
            if (readOnly) throw new DomException(DomExceptionType.NoModificationAllowedErr);

            segments.Add(newItem);
            setListAndIndex(newItem as SvgPathSeg, segments.Count - 1);

            return newItem;
        }

        #endregion

        #region Public members

        public SvgPointF[] Points
        {
            get
            {
                var ret = new List<SvgPointF>();
                foreach (SvgPathSeg seg in segments) ret.Add(seg.AbsXY);

                return ret.ToArray();
            }
        }

        internal SvgPathSeg GetPreviousSegment(SvgPathSeg seg)
        {
            var index = segments.IndexOf(seg);
            if (index == -1)
                throw new Exception("Path segment not part of this list");
            if (index == 0)
                return null;
            return (SvgPathSeg)GetItem(index - 1);
        }

        internal SvgPathSeg GetNextSegment(SvgPathSeg seg)
        {
            var index = segments.IndexOf(seg);
            if (index == -1)
                throw new Exception("Path segment not part of this list");
            if (index == segments.Count - 1)
                return null;
            return (SvgPathSeg)this[index + 1];
        }

        public double GetStartAngle(int index)
        {
            return ((SvgPathSeg)this[index]).StartAngle;
        }

        public double GetEndAngle(int index)
        {
            return ((SvgPathSeg)this[index]).EndAngle;
        }

        public string PathText
        {
            get
            {
                var sb = new StringBuilder();
                foreach (SvgPathSeg seg in segments) sb.Append(seg.PathText);
                return sb.ToString();
            }
        }

        internal double GetTotalLength()
        {
            double result = 0;
            foreach (SvgPathSeg segment in segments) result += segment.Length;
            return result;
        }

        internal int GetPathSegAtLength(double distance)
        {
            double result = 0;
            foreach (SvgPathSeg segment in segments)
            {
                result += segment.Length;
                if (result > distance) return segment.Index;
            }

            // distance was to big, return last item index
            // TODO: is this correct?
            return NumberOfItems - 1;
        }

        #endregion
    }
}