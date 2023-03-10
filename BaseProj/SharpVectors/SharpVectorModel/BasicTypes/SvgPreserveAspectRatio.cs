using System;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgPreserveAspectRatio : ISvgPreserveAspectRatio
    {
        #region Static members

        private static readonly Regex parCheck = new Regex("^(?<align>[A-Za-z]+)\\s*(?<meet>[A-Za-z]*)$");

        #endregion

        #region Constructors

        public SvgPreserveAspectRatio(string attr, SvgElement ownerElement)
        {
            this.ownerElement = ownerElement;

            var match = parCheck.Match(attr.Trim());
            if (match.Groups["align"].Success)
            {
                switch (match.Groups["align"].Value)
                {
                    case "none":
                        _alignment = SvgPreserveAspectRatioType.None;
                        break;
                    case "xMinYMin":
                        _alignment = SvgPreserveAspectRatioType.XMinYMin;
                        break;
                    case "xMidYMin":
                        _alignment = SvgPreserveAspectRatioType.XMidYMin;
                        break;
                    case "xMaxYMin":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMin;
                        break;
                    case "xMinYMid":
                        _alignment = SvgPreserveAspectRatioType.XMinYMid;
                        break;
                    case "xMaxYMid":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMid;
                        break;
                    case "xMinYMax":
                        _alignment = SvgPreserveAspectRatioType.XMinYMax;
                        break;
                    case "xMidYMax":
                        _alignment = SvgPreserveAspectRatioType.XMidYMax;
                        break;
                    case "xMaxYMax":
                        _alignment = SvgPreserveAspectRatioType.XMaxYMax;
                        break;
                    default:
                        _alignment = SvgPreserveAspectRatioType.XMidYMid;
                        IsDefaultAlign = true;
                        break;
                }
            }
            else
            {
                _alignment = SvgPreserveAspectRatioType.XMidYMid;
                IsDefaultAlign = true;
            }

            if (match.Groups["meet"].Success)
                switch (match.Groups["meet"].Value)
                {
                    case "slice":
                        _meetOrSlice = SvgMeetOrSlice.Slice;
                        break;
                    case "meet":
                        _meetOrSlice = SvgMeetOrSlice.Meet;
                        break;
                    case "":
                        _meetOrSlice = SvgMeetOrSlice.Meet;
                        break;
                    default:
                        _meetOrSlice = SvgMeetOrSlice.Unknown;
                        break;
                }
            else
                _meetOrSlice = SvgMeetOrSlice.Meet;
        }

        #endregion

        #region Public Properties

        public bool IsDefaultAlign { get; }

        #endregion

        #region Public Methods

        public double[] FitToViewBox(SvgRect viewBox, SvgRect rectToFit)
        {
            if (ownerElement is SvgSvgElement)
            {
                var mat = ((SvgSvgElement)ownerElement).ViewBoxTransform;
                return new[] { mat.E, mat.F, mat.A, mat.D };
            }

            double translateX = 0;
            double translateY = 0;
            double scaleX = 1;
            double scaleY = 1;

            if (!viewBox.IsEmpty)
            {
                // calculate scale values for non-uniform scaling
                scaleX = rectToFit.Width / viewBox.Width;
                scaleY = rectToFit.Height / viewBox.Height;

                if (_alignment != SvgPreserveAspectRatioType.None)
                {
                    // uniform scaling
                    if (_meetOrSlice == SvgMeetOrSlice.Meet)
                        scaleX = Math.Min(scaleX, scaleY);
                    else
                        scaleX = Math.Max(scaleX, scaleY);

                    scaleY = scaleX;

                    if (_alignment == SvgPreserveAspectRatioType.XMidYMax ||
                        _alignment == SvgPreserveAspectRatioType.XMidYMid ||
                        _alignment == SvgPreserveAspectRatioType.XMidYMin)
                        // align to the Middle X
                        translateX = rectToFit.X + rectToFit.Width / 2 - scaleX * (viewBox.X + viewBox.Width / 2);
                    else if (_alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                             _alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                             _alignment == SvgPreserveAspectRatioType.XMaxYMin)
                        // align to the right X
                        translateX = rectToFit.Width - viewBox.Width * scaleX;

                    if (_alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                        _alignment == SvgPreserveAspectRatioType.XMidYMid ||
                        _alignment == SvgPreserveAspectRatioType.XMinYMid)
                        // align to the Middle Y
                        translateY = rectToFit.Y + rectToFit.Height / 2 - scaleY * (viewBox.Y + viewBox.Height / 2);
                    else if (_alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                             _alignment == SvgPreserveAspectRatioType.XMidYMax ||
                             _alignment == SvgPreserveAspectRatioType.XMinYMax)
                        // align to the bottom Y
                        translateY = rectToFit.Height - viewBox.Height * scaleY;
                }
                else
                {
                    translateX = -viewBox.X * scaleX;
                    translateY = -viewBox.Y * scaleY;
                }
            }

            return new[]
            {
                translateX,
                translateY,
                scaleX,
                scaleY
            };
        }

        #endregion

        #region Private Fields

        private readonly SvgElement ownerElement;

        private readonly SvgMeetOrSlice _meetOrSlice;
        private readonly SvgPreserveAspectRatioType _alignment;

        #endregion

        #region ISvgPreserveAspectRatio Members

        public SvgPreserveAspectRatioType Align
        {
            get => _alignment;
            set => throw new NotImplementedException();
        }

        public SvgMeetOrSlice MeetOrSlice
        {
            get => _meetOrSlice;
            set => throw new NotImplementedException();
        }

        #endregion
    }
}