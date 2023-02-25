using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.ExtensionMethods
{
    public static class DrawingExtensions
    {
        private static SolidColorBrush ChangeOpacityBrush(SolidColorBrush brush, double opacityCoeficient)
        {
            var resultBrush = new SolidColorBrush(Color.FromArgb((byte)(byte.MaxValue * opacityCoeficient),
                brush.Color.R, brush.Color.G, brush.Color.B));
            return resultBrush;
        }

        public static void ChangeOpacity(this Drawing drawing, double opacityCoeficient)
        {
            if (drawing is DrawingGroup dg)
            {
                dg.Opacity = opacityCoeficient;
                //foreach (var drawingItems in (drawing as DrawingGroup).Children)
                //{
                //    ChangeOpacity(drawingItems, opacityCoeficient);
                //}
            }
            else if (drawing is GeometryDrawing)
            {
                var geo = drawing as GeometryDrawing;

                if (geo.Brush != null && geo.Brush is SolidColorBrush)
                    geo.Brush = ChangeOpacityBrush(geo.Brush as SolidColorBrush, opacityCoeficient);
                if (geo.Pen != null && geo.Pen?.Brush?.Opacity != 0 && geo.Pen.Brush is SolidColorBrush)
                    geo.Pen.Brush = ChangeOpacityBrush(geo.Pen.Brush as SolidColorBrush, opacityCoeficient);
            }
        }

        public static void ChangeColor(this Drawing drawing, Brush brush, bool isColorShiftFill = true,
            bool isColorShiftStroke = true, bool isColorShiftInvisible = true)
        {
            if (brush == null)
                return;

            if (drawing is DrawingGroup)
            {
                ChangeColorForDrawingGroup(drawing as DrawingGroup, brush, isColorShiftFill, isColorShiftStroke,
                    isColorShiftInvisible);
            }

            else if (drawing is GeometryDrawing)
            {
                var geo = drawing as GeometryDrawing;
                var brushAsSolid = geo.Brush as SolidColorBrush;

                if (isColorShiftFill && (isColorShiftInvisible || (geo.Brush != null && geo.Brush.Opacity != 0 &&
                                                                   (brushAsSolid == null ||
                                                                    brushAsSolid.Color.A != 0))))
                {
                    var opacity = brush.Opacity;
                    var brushClone = brush.CloneCurrentValue();
                    brushClone.Opacity = opacity;
                    brushClone.TryFreeze();
                    geo.Brush = brushClone;
                }

                if (isColorShiftStroke && (isColorShiftInvisible || (geo.Pen != null && geo.Pen?.Brush?.Opacity != 0)))
                {
                    if (geo.Pen == null)
                    {
                        geo.Pen = new Pen(brush, 1);
                    }
                    else
                    {
                        var penClone = geo.Pen.CloneCurrentValue();
                        var brushClone = brush.CloneCurrentValue();
                        brushClone.TryFreeze();
                        penClone.Brush = brushClone;
                        penClone.TryFreeze();
                        geo.Pen = penClone;
                    }
                }
            }
            else if (drawing is GlyphRunDrawing)
            {
                ((GlyphRunDrawing)drawing).ForegroundBrush = brush;
            }
        }

        public static void ChangeColor(this Drawing drawing, Color color, double? colorOpacity = null,
            bool isColorShiftFill = true, bool isColorShiftStroke = true, bool isColorShiftInvisible = true)
        {
            var brush = new SolidColorBrush(color);

            if (colorOpacity.HasValue)
                brush.Opacity = colorOpacity.Value;

            ChangeColor(drawing, brush, isColorShiftFill, isColorShiftStroke, isColorShiftInvisible);
        }

        private static void ChangeColorForDrawingGroup(DrawingGroup group, Brush brush, bool isColorShiftFill = true,
            bool isColorShiftStroke = true, bool isColorShiftInvisible = true)
        {
            foreach (var drawing in group.Children)
                ChangeColor(drawing, brush, isColorShiftFill, isColorShiftStroke, isColorShiftInvisible);
        }

        public static Brush GetAnyBrushFromGroupByName(this DrawingImage drawingImage, string name)
        {
            return drawingImage.GetItemByName(name).GetAnyBrushFroumGroup();
        }

        public static Brush GetAnyBrushFroumGroup(this Drawing drawing)
        {
            return GetAnyBrushFroumGroupByNameRecursive(drawing);
        }

        private static Brush GetAnyBrushFroumGroupByNameRecursive(Drawing drawing)
        {
            if (drawing is DrawingGroup drawingGroup)
            {
                foreach (var drawingGroupChild in drawingGroup.Children)
                {
                    var brush = GetAnyBrushFroumGroupByNameRecursive(drawingGroupChild);
                    if (brush != null) return brush;
                }
            }
            else if (drawing is GeometryDrawing)
            {
                var geo = drawing as GeometryDrawing;

                if (geo.Brush != null) return geo.Brush;

                if (geo.Pen?.Brush != null) return geo.Pen.Brush;
            }
            else if (drawing is GlyphRunDrawing)
            {
                return ((GlyphRunDrawing)drawing).ForegroundBrush;
            }

            return null;
        }

        public static void SetItemByName(this DrawingImage drawingImage, Drawing setItem, string name)
        {
            drawingImage.Drawing.SetItemByName(setItem, name);
        }

        public static void SetItemByName(this Drawing drawing, Drawing setItem, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var drawingGroup = drawing as DrawingGroup;

            if (drawingGroup != null) SetItemByNameRecursive(drawingGroup, setItem, name);
        }

        private static void SetItemByNameRecursive(DrawingGroup drawingGroup, Drawing setItem, string name)
        {
            Drawing foundItem;
            DrawingGroup foundItemParent;
            foreach (var child in drawingGroup.Children)
            {
                if (SvgObject.GetId(child) == name)
                {
                    drawingGroup.Children.Remove(child);
                    drawingGroup.Children.Add(setItem);
                    break;
                }

                if (child is DrawingGroup)
                {
                    foundItem = GetItemByNameRecursive((DrawingGroup)child, name, out foundItemParent);
                    if (foundItem != null)
                    {
                        foundItemParent.Children.Remove(foundItem);
                        foundItemParent.Children.Add(setItem);
                        break;
                    }
                }
            }
        }

        public static void SetOpacityForGroupByName(this DrawingImage drawingImage, string name, double opacity)
        {
            drawingImage.Drawing.SetOpacityForGroupByName(name, opacity);
        }

        public static void SetOpacityForGroupByName(this Drawing drawing, string name, double opacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var drawingGroup = drawing as DrawingGroup;

            if (drawingGroup != null) SetOpacityForGroupByNameRecursive(drawingGroup, name, opacity);
        }

        public static void SetOpacityForGroupByNameRecursive(DrawingGroup drawingGroup, string name, double opacity)
        {
            foreach (var child in drawingGroup.Children)
                if (child is DrawingGroup dg)
                {
                    if (SvgObject.GetId(dg) == name)
                    {
                        dg.Opacity = opacity;
                        break;
                    }

                    dg.SetOpacityForGroupByName(name, opacity);
                }
        }

        public static Drawing CloneWithChildren(this Drawing drawing)
        {
            if (drawing == null)
                return null;
            //if (drawing is DrawingGroup)
            //{
            //    CloneChildren((DrawingGroup)drawing);
            //}

            return drawing.Clone();
        }


        #region GetItemByName

        public static Drawing GetItemByName(this DrawingImage drawingImage, string name)
        {
            return drawingImage?.Drawing?.GetItemByName(name);
        }

        public static Drawing GetItemByName(this Drawing drawing, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var drawingGroup = drawing as DrawingGroup;

            if (drawingGroup != null) return GetItemByNameRecursive(drawingGroup, name);

            return null;
        }

        private static Drawing GetItemByNameRecursive(DrawingGroup drawingGroup, string name)
        {
            DrawingGroup fakeParent;
            return GetItemByNameRecursive(drawingGroup, name, out fakeParent);
        }

        private static Drawing GetItemByNameRecursive(DrawingGroup drawingGroup, string name,
            out DrawingGroup foundItemParent)
        {
            Drawing foundItem;
            if (drawingGroup != null)
                foreach (var child in drawingGroup.Children)
                {
                    var id = SvgObject.GetId(child);

                    //Tracer.TraceWrite(id);
                    if (
                        id == name ||
                        id == name + "_1" ||
                        id == name + "_2" ||
                        id == name + "_3" ||
                        id == name + "_4" ||
                        id == name + "_5" ||
                        id == name + "_6" ||
                        id == name + "_7" ||
                        id == name + "_8" ||
                        id == name + "_9"
                    )
                    {
                        foundItemParent = drawingGroup;

                        // This check is supposed to check if item parent is visible and not return if it's parent is "hidden" It should return items which
                        // "visibility" is resolved using brush or pen alpha
                        if (drawingGroup.Opacity > 0) return child;
                    }

                    if (child is DrawingGroup)
                    {
                        foundItem = GetItemByNameRecursive((DrawingGroup)child, name, out foundItemParent);
                        if (foundItem != null) return foundItem;
                    }
                }

            foundItemParent = null;
            return null;
        }

        #endregion // End - GetItemByName

        #region GetFirstNamedItemName

        public static string GetFirstNamedItemName(this DrawingImage drawingImage)
        {
            return drawingImage.Drawing.GetFirstNamedItemName();
        }

        public static string GetFirstNamedItemName(this Drawing drawing)
        {
            var drawingGroup = drawing as DrawingGroup;

            if (drawingGroup != null) return GetFirstNamedItemNameRecursive(drawingGroup);

            return null;
        }

        private static string GetFirstNamedItemNameRecursive(DrawingGroup drawingGroup)
        {
            DrawingGroup fakeParent;
            return GetItemByGetItemByNameRecursiveNameRecursive(drawingGroup, out fakeParent);
        }

        private static string GetItemByGetItemByNameRecursiveNameRecursive(DrawingGroup drawingGroup,
            out DrawingGroup foundItemParent)
        {
            var name = SvgObject.GetId(drawingGroup);
            if (!string.IsNullOrWhiteSpace(name))
            {
                foundItemParent = null;
                return name;
            }

            foreach (var child in drawingGroup.Children)
            {
                name = SvgObject.GetId(child);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    foundItemParent = drawingGroup;
                    return name;
                }

                if (child is DrawingGroup)
                {
                    var foundItemName =
                        GetItemByGetItemByNameRecursiveNameRecursive((DrawingGroup)child, out foundItemParent);
                    if (foundItemName != null) return foundItemName;
                }
            }

            foundItemParent = null;
            return null;
        }

        #endregion // End - GetFirstNamedItemName

        /*
        private static void CloneChildren(DrawingGroup group)
        {
            var clonedChildren = group.Children.Select(child => child.Clone()).ToList();
            foreach (var clonedChild in clonedChildren)
            {
                if (clonedChild is DrawingGroup)
                {
                    CloneChildren((DrawingGroup)clonedChild);
                }
            }
            group.Children.Clear();
            group.Children = new DrawingCollection(clonedChildren);
        }*/
    }
}