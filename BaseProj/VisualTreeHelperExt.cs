using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseProj
{
    /// <summary>
    ///     Use VisualTreeHelperExt.HitTest instead of VisualTreeHelper.HitTest to take in account 'IsHitTestVisible' and
    ///     'IsVisible' properties
    /// </summary>
    public static class VisualTreeHelperExt
    {
        public static HitTestResult HitTest(Visual visual, Point point)
        {
            HitTestResult result = null;

            // Use the advanced HitTest method and specify a custom filter that filters out the
            // invisible elements or the elements that don't allow hittesting.
            VisualTreeHelper.HitTest(visual,
                target =>
                {
                    var uiElement = target as UIElement;
                    if (uiElement != null &&
                        (!uiElement.IsHitTestVisible || !uiElement.IsVisible))
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                    return HitTestFilterBehavior.Continue;
                },
                target =>
                {
                    result = target;
                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(point));

            // Return the result
            return result;
        }

        public static HitTestResult HitTestTreeViewItem(Visual visual, Point point)
        {
            HitTestResult result = null;

            // Use the advanced HitTest method and specify a custom filter that filters out the
            // invisible elements or the elements that don't allow hittesting.
            VisualTreeHelper.HitTest(visual,
                target =>
                {
                    var uiElement = target as UIElement;
                    if (uiElement != null)
                        if (
                            uiElement.GetType() == typeof(TextBlock) ||
                            uiElement.GetType() == typeof(Image) ||
                            uiElement.GetType() == typeof(Border) ||
                            uiElement.GetType().ToString() == "System.Windows.Controls.TextBoxView" ||
                            uiElement.GetType().ToString() == "System.Windows.Controls.TextBoxLineDrawingVisual"
                        )
                        {
                            if (uiElement is TextBlock && string.IsNullOrEmpty(((TextBlock)uiElement).Text))
                                return HitTestFilterBehavior.ContinueSkipSelf;
                            return HitTestFilterBehavior.Continue;
                        }

                    return HitTestFilterBehavior.ContinueSkipSelf;
                },
                target =>
                {
                    result = target;
                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(point));

            // Return the result
            return result;
        }

        public static DependencyObject GetItemByMousePoint(Visual visual, Point point, Type targetType)
        {
            var htResult = HitTest(visual, point);
            if (htResult == null) return null;

            var k = htResult.VisualHit;

            while (k != null)
            {
                if (k.GetType() == targetType) return k;

                k = VisualTreeHelper.GetParent(k);
            }

            return null;
        }

        public static DependencyObject FindParent(DependencyObject child, List<Type> parentTypes,
            ushort? parentLevelThreshold = null)
        {
            ushort currentParentLevel = 0;
            return FindParent(child, parentTypes, parentLevelThreshold, ref currentParentLevel);
        }

        private static DependencyObject FindParent(DependencyObject child, List<Type> parentTypes,
            ushort? parentLevelThreshold, ref ushort currentParentLevel)
        {
            if (!(child is Visual) && !(child is Visual3D)) return null;

            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            if (parentTypes.Any(t => t.IsInstanceOfType(parentObject))) return parentObject;

            currentParentLevel++;
            if (currentParentLevel > parentLevelThreshold)
                return null;
            return FindParent(parentObject, parentTypes, parentLevelThreshold, ref currentParentLevel);
        }

        public static T FindParentByParentProperty<T>(FrameworkElement child) where T : FrameworkElement
        {
            var control = child;
            while (control != null)
            {
                control = control?.Parent as FrameworkElement;
                if (control is T) return control as T;
            }

            return null;
        }

        public static T FindParent<T>(DependencyObject child, ushort? parentLevelThreshold = null)
            where T : DependencyObject
        {
            ushort currentParentLevel = 0;
            return FindParent<T>(child, parentLevelThreshold, ref currentParentLevel);
        }

        private static T FindParent<T>(DependencyObject child, ushort? parentLevelThreshold,
            ref ushort currentParentLevel) where T : DependencyObject
        {
            if (!(child is Visual) && !(child is Visual3D)) return null;

            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            if (parent != null) return parent;

            currentParentLevel++;
            if (currentParentLevel > parentLevelThreshold)
                return null;
            return FindParent<T>(parentObject, parentLevelThreshold, ref currentParentLevel);
        }

        public static T FindChild<T>(DependencyObject parent, string childName = null)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static List<T> FindChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            var foundChildren = new List<T>();

            if (parent == null) return foundChildren;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;

                if (childType != null && !foundChildren.Contains(child)) foundChildren.Add((T)child);

                foundChildren.AddRange(FindChildren<T>(child));


                //if (childType == null)
                //{
                //    // recursively drill down the tree
                //    foundChildren = FindChildren<T>(child);

                //    // If the child is found, break so we do not overwrite the found child. 
                //    if (foundChildren.Count > 0 )
                //        foundChildren.AddRange(foundChildren);
                //}
                //else
                //{
                //    // child element found.
                //    foundChildren.Add((T)child);
                //}
            }

            var parentAsContentControl = parent as ContentControl;
            if (parentAsContentControl != null && parentAsContentControl.Content != null)
                foundChildren.AddRange(FindChildren<T>(parentAsContentControl.Content as DependencyObject));

            return foundChildren.Distinct().ToList();
        }

        public static List<T> FindChildrenOnTheSameLevel<T>(DependencyObject parent)
            where T : DependencyObject
        {
            var foundChildren = new List<T>();

            if (parent == null) return foundChildren;

            var parents = new List<DependencyObject>();
            parents.Add(parent);

            FindChildrenOnTheSameLevel(parents, foundChildren);

            return foundChildren;
        }

        private static void FindChildrenOnTheSameLevel<T>(List<DependencyObject> parents, List<T> result)
            where T : DependencyObject
        {
            if (parents == null || result == null)
                return;

            var children = new List<DependencyObject>();
            foreach (var parent in parents)
            {
                var count = VisualTreeHelper.GetChildrenCount(parent);
                for (var i = 0; i < count; i++)
                {
                    var item = VisualTreeHelper.GetChild(parent, i);
                    children.Add(item);
                }
            }

            foreach (var item in children)
                if (item is T tItem)
                    result.Add(tItem);

            if (result.Count == 0)
                FindChildrenOnTheSameLevel(children, result);
        }

        public static UIElement GetByUid(DependencyObject rootElement, string uid)
        {
            foreach (var element in LogicalTreeHelper.GetChildren(rootElement).OfType<UIElement>())
            {
                if (element.Uid == uid)
                    return element;
                var resultChildren = GetByUid(element, uid);
                if (resultChildren != null)
                    return resultChildren;
            }

            return null;
        }

        public static void RemoveChild(DependencyObject parent, UIElement child)
        {
            var panel = parent as Panel;
            if (panel != null)
            {
                panel.Children.Remove(child);
                return;
            }

            var decorator = parent as Decorator;
            if (decorator != null)
            {
                if (decorator.Child == child) decorator.Child = null;
                return;
            }

            var contentPresenter = parent as ContentPresenter;
            if (contentPresenter != null)
            {
                if (contentPresenter.Content == child) contentPresenter.Content = null;
                return;
            }

            var contentControl = parent as ContentControl;
            if (contentControl != null)
                if (contentControl.Content == child)
                    contentControl.Content = null;

            // maybe more
        }

        public static void AddChild(DependencyObject parent, UIElement child)
        {
            var panel = parent as Panel;
            if (panel != null)
            {
                panel.Children.Add(child);

                return;
            }

            var contentPresenter = parent as ContentPresenter;
            if (contentPresenter != null)
            {
                contentPresenter.Content = child;
                ;
            }
        }
    }
}