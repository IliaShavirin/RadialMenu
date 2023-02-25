using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BaseProj.AttachedBehaviours
{
    public class EnumHelper : DependencyObject
    {
        // Using a DependencyProperty as the backing store for Enum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.RegisterAttached("Enum", typeof(Type), typeof(EnumHelper),
                new PropertyMetadata(null, OnEnumChanged));

        public static readonly DependencyProperty IsSelectFirstItemOnEnumPopulatedProperty =
            DependencyProperty.RegisterAttached(
                "IsSelectFirstItemOnEnumPopulated",
                typeof(bool),
                typeof(EnumHelper)
                , new PropertyMetadata(default(bool), OnIsSelectFirstItemOnEnumPopulatedPropertyChanged));

        // Using a DependencyProperty as the backing store for MoreDetails.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoreDetailsProperty =
            DependencyProperty.RegisterAttached("MoreDetails", typeof(bool), typeof(EnumHelper),
                new PropertyMetadata(false, OnMoreDetailsChanged));

        public static Type GetEnum(DependencyObject obj)
        {
            return (Type)obj.GetValue(EnumProperty);
        }

        public static void SetEnum(DependencyObject obj, Type value)
        {
            obj.SetValue(EnumProperty, value);
        }

        private static void OnEnumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ItemsControl;

            if (control != null)
                if (e.NewValue is Type type)
                {
                    var _enum = Enum.GetValues(type);
                    control.ItemsSource = _enum;

                    if (GetIsSelectFirstItemOnEnumPopulated(control) && control is Selector selector)
                    {
                        selector.SelectedItem = _enum.GetValue(0);
                        ;
                    }
                }
        }

        public static void SetIsSelectFirstItemOnEnumPopulated(DependencyObject element, bool value)
        {
            element.SetValue(IsSelectFirstItemOnEnumPopulatedProperty, value);
        }

        public static bool GetIsSelectFirstItemOnEnumPopulated(DependencyObject element)
        {
            return (bool)element.GetValue(IsSelectFirstItemOnEnumPopulatedProperty);
        }

        private static void OnIsSelectFirstItemOnEnumPopulatedPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            var control = obj as Control;
            if (control == null || !(args.NewValue is bool)) return;

            if ((bool)args.NewValue && control is Selector selector)
            {
                var enumType = GetEnum(selector);
                if (enumType != null)
                {
                    var _enum = Enum.GetValues(enumType);
                    selector.SelectedItem = _enum.GetValue(0);
                }
            }
        }

        public static bool GetMoreDetails(DependencyObject obj)
        {
            return (bool)obj.GetValue(MoreDetailsProperty);
        }

        public static void SetMoreDetails(DependencyObject obj, bool value)
        {
            obj.SetValue(MoreDetailsProperty, value);
        }

        private static void OnMoreDetailsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control != null)
            {
                var enumobject = control.DataContext;
                var fieldInfo = enumobject.GetType().GetField(enumobject.ToString());

                var array = fieldInfo.GetCustomAttributes(false);

                if (array.Length == 0)
                {
                    if (control is TextBlock)
                        ((TextBlock)control).Text = enumobject.ToString();
                    else if (control is ContentControl) ((ContentControl)control).Content = enumobject;
                    return;
                }

                foreach (var o in array)
                    if (o is DescriptionAttribute)
                    {
                        control.ToolTip = ((DescriptionAttribute)o).Description;
                    }
                    else if (o is DisplayAttribute)
                    {
                        if (control is TextBlock)
                            ((TextBlock)control).Text = ((DisplayAttribute)o).Name;
                        else if (control is ContentControl)
                            ((ContentControl)control).Content = ((DisplayAttribute)o).Name;
                    }
            }
        }
    }
}