using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using Xceed.Wpf.Toolkit;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlHelpers
{
    public class CheckComboBoxHelper  
    {
        public static string GetCustomTextContent(DependencyObject obj)
        {
            return (string)obj.GetValue(CustomTextContentProperty);
        }

        public static void SetCustomTextContent(DependencyObject obj, string value)
        {
            obj.SetValue(CustomTextContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for CustomTextContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomTextContentProperty =
            DependencyProperty.RegisterAttached("CustomTextContent", typeof(string), typeof(CheckComboBoxHelper), new PropertyMetadata(null));


    }
}
