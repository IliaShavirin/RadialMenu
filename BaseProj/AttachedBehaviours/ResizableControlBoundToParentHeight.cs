using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BaseProj.AttachedBehaviours
{
    public class ResizableControlBoundToParentHeight
    {
        private static readonly Dictionary<int, Resizer> resizersDictionary = new Dictionary<int, Resizer>();

        public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterAttached(
            "Parent", typeof(UIElement), typeof(ResizableControlBoundToParentHeight),
            new PropertyMetadata(default(UIElement), OnParentChangedCallback));

        private static void OnParentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Attach(d as FrameworkElement, e.NewValue as FrameworkElement);
        }

        public static void SetParent(DependencyObject element, UIElement value)
        {
            element.SetValue(ParentProperty, value);
        }

        public static UIElement GetParent(DependencyObject element)
        {
            return (UIElement)element.GetValue(ParentProperty);
        }

        private static void Attach(FrameworkElement d, FrameworkElement parent)
        {
            if (d == null || parent == null) return;
            if (!resizersDictionary.Keys.Contains(d.GetHashCode()))
                resizersDictionary.Add(d.GetHashCode(), new Resizer(d, parent));
        }

        private class Resizer : DependencyObject
        {
            public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
                "Height", typeof(double), typeof(Resizer), new PropertyMetadata(100D));

            private readonly FrameworkElement _element;
            private readonly FrameworkElement _parent;
            private bool _isInited;

            private double _lastScrollHeight;
            private ScrollViewer _scroll;

            public Resizer(FrameworkElement element, FrameworkElement parent)
            {
                _element = element;
                _parent = parent;

                Init();
            }

            public double Height
            {
                get => (double)GetValue(HeightProperty);
                set => SetValue(HeightProperty, value);
            }

            public void Init()
            {
                if (_isInited) return;
                _element.Loaded += (s, e) =>
                {
                    var binding = new Binding(nameof(Height))
                    {
                        Source = this,
                        Mode = BindingMode.OneWay
                    };

                    _scroll = VisualTreeHelperExt.FindParent<ScrollViewer>(_element);

                    if (_scroll == null) return;

                    _element.SetBinding(FrameworkElement.HeightProperty, binding);
                    _element.SetBinding(FrameworkElement.MaxHeightProperty, binding);

                    _lastScrollHeight = _scroll.ActualHeight;

                    SetHeight(_element, _scroll, _parent);

                    if (!_isInited)
                    {
                        Application.Current.MainWindow.SizeChanged += (sender, args) =>
                        {
                            SetHeight(_element, _scroll, _parent);
                        };

                        Application.Current.MainWindow.StateChanged += (sender, args) =>
                        {
                            _parent.LayoutUpdated += ParentOnLayoutUpdated;
                            _parent.UpdateLayout();
                        };
                        _isInited = true;
                    }
                };
            }

            private void ParentOnLayoutUpdated(object sender, EventArgs eventArgs)
            {
                SetHeight(_element, _scroll, _parent);
//                _parent.LayoutUpdated -= ParentOnLayoutUpdated;
            }

            private void SetHeight(FrameworkElement d, ScrollViewer scroll, FrameworkElement parent)
            {
                var newScrollHeight = scroll.ActualHeight;
                var delta = newScrollHeight - _lastScrollHeight;
                _lastScrollHeight = newScrollHeight;

                var val = Height;

                if (delta > 0)
                {
                    if (scroll.ComputedVerticalScrollBarVisibility != Visibility.Visible)
                    {
                        if (parent is Border)
                        {
                            var border = parent as Border;
                            val = border.ActualHeight - border.BorderThickness.Top - border.BorderThickness.Bottom -
                                  border.Margin.Top - border.Margin.Bottom;
                        }
                        else if (parent is Grid)
                        {
                            var grid = parent as Grid;
                            var rowIndex = Grid.GetRow(d);
                            if (grid.RowDefinitions.Count > 0)
                                val = grid.RowDefinitions[rowIndex].ActualHeight;
                            else
                                val = grid.ActualHeight;
                        }
                        else
                        {
                            val = parent.ActualHeight;
                        }
                    }
                }
                else if (delta < 0)
                {
//                    if (scroll.ComputedVerticalScrollBarVisibility != Visibility.Visible)
//                    {
                    val += delta;
//                    }
                }
                else
                {
                    if (parent is Border)
                    {
                        var border = parent as Border;
                        val = border.ActualHeight - border.BorderThickness.Top - border.BorderThickness.Bottom -
                              border.Margin.Top - border.Margin.Bottom;
                    }
                    else if (parent is Grid)
                    {
                        var grid = parent as Grid;
                        var rowIndex = Grid.GetRow(d);
                        if (grid.RowDefinitions.Count > 0)
                            val = grid.RowDefinitions[rowIndex].ActualHeight;
                        else
                            val = grid.ActualHeight;
                    }
                    else
                    {
                        val = parent.ActualHeight;
                    }
                }

                // !!!!Note!!!!: If parent container is collapsed then val will be 0, which automatically will put it to MinHeight.
                // In order to avoid this behavior parent container must be hidden instead, so it actually takes space on initialization
                if (val < _element.MinHeight) val = _element.MinHeight;
                Height = val;
            }
        }
    }
}