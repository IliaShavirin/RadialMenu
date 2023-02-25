using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaseProj.AttachedBehaviours
{
    public static class ScrollViewerHelper
    {
        public static readonly DependencyProperty UseHorizontalScrollingProperty = DependencyProperty.RegisterAttached(
            "UseHorizontalScrolling", typeof(bool), typeof(ScrollViewerHelper),
            new PropertyMetadata(default(bool), UseHorizontalScrollingChangedCallback));

        private static void UseHorizontalScrollingChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var itemsControl = dependencyObject as ItemsControl;

            if (itemsControl == null) throw new ArgumentException("Element is not an ItemsControl");

            itemsControl.PreviewMouseWheel += delegate(object sender, MouseWheelEventArgs args)
            {
                var scrollViewer = VisualTreeHelperExt.FindChild<ScrollViewer>(itemsControl);

                if (scrollViewer == null) return;

                if (scrollViewer.ComputedVerticalScrollBarVisibility != Visibility.Visible)
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - args.Delta);
                    args.Handled = true;
                }
            };
        }


        public static void SetUseHorizontalScrolling(ItemsControl element, bool value)
        {
            element.SetValue(UseHorizontalScrollingProperty, value);
        }

        public static bool GetUseHorizontalScrolling(ItemsControl element)
        {
            return (bool)element.GetValue(UseHorizontalScrollingProperty);
        }
    }
}