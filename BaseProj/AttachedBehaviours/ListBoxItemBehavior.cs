using System.Windows;
using System.Windows.Controls;

namespace BaseProj.AttachedBehaviours
{
    /// <summary>
    ///     Exposes attached behaviors that can be
    ///     applied to ListViewItem objects.
    /// </summary>
    public static class ListBoxItemBehavior
    {
        #region IsBroughtIntoViewWhenAdded

        public static bool GetIsBroughtIntoViewWhenAdded(ListBoxItem listBoxItem)
        {
            return (bool)listBoxItem.GetValue(IsBroughtIntoViewWhenAddedProperty);
        }

        public static void SetIsBroughtIntoViewWhenAdded(
            ListBoxItem listBoxItem, bool value)
        {
            listBoxItem.SetValue(IsBroughtIntoViewWhenAddedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenAddedProperty =
            DependencyProperty.RegisterAttached(
                "IsBroughtIntoViewWhenAdded",
                typeof(bool),
                typeof(ListBoxItemBehavior),
                new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenAddedChanged));

        private static void OnIsBroughtIntoViewWhenAddedChanged(
            DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            //Tracer.TraceWrite("OnIsBroughtIntoViewWhenAddedChanged");
            //var item = depObj as ListViewItem;
            //if (item == null)
            //    return;

            //if (e.NewValue is bool == false)
            //    return;

            //item.BringIntoView();

            var item = depObj as ListBoxItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                item.Selected += OnListBoxItemSelected;
                item.BringIntoView();
            }
            else
            {
                item.Selected -= OnListBoxItemSelected;
            }
        }

        private static void OnListBoxItemSelected(object sender, RoutedEventArgs e)
        {
            //Tracer.TraceWrite("OnListBoxItemSelected");
            // Only react to the Selected event raised by the ListBoxItem 
            // whose IsSelected property was modified.  Ignore all ancestors 
            // who are merely reporting that a descendant's Selected fired. 
            if (!ReferenceEquals(sender, e.OriginalSource))
                return;

            var item = e.OriginalSource as ListBoxItem;
            if (item != null)
                item.BringIntoView();
        }

        #endregion // IsBroughtIntoViewWhenAdded
    }
}