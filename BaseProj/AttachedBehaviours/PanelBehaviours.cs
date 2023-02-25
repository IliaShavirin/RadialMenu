using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BaseProj.AttachedBehaviours
{
    public class PanelBehaviours
    {
        // ----------------------------------------------------------------------
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(PanelBehaviours),
            new FrameworkPropertyMetadata(OnPanelBehavioursEnabledChanged));

        private readonly double InitialPanelHeight;
        private bool loaded;
        public Panel Panel;

        public PanelBehaviours(Panel panel)
        {
            Panel = panel;
            InitialPanelHeight = Panel.Height;
            Panel.Loaded += PanelLoaded;
            Panel.Unloaded += PanelUnloaded;
        }

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private void PanelLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(Panel);
            loaded = true;
        } // ListViewLoaded

        // ----------------------------------------------------------------------
        private void PanelUnloaded(object sender, RoutedEventArgs e)
        {
            if (loaded) return;
            UnregisterEvents(Panel);
            loaded = false;
        }

        private void RegisterEvents(Panel panel)
        {
            OnIsCollapsedIfContentNotVisible(panel);
        }

        // ----------------------------------------------------------------------
        private void UnregisterEvents(Panel panel)
        {
            DetachIsCollapsedIfContentNotVisible(panel);
        }

        private static void OnPanelBehavioursEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var panel = dependencyObject as Panel;
            if (panel != null)
            {
                var enabled = (bool)e.NewValue;
                if (enabled) new PanelBehaviours(panel);
            }
        }

        #region CollapsedIfContentNotVisible

        public static bool GetIsCollapsedIfContentNotVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCollapsedIfContentNotVisibleProperty);
        }

        public static void SetIsCollapsedIfContentNotVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCollapsedIfContentNotVisibleProperty, value);
        }

        /// <summary>
        ///     For this mechanics to work Panel should have a fixed height set
        /// </summary>
        public static readonly DependencyProperty IsCollapsedIfContentNotVisibleProperty =
            DependencyProperty.RegisterAttached("IsCollapsedIfContentNotVisible", typeof(bool), typeof(PanelBehaviours),
                new UIPropertyMetadata(false));

        private void OnIsCollapsedIfContentNotVisible(DependencyObject d)
        {
            var panel = d as Panel;
            if (panel == null)
                return;

            if (GetIsCollapsedIfContentNotVisible(panel))
                AttachIsCollapsedIfContentNotVisible(panel);
            else
                DetachIsCollapsedIfContentNotVisible(panel);
        }

        private void AttachIsCollapsedIfContentNotVisible(Panel panel)
        {
            if (panel == null)
                return;

            var children = panel.Children.Cast<UIElement>();
            foreach (var uiElement in children) uiElement.IsVisibleChanged += OnChildVisibilityChanged;
        }

        private void DetachIsCollapsedIfContentNotVisible(Panel panel)
        {
            if (panel == null)
                return;

            var children = panel.Children.Cast<UIElement>();
            foreach (var uiElement in children) uiElement.IsVisibleChanged -= OnChildVisibilityChanged;
        }

        private void OnChildVisibilityChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            var shoudlCollapseParent =
                Panel.Children.Cast<UIElement>().All(element => element.Visibility != Visibility.Visible);

            Panel.Height = shoudlCollapseParent ? 0 : InitialPanelHeight;
        }

        #endregion
    }
}