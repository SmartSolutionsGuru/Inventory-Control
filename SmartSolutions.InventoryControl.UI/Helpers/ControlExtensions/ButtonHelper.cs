using System.Windows;
using System.Windows.Controls;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlHelpers
{
    public class ButtonHelper : Button
    {
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonHelper), new PropertyMetadata(new CornerRadius()));





        public static bool GetIsBusyProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBusyPropertyProperty);
        }

        public static void SetIsBusyProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBusyPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsBusyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyPropertyProperty =
            DependencyProperty.RegisterAttached("IsBusyProperty", typeof(bool), typeof(ButtonHelper), new PropertyMetadata(false));




    }
}
