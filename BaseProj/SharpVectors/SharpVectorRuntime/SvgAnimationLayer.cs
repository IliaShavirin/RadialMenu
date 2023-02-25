using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    /// <summary>
    ///     This creates a host for visuals derived from the <see cref="Canvas" /> class.
    /// </summary>
    /// <remarks>
    ///     This class provides layout, event handling, and container support for the
    ///     child visual objects.
    /// </remarks>
    public sealed class SvgAnimationLayer : DependencyObject
    {
        #region Constructors and Destructor

        public SvgAnimationLayer(SvgDrawingCanvas drawingCanvas)
        {
            _animator = new SvgAnimator();
            _drawingCanvas = drawingCanvas;

            _displayTransform = Transform.Identity;

            _colorText = Colors.Black;
            _colorLink = Colors.Blue;
            _colorSelected = Colors.Red;
            _colorHover = (Color)ColorConverter.ConvertFromString("#ffa500");

            _visualBrushes = new Dictionary<string, Brush>(StringComparer.OrdinalIgnoreCase);

            _drawObjects = new List<Drawing>();
            _linkObjects = new List<Drawing>();

            // Create a tooltip and set its position.
            _tooltip = new ToolTip();
            _tooltip.Placement = PlacementMode.MousePoint;
            _tooltip.PlacementRectangle = new Rect(50, 0, 0, 0);
            _tooltip.HorizontalOffset = 20;
            _tooltip.VerticalOffset = 20;

            _tooltipText = new TextBlock();
            _tooltipText.Text = string.Empty;
            _tooltipText.Margin = new Thickness(6, 0, 0, 0);

            //Create BulletDecorator and set it as the tooltip content.
            var bullet = new Ellipse();
            bullet.Height = 10;
            bullet.Width = 10;
            bullet.Fill = Brushes.LightCyan;

            var decorator = new BulletDecorator();
            decorator.Bullet = bullet;
            decorator.Margin = new Thickness(0, 0, 10, 0);
            decorator.Child = _tooltipText;

            _tooltip.Content = decorator;
            _tooltip.IsOpen = false;
            _tooltip.Visibility = Visibility.Hidden;

            //Finally, set tooltip on this canvas
            _drawingCanvas.ToolTip = _tooltip;
            //_drawingCanvas.Background = Brushes.Transparent;
        }

        #endregion

        #region Public Properties

        public Transform DisplayTransform
        {
            get => _displayTransform;
            set
            {
                if (value == null)
                    _displayTransform = new MatrixTransform(Matrix.Identity);
                else
                    _displayTransform = value;
            }
        }

        #endregion

        #region Private Fields

        private Rect _bounds;

        private readonly Color _colorText;
        private readonly Color _colorLink;
        private readonly Color _colorSelected;
        private readonly Color _colorHover;

        private readonly ToolTip _tooltip;
        private readonly TextBlock _tooltipText;

        private Transform _displayTransform;

        private DrawingGroup _wholeDrawing;
        private DrawingGroup _linksDrawing;

        // Create a collection of child visual objects.
        private readonly List<Drawing> _linkObjects;
        private readonly List<Drawing> _drawObjects;

        private Drawing _hitVisual;
        private Drawing _selectedVisual;

        private readonly SvgAnimator _animator;

        private readonly SvgDrawingCanvas _drawingCanvas;

        private readonly Dictionary<string, Brush> _visualBrushes;

        #endregion

        #region Public Methods

        public void LoadDiagrams(DrawingGroup linkGroups, DrawingGroup wholeGroup)
        {
            if (linkGroups == null) return;
            var drawings = linkGroups.Children;
            if (drawings == null || drawings.Count == 0) return;

            if (drawings.Count == 1)
            {
                var layerGroup = drawings[0] as DrawingGroup;
                if (layerGroup != null)
                {
                    var elementId = SvgObject.GetId(layerGroup);
                    if (!string.IsNullOrEmpty(elementId) &&
                        string.Equals(elementId, "IndicateLayer", StringComparison.OrdinalIgnoreCase))
                    {
                        LoadLayerDiagrams(layerGroup);

                        _wholeDrawing = wholeGroup;
                        _linksDrawing = linkGroups;

                        return;
                    }
                }
            }

            UnloadDiagrams();

            for (var i = 0; i < drawings.Count; i++)
            {
                var childGroup = drawings[i] as DrawingGroup;
                if (childGroup != null)
                {
                    var groupName = SvgLink.GetKey(childGroup);
                    //string groupName = childGroup.GetValue(FrameworkElement.NameProperty) as string;
                    if (string.IsNullOrEmpty(groupName))
                    {
                        if (childGroup.Children != null && childGroup.Children.Count == 1)
                            AddDrawing(childGroup);
                        else
                            throw new InvalidOperationException("Error: The link group is in error.");
                    }
                    else
                    {
                        if (childGroup.Children != null && childGroup.Children.Count == 1)
                            AddDrawing(childGroup, groupName);
                        else
                            throw new InvalidOperationException(
                                string.Format("Error: The link group is in error - {0}", groupName));
                    }
                }
            }

            _wholeDrawing = wholeGroup;
            _linksDrawing = linkGroups;

            if (_drawingCanvas != null) _displayTransform = _drawingCanvas.DisplayTransform;
        }

        public void UnloadDiagrams()
        {
            _displayTransform = Transform.Identity;

            _bounds = new Rect(0, 0, 1, 1);

            //_brushIndex = 0;

            if (_animator != null) _animator.Stop();

            ClearVisuals();

            if (_tooltip != null)
            {
                _tooltip.IsOpen = false;
                _tooltip.Visibility = Visibility.Hidden;
            }

            //this.RenderTransform = new TranslateTransform(0, 0);

            _wholeDrawing = null;
            _linksDrawing = null;
        }

        #endregion

        #region Public Mouse Methods

        public bool HandleMouseMove(MouseEventArgs e)
        {
            // Retrieve the coordinates of the mouse button event.
            var pt = e.GetPosition(_drawingCanvas);

            var hitVisual = HitTest(pt);

            if (_selectedVisual != null && hitVisual == _selectedVisual)
            {
                _drawingCanvas.Cursor = Cursors.Hand;

                return true;
            }

            string itemName = null;

            if (hitVisual == null)
            {
                if (_hitVisual != null)
                {
                    itemName = _hitVisual.GetValue(FrameworkElement.NameProperty) as string;
                    if (itemName == null)
                    {
                        _hitVisual = null;
                        return false;
                    }

                    if (_visualBrushes.ContainsKey(itemName) && _hitVisual != _selectedVisual)
                    {
                        var brush = _visualBrushes[itemName] as SolidColorBrush;
                        if (brush != null) brush.Color = _colorLink;
                    }

                    _hitVisual = null;
                }

                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return false;
            }

            _drawingCanvas.Cursor = Cursors.Hand;

            if (hitVisual == _hitVisual) return false;

            if (_hitVisual != null)
            {
                itemName = _hitVisual.GetValue(FrameworkElement.NameProperty) as string;
                if (itemName == null)
                {
                    _hitVisual = null;
                    return false;
                }

                if (_visualBrushes.ContainsKey(itemName) && _hitVisual != _selectedVisual)
                {
                    var brush = _visualBrushes[itemName] as SolidColorBrush;
                    if (brush != null) brush.Color = _colorLink;
                }

                _hitVisual = null;
            }

            itemName = hitVisual.GetValue(FrameworkElement.NameProperty) as string;
            if (itemName == null) return false;
            if (_visualBrushes.ContainsKey(itemName))
            {
                var brush = _visualBrushes[itemName] as SolidColorBrush;
                if (brush != null) brush.Color = _colorHover;
            }

            _hitVisual = hitVisual;

            var tooltipText = itemName;
            var linkAction = SvgLink.GetAction(hitVisual);
            if (linkAction == SvgLinkAction.LinkTooltip &&
                _tooltip != null && !string.IsNullOrEmpty(tooltipText))
            {
                var rectBounds = hitVisual.Bounds;

                _tooltip.PlacementRectangle = rectBounds;

                _tooltipText.Text = tooltipText;

                if (_tooltip.Visibility == Visibility.Hidden) _tooltip.Visibility = Visibility.Visible;

                _tooltip.IsOpen = true;
            }
            else
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }
            }

            return true;
        }

        public bool HandleMouseDown(MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(_drawingCanvas);

            var visual = HitTest(pt);
            if (visual == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return false;
            }

            if (_selectedVisual != null && visual == _selectedVisual)
            {
                _drawingCanvas.Cursor = Cursors.Hand;

                return true;
            }

            var itemName = visual.GetValue(FrameworkElement.NameProperty) as string;
            if (itemName == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return false;
            }

            SolidColorBrush brush = null;
            if (_visualBrushes.ContainsKey(itemName))
            {
                brush = _visualBrushes[itemName] as SolidColorBrush;

                if (brush != null) brush.Color = _colorSelected;
            }

            if (brush == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return false;
            }

            if (_selectedVisual != null)
            {
                itemName = _selectedVisual.GetValue(FrameworkElement.NameProperty) as string;
                if (itemName == null) return false;
                if (_visualBrushes.ContainsKey(itemName))
                {
                    brush = _visualBrushes[itemName] as SolidColorBrush;

                    if (brush != null) brush.Color = _colorLink;
                }
                else
                {
                    return false;
                }
            }

            _selectedVisual = visual;

            if (e.ChangedButton == MouseButton.Left)
            {
                var brushName = brush.GetValue(FrameworkElement.NameProperty) as string;
                if (!string.IsNullOrEmpty(brushName))
                {
                    var linkAction = SvgLink.GetAction(visual);
                    if (linkAction == SvgLinkAction.LinkHtml ||
                        linkAction == SvgLinkAction.LinkPage)
                        _animator.Start(brushName, brush);
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                _animator.Stop();
            }

            return true;
        }

        public bool HandleMouseLeave(MouseEventArgs e)
        {
            if (_tooltip != null)
            {
                _tooltip.IsOpen = false;
                _tooltip.Visibility = Visibility.Hidden;
            }

            if (_hitVisual == null) return false;

            var itemName = _hitVisual.GetValue(FrameworkElement.NameProperty) as string;
            if (itemName == null)
            {
                _hitVisual = null;
                return false;
            }

            if (_visualBrushes.ContainsKey(itemName) && _hitVisual != _selectedVisual)
            {
                var brush = _visualBrushes[itemName] as SolidColorBrush;
                if (brush != null) brush.Color = _colorLink;
            }

            _hitVisual = null;

            return true;
        }

        #endregion

        #region Private Methods

        private void ClearVisuals()
        {
            if (_drawObjects != null && _drawObjects.Count != 0) _drawObjects.Clear();

            if (_linkObjects != null && _linkObjects.Count != 0) _linkObjects.Clear();
        }

        private Drawing HitTest(Point pt)
        {
            var ptDisplay = _displayTransform.Transform(pt);

            for (var i = 0; i < _linkObjects.Count; i++)
            {
                var drawing = _linkObjects[i];
                if (drawing.Bounds.Contains(ptDisplay)) return drawing;
            }

            return null;
        }

        private void AddDrawing(DrawingGroup group)
        {
        }

        private void AddDrawing(DrawingGroup group, string name)
        {
        }

        private void AddTextDrawing(DrawingGroup group, string name)
        {
            var isHyperlink = false;
            if (name == "x11201_1_") isHyperlink = true;

            SolidColorBrush textBrush = null;
            if (name.StartsWith("XMLID_", StringComparison.OrdinalIgnoreCase))
            {
                textBrush = new SolidColorBrush(_colorText);
            }
            else
            {
                isHyperlink = true;
                textBrush = new SolidColorBrush(_colorLink);
            }

            var brushName = name + "_Brush";
            textBrush.SetValue(FrameworkElement.NameProperty, brushName);

            var drawings = group.Children;
            var itemCount = drawings != null ? drawings.Count : 0;
            for (var i = 0; i < itemCount; i++)
            {
                var drawing = drawings[i] as DrawingGroup;
                if (drawing != null)
                    for (var j = 0; j < drawing.Children.Count; j++)
                    {
                        var glyphDrawing = drawing.Children[j] as GlyphRunDrawing;
                        if (glyphDrawing != null) glyphDrawing.ForegroundBrush = textBrush;
                    }
            }

            if (isHyperlink)
            {
                _visualBrushes[name] = textBrush;

                _linkObjects.Add(group);
            }
        }

        private void AddGeometryDrawing(GeometryDrawing drawing)
        {
        }

        private void AddGeometryDrawing(GeometryDrawing drawing, string name)
        {
        }

        private static bool IsValidBounds(Rect rectBounds)
        {
            if (rectBounds.IsEmpty || double.IsNaN(rectBounds.Width) || double.IsNaN(rectBounds.Height)
                || double.IsInfinity(rectBounds.Width) || double.IsInfinity(rectBounds.Height))
                return false;

            return true;
        }

        private static bool TryCast<TBase, TDerived>(TBase baseObj, out TDerived derivedObj)
            where TDerived : class, TBase
        {
            return (derivedObj = baseObj as TDerived) != null;
        }

        private void LoadLayerDiagrams(DrawingGroup layerGroup)
        {
            UnloadDiagrams();

            var drawings = layerGroup.Children;

            DrawingGroup drawGroup = null;
            GeometryDrawing drawGeometry = null;
            for (var i = 0; i < drawings.Count; i++)
            {
                var drawing = drawings[i];
                if (TryCast(drawing, out drawGroup))
                {
                    var groupName = SvgLink.GetKey(drawGroup);
                    if (string.IsNullOrEmpty(groupName)) groupName = SvgObject.GetId(drawGroup);
                    //string groupName = childGroup.GetValue(FrameworkElement.NameProperty) as string;
                    if (string.IsNullOrEmpty(groupName))
                    {
                        LoadLayerGroup(drawGroup);
                    }
                    else
                    {
                        var elementType = SvgObject.GetType(drawGroup);

                        if (elementType == SvgObjectType.Text)
                        {
                            AddTextDrawing(drawGroup, groupName);
                        }
                        else
                        {
                            if (drawGroup.Children != null && drawGroup.Children.Count == 1)
                                AddDrawing(drawGroup, groupName);
                            //throw new InvalidOperationException(
                            //    String.Format("Error: The link group is in error - {0}", groupName));
                        }
                    }
                }
                else if (TryCast(drawing, out drawGeometry))
                {
                    AddGeometryDrawing(drawGeometry);
                }
            }

            if (_drawingCanvas != null) _displayTransform = _drawingCanvas.DisplayTransform;
        }

        private void LoadLayerGroup(DrawingGroup group)
        {
            var drawings = group.Children;

            DrawingGroup drawGroup = null;
            GeometryDrawing drawGeometry = null;
            for (var i = 0; i < drawings.Count; i++)
            {
                var drawing = drawings[i];
                if (TryCast(drawing, out drawGroup))
                {
                    var groupName = SvgLink.GetKey(drawGroup);
                    if (string.IsNullOrEmpty(groupName)) groupName = SvgObject.GetId(drawGroup);
                    //string groupName = childGroup.GetValue(FrameworkElement.NameProperty) as string;
                    if (string.IsNullOrEmpty(groupName))
                    {
                        LoadLayerGroup(drawGroup);
                    }
                    else
                    {
                        var elementType = SvgObject.GetType(drawGroup);

                        if (elementType == SvgObjectType.Text)
                        {
                            AddTextDrawing(drawGroup, groupName);
                        }
                        else
                        {
                            if (drawGroup.Children != null && drawGroup.Children.Count == 1)
                                AddDrawing(drawGroup, groupName);
                            //throw new InvalidOperationException(
                            //    String.Format("Error: The link group is in error - {0}", groupName));
                        }
                    }
                }
                else if (TryCast(drawing, out drawGeometry))
                {
                    AddGeometryDrawing(drawGeometry);
                }
            }
        }

        #endregion
    }
}