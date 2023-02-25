using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Utils;
using IoPath = System.IO.Path;

namespace BaseProj.SharpVectors.SharpVectorRuntime
{
    /// <summary>
    ///     This is the main drawing canvas for the wiring diagrams.
    /// </summary>
    public class SvgDrawingCanvas : Canvas
    {
        #region Protected Properties

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get
            {
                if (_hostVisual != null) return 1;

                return 0;
            }
        }

        #endregion

        #region Private Fields

        private readonly bool _drawForInteractivity;

        private double _offsetX;
        private double _offsetY;

        private readonly ToolTip _tooltip;
        private readonly TextBlock _tooltipText;

        private DrawingGroup _wholeDrawing;
        private DrawingGroup _linksDrawing;
        private DrawingGroup _mainDrawing;

        private Drawing _hitVisual;

        private DrawingVisual _hostVisual;

        // Create a collection of child visual objects.
        private readonly List<Drawing> _drawObjects;

        // Create a collection of child visual link objects.
        private readonly List<Drawing> _linkObjects;

        #endregion

        #region Constructors and Destructor

        static SvgDrawingCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgDrawingCanvas),
                new FrameworkPropertyMetadata(typeof(SvgDrawingCanvas)));
        }

        public SvgDrawingCanvas()
        {
            _drawForInteractivity = true;

            _drawObjects = new List<Drawing>();
            _linkObjects = new List<Drawing>();

            DisplayTransform = Transform.Identity;

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
            ToolTip = _tooltip;
            Background = Brushes.Transparent;

            AnimationCanvas = new SvgAnimationLayer(this);
        }

        #endregion

        #region Public Properties

        public Rect Bounds { get; private set; }

        public SvgAnimationLayer AnimationCanvas { get; }

        public Transform DisplayTransform { get; private set; }

        #endregion

        #region Public Methods

        public void LoadDiagrams(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return;

            UnloadDiagrams();

            var fileExt = IoPath.GetExtension(fileName);

            var curCursor = Cursor;
            Cursor = Cursors.Wait;
            try
            {
                object xamlObject = null;

                if (string.Equals(fileExt, ".xaml", StringComparison.OrdinalIgnoreCase))
                    using (var xmlReader = XmlReader.Create(new StreamReader(fileName)))
                    {
                        xamlObject = XamlReader.Load(xmlReader);
                    }
                else if (string.Equals(fileExt, ".zaml", StringComparison.OrdinalIgnoreCase))
                    using (var fileStream = File.OpenRead(fileName))
                    {
                        using (var zipStream =
                               new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            xamlObject = XamlReader.Load(zipStream);
                        }
                    }

                if (xamlObject is SvgImage)
                {
                    var imageDrawing = (SvgImage)xamlObject;
                    RenderDiagrams(imageDrawing);
                }
                else if (xamlObject is DrawingGroup)
                {
                    var groupDrawing = (DrawingGroup)xamlObject;
                    RenderDiagrams(groupDrawing);
                }
            }
            finally
            {
                Cursor = curCursor;
            }
        }

        public void LoadDiagrams(DrawingGroup whole,
            DrawingGroup links, DrawingGroup main)
        {
            if (whole == null) return;

            UnloadDiagrams();

            Draw(whole, main);

            _wholeDrawing = whole;
            _linksDrawing = links;
            _mainDrawing = main;

            InvalidateMeasure();
        }

        public void UnloadDiagrams()
        {
            _offsetX = 0;
            _offsetY = 0;
            Bounds = new Rect(0, 0, 1, 1);

            _wholeDrawing = null;

            DisplayTransform = Transform.Identity;

            ClearVisuals();
            ClearDrawings();

            InvalidateMeasure();
            InvalidateVisual();
        }

        #region RenderDiagrams Methods

        public void RenderDiagrams(SvgImage image)
        {
            var drawingImage = image.Source as DrawingImage;
            if (drawingImage == null) return;

            var renderedGroup = drawingImage.Drawing as DrawingGroup;

            if (renderedGroup != null) RenderDiagrams(renderedGroup);
        }

        public void RenderDiagrams(DrawingGroup renderedGroup)
        {
            var drawings = renderedGroup.Children;
            var linkIndex = -1;
            var drawIndex = -1;
            for (var i = 0; i < drawings.Count; i++)
            {
                var drawing = drawings[i];
                //string drawingName = drawing.GetValue(FrameworkElement.NameProperty) as string;
                var drawingName = SvgLink.GetKey(drawing);
                if (!string.IsNullOrEmpty(drawingName) &&
                    string.Equals(drawingName, SvgObject.DrawLayer))
                    drawIndex = i;
                else if (!string.IsNullOrEmpty(drawingName) &&
                         string.Equals(drawingName, SvgObject.LinksLayer))
                    linkIndex = i;
            }

            DrawingGroup mainGroups = null;
            if (drawIndex >= 0) mainGroups = drawings[drawIndex] as DrawingGroup;
            DrawingGroup linkGroups = null;
            if (linkIndex >= 0) linkGroups = drawings[linkIndex] as DrawingGroup;

            LoadDiagrams(renderedGroup, linkGroups, mainGroups);

            if (linkGroups != null) AnimationCanvas.LoadDiagrams(linkGroups, renderedGroup);

            InvalidateMeasure();
            InvalidateVisual();
        }

        #endregion

        #endregion

        #region Protected Methods

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (_hostVisual == null) return null;

            return _hostVisual;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //_offsetX = 0;
            //_offsetY = 0;

            if (_wholeDrawing != null)
            {
                var rectBounds = _wholeDrawing.Bounds;

                if (!rectBounds.IsEmpty)
                {
                    // Return the new size
                    var diaWidth = rectBounds.Width;
                    var diaHeight = rectBounds.Height;
                    if (rectBounds.X > 0)
                    {
                        //diaWidth += rectBounds.X;
                        //_offsetX = rectBounds.X;
                    }

                    if (rectBounds.Y > 0)
                    {
                        //diaHeight += rectBounds.Y;
                        //_offsetY = rectBounds.Y;
                    }

                    Bounds = rectBounds;

                    //double sizeX = this.Width;
                    //double sizeY = this.Height;
                    //if ((!Double.IsNaN(sizeX) && !Double.IsInfinity(sizeX)) &&
                    //    (!Double.IsNaN(sizeY) && !Double.IsInfinity(sizeY)))
                    //{
                    //    diaWidth = sizeX;
                    //    diaHeight = sizeY;

                    //    _bounds.Width  = sizeX;
                    //    _bounds.Height = sizeY;
                    //}

                    return new Size(diaWidth, diaHeight);

                    //double inflateX = 6;
                    //double inflateY = 6;
                    //if (rectBounds.X > 0)
                    //{
                    //    inflateX = rectBounds.X/2d;
                    //}
                    //double diaHeight = rectBounds.Height;
                    //if (rectBounds.Y > 0)
                    //{
                    //    inflateY = rectBounds.Y / 2d;
                    //}

                    //rectBounds.Inflate(inflateX, inflateY);

                    //_bounds = rectBounds;

                    //return rectBounds.Size;
                }
            }

            return new Size(640, 480);
            //return base.MeasureOverride(constraint);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (AnimationCanvas != null && AnimationCanvas.HandleMouseDown(e)) return;

            var pt = e.GetPosition(this);

            var visual = HitTest(pt);
            if (visual == null)
            {
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                Cursor = Cursors.Arrow;
                return;
            }

            var itemName = visual.GetValue(NameProperty) as string;
            if (itemName == null)
                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }
            //Brush brush = null;
            //if (_visualBrushes.ContainsKey(itemName))
            //{
            //    brush = _visualBrushes[itemName];
            //}
            //if (brush == null)
            //{
            //    if (_tooltip != null)
            //    {
            //        _tooltip.IsOpen = false;
            //        _tooltip.Visibility = Visibility.Hidden;
            //    }

            //    return;
            //}

            //if (e.ChangedButton == MouseButton.Left)
            //{
            //    string brushName = brush.GetValue(FrameworkElement.NameProperty) as string;
            //    if (!String.IsNullOrEmpty(brushName))
            //    {
            //        SvgLinkAction linkAction = SvgLink.GetLinkAction(visual);
            //        if (linkAction == SvgLinkAction.LinkHtml ||
            //            linkAction == SvgLinkAction.LinkPage)
            //        {
            //            _animator.Start(brushName, brush);
            //        }
            //    }
            //}
            //else if (e.ChangedButton == MouseButton.Right)
            //{
            //    _animator.Stop();
            //}
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (AnimationCanvas != null && AnimationCanvas.HandleMouseMove(e)) return;

            // Retrieve the coordinates of the mouse button event.
            var pt = e.GetPosition(this);

            var hitVisual = HitTest(pt);

            //string itemName = null;

            if (hitVisual == null)
            {
                Cursor = Cursors.Arrow;

                if (_hitVisual != null)
                    //itemName = _hitVisual.GetValue(FrameworkElement.NameProperty) as string;
                    //if (itemName == null)
                    //{
                    //    _hitVisual = null;
                    //    return;
                    //}
                    //if (_visualBrushes.ContainsKey(itemName))
                    //{
                    //    Brush brush = _visualBrushes[itemName];
                    //    brush.Opacity = 0;
                    //}
                    _hitVisual = null;

                if (_tooltip != null)
                {
                    _tooltip.IsOpen = false;
                    _tooltip.Visibility = Visibility.Hidden;
                }

                return;
            }

            Cursor = Cursors.Hand;

            if (hitVisual == _hitVisual) return;

            if (_hitVisual != null)
                //itemName = _hitVisual.GetValue(FrameworkElement.NameProperty) as string;
                //if (itemName == null)
                //{
                //    _hitVisual = null;
                //    return;
                //}
                //if (_visualBrushes.ContainsKey(itemName))
                //{
                //    Brush brush = _visualBrushes[itemName];
                //    brush.Opacity = 0;
                //}
                _hitVisual = null;

            //itemName = hitVisual.GetValue(FrameworkElement.NameProperty) as string;
            //if (itemName == null)
            //{
            //    return;
            //}
            //if (_visualBrushes.ContainsKey(itemName))
            //{
            //    Brush brush = _visualBrushes[itemName];
            //    brush.Opacity = 0.5;
            //}
            _hitVisual = hitVisual;

            var tooltipText = SvgObject.GetTitle(_hitVisual);
            var rectBounds = _hitVisual.Bounds;
            //Drawing drawing = _hitVisual.GetValue(FrameworkElement.TagProperty) as Drawing;
            //if (drawing != null)
            //{
            //    rectBounds  = drawing.Bounds; 
            //    tooltipText = SvgObject.GetTitle(drawing);
            //}

            if (_tooltip != null && !string.IsNullOrEmpty(tooltipText))
            {
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
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (AnimationCanvas != null && AnimationCanvas.HandleMouseLeave(e)) return;

            if (_tooltip != null)
            {
                _tooltip.IsOpen = false;
                _tooltip.Visibility = Visibility.Hidden;
            }

            if (_hitVisual == null) return;

            var itemName = _hitVisual.GetValue(NameProperty) as string;
            if (itemName == null)
            {
                _hitVisual = null;
                return;
            }

            //if (_visualBrushes.ContainsKey(itemName))
            //{
            //    Brush brush = _visualBrushes[itemName];
            //    brush.Opacity = 0;
            //}
            _hitVisual = null;

            Cursor = Cursors.Arrow;
        }

        #endregion

        #region Private Methods

        #region Visuals Methods

        private void AddVisual(DrawingVisual visual)
        {
            if (visual == null) return;

            _hostVisual = visual;

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        private void RemoveVisual(DrawingVisual visual)
        {
            if (visual == null) return;

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        private void ClearVisuals()
        {
            if (_hostVisual != null)
            {
                RemoveVisualChild(_hostVisual);
                RemoveLogicalChild(_hostVisual);

                _hostVisual = null;
            }
        }

        private void AddDrawing(Drawing visual)
        {
            if (visual == null) return;

            _drawObjects.Add(visual);
        }

        private void RemoveDrawing(Drawing visual)
        {
            if (visual == null) return;

            _drawObjects.Remove(visual);

            if (_linkObjects != null && _linkObjects.Count != 0) _linkObjects.Remove(visual);
        }

        private void ClearDrawings()
        {
            if (_drawObjects != null && _drawObjects.Count != 0) _drawObjects.Clear();

            if (_linkObjects != null && _linkObjects.Count != 0) _linkObjects.Clear();
        }

        #endregion

        #region Draw Methods

        private void Draw(DrawingGroup group, DrawingGroup main)
        {
            var drawingVisual = new DrawingVisual();

            var drawingContext = drawingVisual.RenderOpen();

            _offsetX = 0;
            _offsetY = 0;
            DisplayTransform = Transform.Identity;

            TranslateTransform offsetTransform = null;
            var rectBounds = group.Bounds;
            if (!rectBounds.IsEmpty)
            {
                // Return the new size
                //double diaWidth = rectBounds.Width;
                if (rectBounds.X > 0)
                    //diaWidth += rectBounds.X;
                    _offsetX = rectBounds.X;
                //double diaHeight = rectBounds.Height;
                if (rectBounds.Y > 0)
                    //diaHeight += rectBounds.Y;
                    _offsetY = rectBounds.Y;

                Bounds = rectBounds;

                if (_offsetX > 0 || _offsetY > 0)
                {
                    offsetTransform = new TranslateTransform(-_offsetX, -_offsetY);
                    DisplayTransform = new TranslateTransform(_offsetX, _offsetY); // the inverse...
                }
            }

            //Canvas.SetTop(this, -_offsetX);
            //Canvas.SetLeft(this, -_offsetY);

            //if (offsetTransform != null)
            //{
            //    drawingContext.PushTransform(offsetTransform);
            //}

            drawingContext.DrawDrawing(group);

            //if (offsetTransform != null)
            //{
            //    drawingContext.Pop();
            //}

            drawingVisual.Opacity = group.Opacity;

            //Transform transform = group.Transform;
            //if (transform == null)
            //{
            //    transform = offsetTransform;
            //}
            if (offsetTransform != null) drawingVisual.Transform = offsetTransform;
            var clipGeometry = group.ClipGeometry;
            if (clipGeometry != null) drawingVisual.Clip = clipGeometry;

            drawingContext.Close();

            AddVisual(drawingVisual);

            if (_drawForInteractivity)
            {
                if (main == null) main = group;

                EnumerateDrawings(main);
            }
        }

        //private void Draw(DrawingGroup group)
        //{
        //    DrawingVisual drawingVisual = new DrawingVisual();

        //    DrawingContext drawingContext = drawingVisual.RenderOpen();

        //    if (_offsetTransform != null)
        //    {
        //        drawingContext.PushTransform(_offsetTransform);
        //    }

        //    drawingContext.DrawDrawing(group);

        //    if (_offsetTransform != null)
        //    {
        //        drawingContext.Pop();
        //    }

        //    drawingContext.DrawDrawing(group);
        //    drawingVisual.Opacity = group.Opacity;

        //    Transform transform = group.Transform;
        //    if (transform != null)
        //    {
        //        drawingVisual.Transform = transform;
        //    }
        //    Geometry clipGeometry = group.ClipGeometry;
        //    if (clipGeometry != null)
        //    {
        //        drawingVisual.Clip = clipGeometry;
        //    }

        //    drawingContext.Close();

        //    this.AddVisual(drawingVisual);

        //    if (_drawForInteractivity)
        //    {
        //        this.EnumerateDrawings(group);
        //    }
        //}

        private void EnumerateDrawings(DrawingGroup group)
        {
            if (group == null || group == _linksDrawing) return;

            var drawings = group.Children;
            for (var i = 0; i < drawings.Count; i++)
            {
                var drawing = drawings[i];
                var childGroup = drawing as DrawingGroup;
                if (childGroup != null)
                {
                    var objectType = SvgObject.GetType(childGroup);
                    if (objectType == SvgObjectType.Link)
                        InsertLinkDrawing(childGroup);
                    else if (objectType == SvgObjectType.Text)
                        InsertTextDrawing(childGroup);
                    else
                        EnumerateDrawings(childGroup);
                }
                else
                {
                    InsertDrawing(drawing);
                }
            }
        }

        private void InsertTextDrawing(DrawingGroup group)
        {
            AddDrawing(group);
        }

        private void InsertLinkDrawing(DrawingGroup group)
        {
            AddDrawing(group);

            if (_linkObjects != null) _linkObjects.Add(group);
        }

        private void InsertDrawing(Drawing drawing)
        {
            AddDrawing(drawing);
        }

        #endregion

        #region HitTest Methods

        //private Brush _testHitBrush;
        //private Brush _testHitBrushPen;
        //private Pen _testHitPen;
        //private GeometryDrawing _testHit;
        //private DrawingGroup _testHitGroup;
        private Drawing HitTest(Point pt)
        {
            if (_linkObjects == null) return null;

            var ptDisplay = DisplayTransform.Transform(pt);

            DrawingGroup groupDrawing = null;
            GlyphRunDrawing glyRunDrawing = null;
            GeometryDrawing geometryDrawing = null;

            Drawing foundDrawing = null;

            //for (int i = 0; i < _linkObjects.Count; i++)
            for (var i = _linkObjects.Count - 1; i >= 0; i--)
            {
                var drawing = _linkObjects[i];
                if (TryCast.Cast(drawing, out geometryDrawing))
                {
                    if (HitTestDrawing(geometryDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    if (HitTestDrawing(groupDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }

                    if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text &&
                        groupDrawing.Bounds.Contains(ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
            }

            //if (_testHit != null)
            //{
            //    if (_testHitBrush != null)
            //    {
            //        _testHit.Brush = _testHitBrush;
            //    }
            //    else if (_testHitPen != null && _testHitBrushPen != null)
            //    {
            //        _testHit.Pen.Brush = _testHitBrushPen;
            //    }

            //    _testHit = null;
            //    _testHitPen = null;
            //    _testHitBrush = null;
            //}
            //if (_testHitGroup != null)
            //{
            //    _testHitGroup.BitmapEffect = null;
            //    _testHitGroup = null;
            //}

            //_testHit = foundDrawing as GeometryDrawing;
            //if (_testHit != null)
            //{
            //    _testHitBrush = _testHit.Brush;
            //    _testHitPen = _testHit.Pen;

            //    // Create and animate a Brush to set the button's Background.
            //    SolidColorBrush animationBrush = new SolidColorBrush();
            //    animationBrush.Color = Colors.Blue;

            //    ColorAnimation colorAnimation = new ColorAnimation();
            //    colorAnimation.From           = Colors.Blue;
            //    colorAnimation.To             = Colors.Red;
            //    colorAnimation.Duration       = new Duration(TimeSpan.FromMilliseconds(1000));
            //    colorAnimation.AutoReverse    = true;
            //    colorAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //    if (_testHitBrush != null)
            //    {
            //        _testHit.Brush = animationBrush;
            //    }
            //    else if (_testHitPen != null)
            //    {
            //        _testHitBrushPen  = _testHitPen.Brush;
            //        _testHitPen.Brush = animationBrush;
            //    }

            //    // Apply the animation to the brush's Color property.
            //    //animationBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            //}
            //else
            //{
            //    _testHitGroup = foundDrawing as DrawingGroup;
            //    if (_testHitGroup != null)
            //    {
            //        //// Create a blur effect.
            //        //BlurBitmapEffect blurEffect = new BlurBitmapEffect();
            //        //blurEffect.Radius = 3.0;

            //        //// Apply it to the drawing group.
            //        //_testHitGroup.BitmapEffect = blurEffect;

            //        // Initialize a new OuterGlowBitmapEffect that will be applied
            //        // to the TextBox.
            //        OuterGlowBitmapEffect glowEffect = new OuterGlowBitmapEffect();

            //        // Set the size of the glow to 30 pixels.
            //        glowEffect.GlowSize = 3;

            //        // Set the color of the glow to blue.
            //        Color glowColor = new Color();
            //        glowColor.ScA = 1;
            //        glowColor.ScB = 0;
            //        glowColor.ScG = 0;
            //        glowColor.ScR = 1;
            //        glowEffect.GlowColor = glowColor;

            //        // Set the noise of the effect to the maximum possible (range 0-1).
            //        glowEffect.Noise = 0;

            //        // Set the Opacity of the effect to 75%. Note that the same effect
            //        // could be done by setting the ScA property of the Color to 0.75.
            //        glowEffect.Opacity = 0.5;

            //        // Apply the bitmap effect to the TextBox.
            //        _testHitGroup.BitmapEffect = glowEffect;
            //    }
            //}   

            return foundDrawing;
        }

        private bool HitTestDrawing(GlyphRunDrawing drawing, Point pt)
        {
            if (drawing != null && drawing.Bounds.Contains(pt)) return true;

            return false;
        }

        private bool HitTestDrawing(GeometryDrawing drawing, Point pt)
        {
            var pen = drawing.Pen;
            var brush = drawing.Brush;
            if (pen != null && brush == null)
            {
                if (drawing.Geometry.StrokeContains(pen, pt)) return true;

                var geometry = drawing.Geometry;

                EllipseGeometry ellipse = null;
                RectangleGeometry rectangle = null;
                PathGeometry path = null;
                if (TryCast.Cast(geometry, out ellipse))
                {
                    if (ellipse.FillContains(pt)) return true;
                }
                else if (TryCast.Cast(geometry, out rectangle))
                {
                    if (rectangle.FillContains(pt)) return true;
                }
                else if (TryCast.Cast(geometry, out path))
                {
                    var pathFigures = path.Figures;
                    var itemCount = pathFigures.Count;
                    if (itemCount == 1)
                    {
                        if (pathFigures[0].IsClosed && path.FillContains(pt)) return true;
                    }
                    else
                    {
                        for (var f = 0; f < itemCount; f++)
                        {
                            var pathFigure = pathFigures[f];
                            if (pathFigure.IsClosed)
                            {
                                var testFigures = new PathFigureCollection();
                                testFigures.Add(pathFigure);

                                var testPath = new PathGeometry();
                                testPath.Figures = testFigures;

                                if (testPath.FillContains(pt)) return true;
                            }
                        }
                    }
                }
            }
            else if (brush != null && drawing.Geometry.FillContains(pt))
            {
                return true;
            }

            return false;
        }

        private bool HitTestDrawing(DrawingGroup group, Point pt)
        {
            if (group.Bounds.Contains(pt))
            {
                DrawingGroup groupDrawing = null;
                GlyphRunDrawing glyRunDrawing = null;
                GeometryDrawing geometryDrawing = null;
                var drawings = group.Children;

                for (var i = 0; i < drawings.Count; i++)
                {
                    var drawing = drawings[i];
                    if (TryCast.Cast(drawing, out geometryDrawing))
                    {
                        if (HitTestDrawing(geometryDrawing, pt)) return true;
                    }
                    else if (TryCast.Cast(drawing, out groupDrawing))
                    {
                        if (HitTestDrawing(groupDrawing, pt)) return true;
                        var objectType = SvgObject.GetType(groupDrawing);
                        if (objectType == SvgObjectType.Text && groupDrawing.Bounds.Contains(pt)) return true;
                    }
                    else if (TryCast.Cast(drawing, out glyRunDrawing))
                    {
                        if (HitTestDrawing(glyRunDrawing, pt)) return true;
                    }
                }
            }

            return false;
        }

        #endregion

        private static bool IsValidBounds(Rect rectBounds)
        {
            if (rectBounds.IsEmpty || double.IsNaN(rectBounds.Width) || double.IsNaN(rectBounds.Height)
                || double.IsInfinity(rectBounds.Width) || double.IsInfinity(rectBounds.Height))
                return false;

            return true;
        }

        #endregion
    }
}