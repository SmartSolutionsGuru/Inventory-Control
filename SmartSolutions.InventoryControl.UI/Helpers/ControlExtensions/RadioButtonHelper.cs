using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlHelpers
{
    public class RadioButtonHelper : RadioButton
    {
        public static Brush GetOnBackgroundBrush(RadioButton obj)
        {
            return (Brush)obj.GetValue(OnBackgroundBrushProperty);
        }

        public static void SetOnBackgroundBrush(RadioButton obj, Brush value)
        {
            obj.SetValue(OnBackgroundBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnBackgroundBrushProperty =
            DependencyProperty.RegisterAttached("OnBackgroundBrush", typeof(Brush), typeof(RadioButtonHelper), new PropertyMetadata(null));


        public static Brush GetOffBackgroundBrush(RadioButton obj)
        {
            return (Brush)obj.GetValue(OffBackgroundBrushProperty);
        }

        public static void SetOffBackgroundBrush(RadioButton obj, Brush value)
        {
            obj.SetValue(OffBackgroundBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for OffBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffBackgroundBrushProperty =
            DependencyProperty.RegisterAttached("OffBackgroundBrush", typeof(Brush), typeof(RadioButtonHelper), new PropertyMetadata(null));

        public static Brush GetOnForegroundBrush(RadioButton obj)
        {
            return (Brush)obj.GetValue(OnForegroundBrushProperty);
        }

        public static void SetOnForegroundBrush(RadioButton obj, Brush value)
        {
            obj.SetValue(OnForegroundBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnForegroundBrushProperty =
            DependencyProperty.RegisterAttached("OnForegroundBrush", typeof(Brush), typeof(RadioButtonHelper), new PropertyMetadata(null));

        public static Brush GetOffForegroundBrush(RadioButton obj)
        {
            return (Brush)obj.GetValue(OffForegroundBrushProperty);
        }

        public static void SetOffForegroundBrush(RadioButton obj, Brush value)
        {
            obj.SetValue(OffForegroundBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for OffForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffForegroundBrushProperty =
            DependencyProperty.RegisterAttached("OffForegroundBrush", typeof(Brush), typeof(RadioButtonHelper), new PropertyMetadata(null));
    }
}
