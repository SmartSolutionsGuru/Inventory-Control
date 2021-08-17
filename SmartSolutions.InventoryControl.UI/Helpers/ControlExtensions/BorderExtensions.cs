using SmartSolutions.InventoryControl.UI.Converters;
using SmartSolutions.InventoryControl.UI.CustomControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlExtensions
{
    public class BorderExtensions
    {
        public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
           "Circular", typeof(bool), typeof(BorderExtensions), new PropertyMetadata(ValueBoxes.FalseBox, OnCircularChanged));

        private static void OnCircularChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Border border)
            {
                if ((bool)e.NewValue)
                {
                    var binding = new MultiBinding
                    {
                        Converter = new BorderCircularConverter()
                    };
                    binding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = border });
                    binding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name) { Source = border });
                    border.SetBinding(Border.CornerRadiusProperty, binding);
                }
                else
                {
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualWidthProperty);
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualHeightProperty);
                    BindingOperations.ClearBinding(border, Border.CornerRadiusProperty);
                }
            }
        }

        public static void SetCircular(DependencyObject element, bool value)
            => element.SetValue(CircularProperty, ValueBoxes.BooleanBox(value));

        public static bool GetCircular(DependencyObject element)
            => (bool)element.GetValue(CircularProperty);
    }
}
