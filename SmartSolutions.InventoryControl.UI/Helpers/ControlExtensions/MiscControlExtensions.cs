using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlExtensions
{
    public class MiscControlExtensions : FrameworkElement
    {
        public static bool GetIsLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadingProperty);
        }

        public static void SetIsLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadingProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(MiscControlExtensions), new PropertyMetadata(false));

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
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(MiscControlExtensions), new PropertyMetadata(new CornerRadius(0)));

        public static Brush GetCheckedColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckedColorProperty);
        }

        public static void SetCheckedColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckedColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for CheckedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedColorProperty =
            DependencyProperty.RegisterAttached("CheckedColor", typeof(Brush), typeof(MiscControlExtensions), new PropertyMetadata(null));

        public static Brush GetUncheckedColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(UncheckedColorProperty);
        }

        public static void SetUncheckedColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(UncheckedColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for UncheckedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UncheckedColorProperty =
            DependencyProperty.RegisterAttached("UncheckedColor", typeof(Brush), typeof(MiscControlExtensions), new PropertyMetadata(null));

        public static Brush GetUncheckedForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(UncheckedForegroundProperty);
        }

        public static void SetUncheckedForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(UncheckedForegroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for UncheckedForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UncheckedForegroundProperty =
            DependencyProperty.RegisterAttached("UncheckedForeground", typeof(Brush), typeof(MiscControlExtensions), new PropertyMetadata(null));

        public static Geometry GetShape(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(ShapeProperty);
        }

        public static void SetShape(DependencyObject obj, Geometry value)
        {
            obj.SetValue(ShapeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Shape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.RegisterAttached("Shape", typeof(Geometry), typeof(MiscControlExtensions), new PropertyMetadata(null));



        public static FlowDocument GetFlowDocument(RichTextBox obj)
        {
            return (FlowDocument)obj.GetValue(FlowDocumentProperty);
        }

        public static void SetFlowDocument(RichTextBox obj, FlowDocument value)
        {
            obj.SetValue(FlowDocumentProperty, value);
        }

        // Using a DependencyProperty as the backing store for FlowDocument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlowDocumentProperty =
            DependencyProperty.RegisterAttached("FlowDocument", typeof(FlowDocument), typeof(MiscControlExtensions), new PropertyMetadata(null, FlowDocumentChanged));

        private static void FlowDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextBox control = d as RichTextBox;
            if (control != null)
            {
                if ((e.NewValue as FlowDocument) != null)
                {
                    control.Document = e.NewValue as FlowDocument;
                }
                else
                {
                    control.Document = new FlowDocument();
                }
            }
        }


        public static double GetIconWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(double), typeof(MiscControlExtensions), new PropertyMetadata(0d));



        public static double GetIconHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(double), typeof(MiscControlExtensions), new PropertyMetadata(0d));



    }
}

