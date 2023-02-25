using System;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.Fills;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfGradientFill : WpfFill
    {
        #region Private Fields

        private readonly SvgGradientElement _gradientElement;

        #endregion

        #region Constructors and Destructor

        public WpfGradientFill(SvgGradientElement gradientElement)
        {
            _gradientElement = gradientElement;
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context)
        {
            var linearGrad = _gradientElement as SvgLinearGradientElement;
            if (linearGrad != null) return GetLinearGradientBrush(elementBounds, linearGrad);

            var radialGrad = _gradientElement as SvgRadialGradientElement;
            if (radialGrad != null) return GetRadialGradientBrush(elementBounds, radialGrad);

            return new SolidColorBrush(Colors.Black);
        }

        #endregion

        #region Private Methods

        private LinearGradientBrush GetLinearGradientBrush(Rect elementBounds,
            SvgLinearGradientElement res)
        {
            var x1 = res.X1.AnimVal.Value;
            var x2 = res.X2.AnimVal.Value;
            var y1 = res.Y1.AnimVal.Value;
            var y2 = res.Y2.AnimVal.Value;

            var gradientStops = GetGradientStops(res.Stops);

            //LinearGradientBrush brush = new LinearGradientBrush(gradientStops);
            var brush = new LinearGradientBrush(gradientStops,
                new Point(x1, y1), new Point(x2, y2));

            var spreadMethod = SvgSpreadMethod.Pad;
            if (res.SpreadMethod != null)
            {
                spreadMethod = (SvgSpreadMethod)res.SpreadMethod.AnimVal;

                if (spreadMethod != SvgSpreadMethod.None) brush.SpreadMethod = WpfConvert.ToSpreadMethod(spreadMethod);
            }

            Transform viewBoxTransform = null;

            var mappingMode = SvgUnitType.ObjectBoundingBox;
            if (res.GradientUnits != null)
            {
                mappingMode = (SvgUnitType)res.GradientUnits.AnimVal;
                if (mappingMode == SvgUnitType.ObjectBoundingBox)
                {
                    brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                }
                else if (mappingMode == SvgUnitType.UserSpaceOnUse)
                {
                    brush.MappingMode = BrushMappingMode.Absolute;

                    viewBoxTransform = FitToViewbox(new SvgRect(x1, y1,
                            Math.Abs(x2 - x1), Math.Abs(y2 - y1)),
                        elementBounds);
                }
            }

            var transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity)
            {
                if (viewBoxTransform != null)
                {
                    var group = new TransformGroup();
                    group.Children.Add(viewBoxTransform);
                    group.Children.Add(transform);

                    brush.Transform = group;
                }
                else
                {
                    brush.Transform = transform;
                    //brush.StartPoint = new Point(0, 0.5);
                    //brush.EndPoint = new Point(1, 0.5);
                }

                //brush.StartPoint = new Point(0, 0);
                //brush.EndPoint = new Point(1, 1);
            }
            else
            {
                var fLeft = (float)res.X1.AnimVal.Value;
                var fRight = (float)res.X2.AnimVal.Value;
                var fTop = (float)res.Y1.AnimVal.Value;
                var fBottom = (float)res.Y2.AnimVal.Value;

                if (fTop == fBottom)
                {
                    //mode = LinearGradientMode.Horizontal;

                    //brush.StartPoint = new Point(0, 0.5);
                    //brush.EndPoint = new Point(1, 0.5);
                }
                else
                {
                    if (fLeft == fRight)
                    {
                        //mode = LinearGradientMode.Vertical;

                        //brush.StartPoint = new Point(0.5, 0);
                        //brush.EndPoint = new Point(0.5, 1);
                    }
                    else
                    {
                        if (fLeft < fRight)
                        {
                            if (viewBoxTransform != null)
                            {
                                var group = new TransformGroup();
                                group.Children.Add(viewBoxTransform);
                                group.Children.Add(new RotateTransform(45, 0.5, 0.5));

                                brush.Transform = group;
                            }
                            else
                            {
                                brush.RelativeTransform = new RotateTransform(45, 0.5, 0.5);
                            }

                            //mode = LinearGradientMode.ForwardDiagonal;
                            //brush.EndPoint = new Point(x1, y1 + 1);

                            //brush.StartPoint = new Point(0, 0);
                            //brush.EndPoint = new Point(1, 1);
                        }
                        else
                        {
                            //mode = LinearGradientMode.BackwardDiagonal;
                            if (viewBoxTransform != null)
                            {
                                var group = new TransformGroup();
                                group.Children.Add(viewBoxTransform);
                                group.Children.Add(new RotateTransform(-45, 0.5, 0.5));

                                brush.Transform = group;
                            }
                            else
                            {
                                brush.RelativeTransform = new RotateTransform(-45, 0.5, 0.5);
                            }

                            //brush.StartPoint = new Point(0, 0);
                            //brush.EndPoint = new Point(1, 1);
                        }
                    }
                }
            }

            var colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!string.IsNullOrEmpty(colorInterpolation))
            {
                if (colorInterpolation == "linearRGB")
                    brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                else
                    brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
            }

            return brush;
        }

        private RadialGradientBrush GetRadialGradientBrush(Rect elementBounds,
            SvgRadialGradientElement res)
        {
            var centerX = res.Cx.AnimVal.Value;
            var centerY = res.Cy.AnimVal.Value;
            var focusX = res.Fx.AnimVal.Value;
            var focusY = res.Fy.AnimVal.Value;
            var radius = res.R.AnimVal.Value;

            var gradientStops = GetGradientStops(res.Stops);

            var brush = new RadialGradientBrush(gradientStops);

            brush.RadiusX = radius;
            brush.RadiusY = radius;
            brush.Center = new Point(centerX, centerY);
            brush.GradientOrigin = new Point(focusX, focusY);

            if (res.SpreadMethod != null)
            {
                var spreadMethod = (SvgSpreadMethod)res.SpreadMethod.AnimVal;

                if (spreadMethod != SvgSpreadMethod.None) brush.SpreadMethod = WpfConvert.ToSpreadMethod(spreadMethod);
            }

            if (res.GradientUnits != null)
            {
                var mappingMode = (SvgUnitType)res.GradientUnits.AnimVal;
                if (mappingMode == SvgUnitType.ObjectBoundingBox)
                    brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                else if (mappingMode == SvgUnitType.UserSpaceOnUse) brush.MappingMode = BrushMappingMode.Absolute;
            }

            var transform = GetTransformMatrix(res);
            if (transform != null && !transform.Matrix.IsIdentity) brush.Transform = transform;

            var colorInterpolation = res.GetPropertyValue("color-interpolation");
            if (!string.IsNullOrEmpty(colorInterpolation))
            {
                if (colorInterpolation == "linearRGB")
                    brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
                else
                    brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
            }

            return brush;
        }

        private MatrixTransform GetTransformMatrix(SvgGradientElement gradientElement)
        {
            var svgMatrix =
                ((SvgTransformList)gradientElement.GradientTransform.AnimVal).TotalMatrix;

            var transformMatrix = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);

            return transformMatrix;
        }

        private GradientStopCollection GetGradientStops(XmlNodeList stops)
        {
            var itemCount = stops.Count;
            var gradientStops = new GradientStopCollection(itemCount);

            double lastOffset = 0;
            for (var i = 0; i < itemCount; i++)
            {
                var stop = (SvgStopElement)stops.Item(i);
                var prop = stop.GetAttribute("stop-color");
                var style = stop.GetAttribute("style");
                var color = Colors.Transparent; // no auto-inherited...
                if (!string.IsNullOrEmpty(prop) || !string.IsNullOrEmpty(style))
                {
                    var svgColor = new WpfSvgColor(stop, "stop-color");
                    color = svgColor.Color;
                }
                else
                {
                    color = Colors.Black; // the default color...
                    double alpha = 255;
                    string opacity;

                    opacity = stop.GetAttribute("stop-opacity"); // no auto-inherit
                    if (opacity == "inherit") // if explicitly defined...
                        opacity = stop.GetPropertyValue("stop-opacity");
                    if (opacity != null && opacity.Length > 0)
                        alpha *= SvgNumber.ParseNumber(opacity);

                    alpha = Math.Min(alpha, 255);
                    alpha = Math.Max(alpha, 0);

                    color = Color.FromArgb((byte)Convert.ToInt32(alpha),
                        color.R, color.G, color.B);
                }

                var offset = stop.Offset.AnimVal;

                offset /= 100;
                offset = Math.Max(lastOffset, offset);

                gradientStops.Add(new GradientStop(color, offset));
                lastOffset = offset;
            }

            if (itemCount == 0)
            {
                gradientStops.Add(new GradientStop(Colors.Black, 0));
                gradientStops.Add(new GradientStop(Colors.Black, 1));
            }

            return gradientStops;
        }

        private Transform FitToViewbox(SvgRect viewBox, Rect rectToFit)
        {
            var alignment =
                SvgPreserveAspectRatioType.XMidYMid;

            var transformArray = FitToViewBox(alignment,
                viewBox,
                new SvgRect(rectToFit.X, rectToFit.Y,
                    rectToFit.Width, rectToFit.Height));

            var translateX = transformArray[0];
            var translateY = transformArray[1];
            var scaleX = transformArray[2];
            var scaleY = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            if (translateX != 0 || translateY != 0) translateMatrix = new TranslateTransform(translateX, translateY);
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0f) scaleMatrix = new ScaleTransform(scaleX, scaleY);

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                return transformGroup;
            }

            if (translateMatrix != null)
                return translateMatrix;
            if (scaleMatrix != null) return scaleMatrix;

            return null;
        }

        private double[] FitToViewBox(SvgPreserveAspectRatioType alignment,
            SvgRect viewBox, SvgRect rectToFit)
        {
            double translateX = 0;
            double translateY = 0;
            double scaleX = 1;
            double scaleY = 1;

            if (!viewBox.IsEmpty)
            {
                // calculate scale values for non-uniform scaling
                scaleX = rectToFit.Width / viewBox.Width;
                scaleY = rectToFit.Height / viewBox.Height;

                if (alignment != SvgPreserveAspectRatioType.None)
                {
                    // uniform scaling
                    scaleX = Math.Max(scaleX, scaleY);

                    scaleY = scaleX;

                    if (alignment == SvgPreserveAspectRatioType.XMidYMax ||
                        alignment == SvgPreserveAspectRatioType.XMidYMid ||
                        alignment == SvgPreserveAspectRatioType.XMidYMin)
                        // align to the Middle X
                        translateX = rectToFit.X + rectToFit.Width / 2 - scaleX * (viewBox.X + viewBox.Width / 2);
                    else if (alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                             alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                             alignment == SvgPreserveAspectRatioType.XMaxYMin)
                        // align to the right X
                        translateX = rectToFit.Width - viewBox.Width * scaleX;

                    if (alignment == SvgPreserveAspectRatioType.XMaxYMid ||
                        alignment == SvgPreserveAspectRatioType.XMidYMid ||
                        alignment == SvgPreserveAspectRatioType.XMinYMid)
                        // align to the Middle Y
                        translateY = rectToFit.Y + rectToFit.Height / 2 - scaleY * (viewBox.Y + viewBox.Height / 2);
                    else if (alignment == SvgPreserveAspectRatioType.XMaxYMax ||
                             alignment == SvgPreserveAspectRatioType.XMidYMax ||
                             alignment == SvgPreserveAspectRatioType.XMinYMax)
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
    }
}