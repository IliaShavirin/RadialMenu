using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaseProj.Buttons
{
    public abstract class BaseColoredButton : Button
    {
        public static readonly DependencyProperty IsTriggerSelfOnClickProperty = DependencyProperty.Register(
            "IsTriggerSelfOnClick", typeof(bool), typeof(BaseColoredButton),
            new PropertyMetadata(default(bool), IsTriggerSelfOnClickChangedCallback));

        public static readonly DependencyProperty UntriggerSelfOnUnloadProperty = DependencyProperty.Register(
            "UntriggerSelfOnUnload", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(false));

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName", typeof(string), typeof(BaseColoredButton),
            new PropertyMetadata(default(string), OnGroupNameChanged));

        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
            "IsHighlighted", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(default(bool)));

        [ThreadStatic] private static Hashtable _groupNameToElements;

        public static readonly DependencyProperty IsTriggeredProperty = DependencyProperty.Register(
            "IsTriggered", typeof(bool), typeof(BaseColoredButton),
            new PropertyMetadata(default(bool), IsTriggeredChangedCallback));

        public static readonly DependencyProperty IsMouseOverHiglightEnabledProperty = DependencyProperty.Register(
            "IsMouseOverHiglightEnabled", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(true));

        public static readonly DependencyProperty IsHighlightedOnKeyboardFocusProperty = DependencyProperty.Register(
            "IsHighlightedOnKeyboardFocus", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(true));


        public static readonly DependencyProperty IsColorShiftProperty = DependencyProperty.Register(
            "IsColorShift", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(true));


        public static readonly DependencyProperty IsClickedProperty = DependencyProperty.Register(
            "IsClicked", typeof(bool), typeof(BaseColoredButton), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(BaseColoredButton),
            new PropertyMetadata(default(CornerRadius)));

        public BaseColoredButton()
        {
            Loaded += BaseColoredButton_Loaded;
            Unloaded += OnUnloaded;
        }

        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            set => SetValue(IsHighlightedProperty, value);
        }

        public bool IsTriggered
        {
            get => (bool)GetValue(IsTriggeredProperty);
            set => SetValue(IsTriggeredProperty, value);
        }

        public bool IsMouseOverHiglightEnabled
        {
            get => (bool)GetValue(IsMouseOverHiglightEnabledProperty);
            set => SetValue(IsMouseOverHiglightEnabledProperty, value);
        }

        public bool IsHighlightedOnKeyboardFocus
        {
            get => (bool)GetValue(IsHighlightedOnKeyboardFocusProperty);
            set => SetValue(IsHighlightedOnKeyboardFocusProperty, value);
        }

        public bool IsColorShift
        {
            get => (bool)GetValue(IsColorShiftProperty);
            set => SetValue(IsColorShiftProperty, value);
        }

        public bool IsClicked
        {
            get => (bool)GetValue(IsClickedProperty);
            set => SetValue(IsClickedProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private void BaseColoredButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (PressedBackground == null)
                PressedBackground = Application.Current?.MainWindow?.TryFindResource("ButtonBackgroundHover") as Brush;
            if (PressedBorderBrush == null)
                PressedBorderBrush =
                    Application.Current?.MainWindow?.TryFindResource("ButtonBorderBrushHover") as Brush;
            if (PressedForeground == null)
                PressedForeground = Application.Current?.MainWindow?.TryFindResource("ButtonForeground") as Brush;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (UntriggerSelfOnUnload) IsTriggered = false;
        }

        private static void IsTriggeredChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zis = d as BaseColoredButton;
            if (zis == null) return;

            zis.OnIsTriggeredChanged(e);
        }

        private void OnIsTriggeredChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateTriggeredButtonGroup();
        }

        #region TriggerSelf

        private void TriggerSelf(object sender, RoutedEventArgs e)
        {
            IsTriggered = !IsTriggered;
        }


        public bool IsTriggerSelfOnClick
        {
            get => (bool)GetValue(IsTriggerSelfOnClickProperty);
            set => SetValue(IsTriggerSelfOnClickProperty, value);
        }

        private static void IsTriggerSelfOnClickChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var zis = d as BaseColoredButton;
            if (zis == null) return;

            zis.OnIsTriggerSelfOnClickChanged(e);
        }

        private void OnIsTriggerSelfOnClickChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsTriggerSelfOnClick)
                Click += TriggerSelf;
            else
                Click -= TriggerSelf;
        }


        public bool UntriggerSelfOnUnload
        {
            get => (bool)GetValue(UntriggerSelfOnUnloadProperty);
            set => SetValue(UntriggerSelfOnUnloadProperty, value);
        }

        #endregion // End - TriggerSelf

        #region GroupName

        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }


        private string _currentlyRegisteredGroupName;

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (BaseColoredButton)d;
            var newValue = e.NewValue as string;
            var groupName = button._currentlyRegisteredGroupName;

            if (!(newValue != groupName))
                return;

            if (!string.IsNullOrEmpty(groupName))
                Unregister(groupName, button);
            if (string.IsNullOrEmpty(newValue))
                return;
            Register(newValue, button);
        }

        private static void Register(string groupName, BaseColoredButton button)
        {
            if (_groupNameToElements == null)
                _groupNameToElements = new Hashtable(1);

            lock (_groupNameToElements)
            {
                var elements = (ArrayList)_groupNameToElements[groupName];
                if (elements == null)
                {
                    elements = new ArrayList(1);
                    _groupNameToElements[groupName] = elements;
                }
                else
                {
                    PurgeDead(elements, null);
                }

                elements.Add(new WeakReference(button));
            }

            button.GroupName = groupName;
            button._currentlyRegisteredGroupName = groupName;
        }

        private static void Unregister(string groupName, BaseColoredButton button)
        {
            if (_groupNameToElements == null)
                return;
            lock (_groupNameToElements)
            {
                var groupNameToElement = (ArrayList)_groupNameToElements[groupName];
                if (groupNameToElement != null)
                {
                    PurgeDead(groupNameToElement, button);
                    if (groupNameToElement.Count == 0)
                        _groupNameToElements.Remove(groupName);
                }
            }

            button.GroupName = null;
            button._currentlyRegisteredGroupName = null;
        }

        private static void PurgeDead(ArrayList elements, object elementToRemove)
        {
            var index = 0;
            while (index < elements.Count)
            {
                var target = ((WeakReference)elements[index]).Target;
                if (target == null || target == elementToRemove)
                    elements.RemoveAt(index);
                else
                    ++index;
            }
        }

        private bool _shouldUpdateTriggeredButtonGroup = true;

        private void UpdateTriggeredButtonGroup()
        {
            if (!_shouldUpdateTriggeredButtonGroup)
                return;

            var groupName = GroupName;
            if (!string.IsNullOrEmpty(groupName))
            {
                if (_groupNameToElements == null)
                    _groupNameToElements = new Hashtable(1);

                lock (_groupNameToElements)
                {
                    var groupNameToElement = (ArrayList)_groupNameToElements[groupName];
                    var index = 0;
                    while (index < groupNameToElement.Count)
                        if (!(((WeakReference)groupNameToElement[index]).Target is BaseColoredButton target))
                        {
                            groupNameToElement.RemoveAt(index);
                        }
                        else
                        {
                            if (target != this)
                                if (target.IsTriggered)
                                    target.UntriggerButton();
                            ++index;
                        }
                }
            }
            //This code is from radiobuttons that kinda work without group name if name not specifed it tries to find neighbors
            //else
            //{
            //    DependencyObject parent = this.Parent;

            //    if (parent == null)
            //        return;

            //    foreach (object child in LogicalTreeHelper.GetChildren(parent))
            //    {
            //        if (child is BaseColoredButton button && button != this && string.IsNullOrEmpty(button.GroupName))
            //        {
            //            if (button.IsTriggered)
            //                button.UntriggerButton();
            //        }
            //    }
            //}
        }

        private void UntriggerButton()
        {
            _shouldUpdateTriggeredButtonGroup = false;
            IsTriggered = false;
            _shouldUpdateTriggeredButtonGroup = true;
        }

        #endregion // End - GroupName

        #region Highlight Brushes

        public static readonly DependencyProperty HighlightedBackgroundProperty = DependencyProperty.Register(
            "HighlightedBackground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(null));

        public Brush HighlightedBackground
        {
            get => (Brush)GetValue(HighlightedBackgroundProperty);
            set => SetValue(HighlightedBackgroundProperty, value);
        }

        public static readonly DependencyProperty HighlightedForegroundProperty = DependencyProperty.Register(
            "HighlightedForeground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(null));

        public Brush HighlightedForeground
        {
            get => (Brush)GetValue(HighlightedForegroundProperty);
            set => SetValue(HighlightedForegroundProperty, value);
        }

        public static readonly DependencyProperty HighlightedBorderBrushProperty = DependencyProperty.Register(
            "HighlightedBorderBrush", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(null));

        public Brush HighlightedBorderBrush
        {
            get => (Brush)GetValue(HighlightedBorderBrushProperty);
            set => SetValue(HighlightedBorderBrushProperty, value);
        }

        #endregion //End Highlight Brushes

        #region Disabled Brushes

        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            "DisabledBackground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush DisabledBackground
        {
            get => (Brush)GetValue(DisabledBackgroundProperty);
            set => SetValue(DisabledBackgroundProperty, value);
        }

        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            "DisabledForeground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush DisabledForeground
        {
            get => (Brush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }

        public static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.Register(
            "DisabledBorderBrush", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush DisabledBorderBrush
        {
            get => (Brush)GetValue(DisabledBorderBrushProperty);
            set => SetValue(DisabledBorderBrushProperty, value);
        }

        #endregion //End Disabled Brushes


        #region Pressed Brushes

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
            "PressedBackground", typeof(Brush), typeof(BaseColoredButton)); // highlighted by default

        public Brush PressedBackground
        {
            get => (Brush)GetValue(PressedBackgroundProperty);
            set => SetValue(PressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register(
            "PressedBorderBrush", typeof(Brush), typeof(BaseColoredButton)); // highlighted by default

        public Brush PressedBorderBrush
        {
            get => (Brush)GetValue(PressedBorderBrushProperty);
            set => SetValue(PressedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register(
            "PressedForeground", typeof(Brush), typeof(BaseColoredButton)); // highlighted by default

        public Brush PressedForeground
        {
            get => (Brush)GetValue(PressedForegroundProperty);
            set => SetValue(PressedForegroundProperty, value);
        }

        #endregion //End Pressed Brushes

        #region Triggered Colors

        public static readonly DependencyProperty TriggeredBackgroundProperty = DependencyProperty.Register(
            "TriggeredBackground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush TriggeredBackground
        {
            get => (Brush)GetValue(TriggeredBackgroundProperty);
            set => SetValue(TriggeredBackgroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredBorderBrushProperty = DependencyProperty.Register(
            "TriggeredBorderBrush", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush TriggeredBorderBrush
        {
            get => (Brush)GetValue(TriggeredBorderBrushProperty);
            set => SetValue(TriggeredBorderBrushProperty, value);
        }

        public static readonly DependencyProperty TriggeredForegroundProperty = DependencyProperty.Register(
            "TriggeredForeground", typeof(Brush), typeof(BaseColoredButton), new PropertyMetadata(default(Brush)));

        public Brush TriggeredForeground
        {
            get => (Brush)GetValue(TriggeredForegroundProperty);
            set => SetValue(TriggeredForegroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredHighlightedBackgroundProperty = DependencyProperty.Register(
            "TriggeredHighlightedBackground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredHighlightedBackground
        {
            get => (Brush)GetValue(TriggeredHighlightedBackgroundProperty);
            set => SetValue(TriggeredHighlightedBackgroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredHighlightedBorderBrushProperty = DependencyProperty.Register(
            "TriggeredHighlightedBorderBrush", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredHighlightedBorderBrush
        {
            get => (Brush)GetValue(TriggeredHighlightedBorderBrushProperty);
            set => SetValue(TriggeredHighlightedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty TriggeredHighlightedForegroundProperty = DependencyProperty.Register(
            "TriggeredHighlightedForeground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredHighlightedForeground
        {
            get => (Brush)GetValue(TriggeredHighlightedForegroundProperty);
            set => SetValue(TriggeredHighlightedForegroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredPressedBackgroundProperty = DependencyProperty.Register(
            "TriggeredPressedBackground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredPressedBackground
        {
            get => (Brush)GetValue(TriggeredPressedBackgroundProperty);
            set => SetValue(TriggeredPressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredPressedBorderBrushProperty = DependencyProperty.Register(
            "TriggeredPressedBorderBrush", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredPressedBorderBrush
        {
            get => (Brush)GetValue(TriggeredPressedBorderBrushProperty);
            set => SetValue(TriggeredPressedBorderBrushProperty, value);
        }

        public static readonly DependencyProperty TriggeredPressedForegroundProperty = DependencyProperty.Register(
            "TriggeredPressedForeground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredPressedForeground
        {
            get => (Brush)GetValue(TriggeredPressedForegroundProperty);
            set => SetValue(TriggeredPressedForegroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredDisabledBackgroundProperty = DependencyProperty.Register(
            "TriggeredDisabledBackground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredDisabledBackground
        {
            get => (Brush)GetValue(TriggeredDisabledBackgroundProperty);
            set => SetValue(TriggeredDisabledBackgroundProperty, value);
        }

        public static readonly DependencyProperty TriggeredDisabledBorderBrushProperty = DependencyProperty.Register(
            "TriggeredDisabledBorderBrush", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredDisabledBorderBrush
        {
            get => (Brush)GetValue(TriggeredDisabledBorderBrushProperty);
            set => SetValue(TriggeredDisabledBorderBrushProperty, value);
        }

        public static readonly DependencyProperty TriggeredDisabledForegroundProperty = DependencyProperty.Register(
            "TriggeredDisabledForeground", typeof(Brush), typeof(BaseColoredButton),
            new PropertyMetadata(default(Brush)));

        public Brush TriggeredDisabledForeground
        {
            get => (Brush)GetValue(TriggeredDisabledForegroundProperty);
            set => SetValue(TriggeredDisabledForegroundProperty, value);
        }

        #endregion // End Triggered Colors
    }
}