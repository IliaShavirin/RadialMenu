using System.Windows;
using System.Windows.Controls;

namespace RadialMenu.CustomControls
{
    /// <summary>
    ///     Interaction logic for RadialMenuCentralItem.xaml
    /// </summary>
    public class RadialMenuCentralItem : ContentControl
    {
        static RadialMenuCentralItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialMenuCentralItem),
                new FrameworkPropertyMetadata(typeof(RadialMenuCentralItem)));
        }
    }
}