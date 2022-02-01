using System.Windows;
using System.Windows.Controls;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlHelpers
{
    public class ComboBoxHelper : ComboBox
    {
        public static bool GetShowSelectAll(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowSelectAllProperty);
        }

        public static void SetShowSelectAll(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowSelectAllProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowSelectAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSelectAllProperty =
            DependencyProperty.RegisterAttached("ShowSelectAll", typeof(bool), typeof(ComboBoxHelper), new PropertyMetadata(false));
    }
}
