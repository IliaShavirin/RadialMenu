using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;

namespace RadialMenu.CustomControls
{
    /// <summary>
    ///     Interaction logic for RadialMenu.xaml
    /// </summary>
    public class RadialMenu : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(RadialMenu),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty IsSubMenuProperty =
            DependencyProperty.Register("IsSubMenu", typeof(bool), typeof(RadialMenu),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HalfShiftedItemsProperty =
            DependencyProperty.Register("HalfShiftedItems", typeof(bool), typeof(RadialMenu),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CentralItemProperty =
            DependencyProperty.Register("CentralItem", typeof(RadialMenuCentralItem), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public new static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(List<RadialMenuItem>), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinStrokeThicknessProperty =
            DependencyProperty.Register("MinStrokeThickness", typeof(double), typeof(RadialMenu),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MaxStrokeThicknessProperty =
            DependencyProperty.Register("MaxStrokeThickness", typeof(double), typeof(RadialMenu),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(RadialMenuItem), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty AddSectorDrawingProperty =
            DependencyProperty.Register("AddSectorDrawing", typeof(Drawing), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty AddSubConfigDrawingProperty =
            DependencyProperty.Register("AddSubConfigDrawing", typeof(Drawing), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty DeleteSectorDrawingProperty =
            DependencyProperty.Register("DeleteSectorDrawing", typeof(Drawing), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));


        static RadialMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialMenu),
                new FrameworkPropertyMetadata(typeof(RadialMenu)));
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public bool IsSubMenu
        {
            get => (bool)GetValue(IsSubMenuProperty);
            set => SetValue(IsSubMenuProperty, value);
        }

        public bool HalfShiftedItems
        {
            get => (bool)GetValue(HalfShiftedItemsProperty);
            set => SetValue(HalfShiftedItemsProperty, value);
        }

        public RadialMenuCentralItem CentralItem
        {
            get => (RadialMenuCentralItem)GetValue(CentralItemProperty);
            set => SetValue(CentralItemProperty, value);
        }

        public new List<RadialMenuItem> Content
        {
            get => (List<RadialMenuItem>)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public double MinStrokeThickness
        {
            get => (double)GetValue(MinStrokeThicknessProperty);
            set => SetValue(MinStrokeThicknessProperty, value);
        }

        public double MaxStrokeThickness
        {
            get => (double)GetValue(MaxStrokeThicknessProperty);
            set => SetValue(MaxStrokeThicknessProperty, value);
        }

        public List<RadialMenuItem> Items
        {
            get => Content;
            set => Content = value;
        }

        public RadialMenuItem CurrentItem
        {
            get => (RadialMenuItem)GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        public Drawing AddSectorDrawing
        {
            get => (Drawing)GetValue(AddSectorDrawingProperty);
            set => SetValue(AddSectorDrawingProperty, value);
        }

        public Drawing AddSubConfigDrawing
        {
            get => (Drawing)GetValue(AddSubConfigDrawingProperty);
            set => SetValue(AddSubConfigDrawingProperty, value);
        }

        public Drawing DeleteSectorDrawing
        {
            get => (Drawing)GetValue(DeleteSectorDrawingProperty);
            set => SetValue(DeleteSectorDrawingProperty, value);
        }

        public override void BeginInit()
        {
            Content = new List<RadialMenuItem>();
            base.BeginInit();
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            for (int i = 0, count = Items.Count; i < count; i++)
            {
                Items[i].Index = i;
                Items[i].Count = count;
                Items[i].HalfShifted = HalfShiftedItems;
                Items[i].RadialMenu = this;
            }

            return base.ArrangeOverride(arrangeSize);
        }

        #region Commands

        public static readonly DependencyProperty AddSectorAboveCommandProperty =
            DependencyProperty.Register("AddSectorAboveCommand", typeof(DelegateCommand), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DelegateCommand AddSectorAboveCommand
        {
            get => (DelegateCommand)GetValue(AddSectorAboveCommandProperty);
            set => SetValue(AddSectorAboveCommandProperty, value);
        }

        public static readonly DependencyProperty AddSectorAfterCommandProperty =
            DependencyProperty.Register("AddSectorAfterCommand", typeof(DelegateCommand), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DelegateCommand AddSectorAfterCommand
        {
            get => (DelegateCommand)GetValue(AddSectorAfterCommandProperty);
            set => SetValue(AddSectorAfterCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteSectorCommandProperty =
            DependencyProperty.Register("DeleteSectorCommand", typeof(DelegateCommand), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DelegateCommand DeleteSectorCommand
        {
            get => (DelegateCommand)GetValue(DeleteSectorCommandProperty);
            set => SetValue(DeleteSectorCommandProperty, value);
        }

        public static readonly DependencyProperty AddSubMenuCommandProperty =
            DependencyProperty.Register("AddSubMenuCommand", typeof(DelegateCommand), typeof(RadialMenu),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DelegateCommand AddSubMenuCommand
        {
            get => (DelegateCommand)GetValue(AddSubMenuCommandProperty);
            set => SetValue(AddSubMenuCommandProperty, value);
        }

        #endregion
    }
}