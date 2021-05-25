using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlExtensions
{
    public class WaterMarkTextExtension : DependencyObject
    {
        #region Attached Properties

        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(WaterMarkTextExtension), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkTextProperty);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkTextProperty, value);
        }

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(WaterMarkTextExtension), new UIPropertyMetadata(string.Empty));


        public static int GetTextLength(DependencyObject obj)
        {
            return (int)obj.GetValue(TextLengthProperty);
        }

        public static void SetTextLength(DependencyObject obj, int value)
        {
            obj.SetValue(TextLengthProperty, value);

            if (value >= 1)
                obj.SetValue(HasTextProperty, true);
            else
                obj.SetValue(HasTextProperty, false);
        }

        public static readonly DependencyProperty TextLengthProperty =
            DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(WaterMarkTextExtension), new UIPropertyMetadata(0));

        public static Brush GetSelectionBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectionBrushProperty);
        }

        public static void SetSelectionBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectionBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.RegisterAttached("SelectionBrush", typeof(Brush), typeof(WaterMarkTextExtension), new PropertyMetadata(Brushes.AntiqueWhite));

        public static Brush GetWatermarkColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WatermarkColorProperty);
        }

        public static void SetWatermarkColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(WatermarkColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkColorProperty =
            DependencyProperty.RegisterAttached("WatermarkColor", typeof(Brush), typeof(WaterMarkTextExtension), new PropertyMetadata(Brushes.White));

        public static ImageSource GetWatermarkIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(WatermarkIconProperty);
        }

        public static void SetWatermarkIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(WatermarkIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconProperty =
            DependencyProperty.RegisterAttached("WatermarkIcon", typeof(ImageSource), typeof(WaterMarkTextExtension), new PropertyMetadata(null));

        public static Brush GetWatermarkIconFocusedColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WatermarkIconFocusedColorProperty);
        }

        public static void SetWatermarkIconFocusedColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(WatermarkIconFocusedColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconUnfocusedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconFocusedColorProperty =
            DependencyProperty.RegisterAttached("WatermarkIconFocusedColor", typeof(Brush), typeof(WaterMarkTextExtension), new PropertyMetadata(null));

        public static Geometry GetWatermarkIconPath(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(WatermarkIconPathProperty);
        }

        public static void SetWatermarkIconPath(DependencyObject obj, Geometry value)
        {
            obj.SetValue(WatermarkIconPathProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconPathProperty =
            DependencyProperty.RegisterAttached("WatermarkIconPath", typeof(Geometry), typeof(WaterMarkTextExtension), new PropertyMetadata(null));

        public static Visibility GetWatermarkIconVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(WatermarkIconVisibilityProperty);
        }

        public static void SetWatermarkIconVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(WatermarkIconVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconVisibilityProperty =
            DependencyProperty.RegisterAttached("WatermarkIconVisibility", typeof(Visibility), typeof(WaterMarkTextExtension), new PropertyMetadata(Visibility.Visible));

        public static Thickness GetWatermarkIconMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(WatermarkIconMarginProperty);
        }

        public static double GetWatermarkIconWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WatermarkIconWidthProperty);
        }

        public static void SetWatermarkIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkIconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconWidthProperty =
            DependencyProperty.RegisterAttached("WatermarkIconWidth", typeof(double), typeof(WaterMarkTextExtension), new PropertyMetadata(double.NaN));

        public static double GetWatermarkIconMaxWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WatermarkIconMaxWidthProperty);
        }

        public static void SetWatermarkIconMaxWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkIconMaxWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconMaxWidthProperty =
            DependencyProperty.RegisterAttached("WatermarkIconMaxWidth", typeof(double), typeof(WaterMarkTextExtension), new PropertyMetadata(35d));

        public static double GetWatermarkIconHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(WatermarkIconHeightProperty);
        }

        public static void SetWatermarkIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkIconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconHeightProperty =
            DependencyProperty.RegisterAttached("WatermarkIconHeight", typeof(double), typeof(WaterMarkTextExtension), new PropertyMetadata(double.NaN));

        public static double GetWatermarkIconMaxHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(WatermarkIconMaxHeightProperty);
        }

        public static void SetWatermarkIconMaxHeight(DependencyObject obj, double value)
        {
            obj.SetValue(WatermarkIconMaxHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconMaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconMaxHeightProperty =
            DependencyProperty.RegisterAttached("WatermarkIconMaxHeight", typeof(double), typeof(WaterMarkTextExtension), new PropertyMetadata(32d));

        public static void SetWatermarkIconMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(WatermarkIconMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconMarginProperty =
            DependencyProperty.RegisterAttached("WatermarkIconMargin", typeof(Thickness), typeof(WaterMarkTextExtension), new PropertyMetadata(new Thickness()));

      

        // Using a DependencyProperty as the backing store for ValidationErrorVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationErrorVisibleProperty =
            DependencyProperty.RegisterAttached("ValidationErrorVisible", typeof(bool), typeof(WaterMarkTextExtension), new PropertyMetadata(false));

        public static string GetValidationErrorMessage(DependencyObject obj)
        {
            return (string)obj.GetValue(ValidationErrorMessageProperty);
        }

        public static void SetValidationErrorMessage(DependencyObject obj, string value)
        {
            obj.SetValue(ValidationErrorMessageProperty, value);
        }

        // Using a DependencyProperty as the backing store for ValidationErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationErrorMessageProperty =
            DependencyProperty.RegisterAttached("ValidationErrorMessage", typeof(string), typeof(WaterMarkTextExtension), new PropertyMetadata(string.Empty));

        public static Brush GetValidationErrorColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ValidationErrorColorProperty);
        }

        public static void SetValidationErrorColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(ValidationErrorColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for ValidationErrorColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationErrorColorProperty =
            DependencyProperty.RegisterAttached("ValidationErrorColor", typeof(Brush), typeof(WaterMarkTextExtension), new PropertyMetadata(Brushes.Red));

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
            DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(WaterMarkTextExtension), new PropertyMetadata(false));

        public static bool GetEnableTextHiding(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableTextHidingProperty);
        }

        public static void SetEnableTextHiding(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableTextHidingProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableTextHiding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTextHidingProperty =
            DependencyProperty.RegisterAttached("EnableTextHiding", typeof(bool), typeof(WaterMarkTextExtension), new PropertyMetadata(false, OnEnableTextHidingChanged));

        public static string GetCompleteText(DependencyObject obj)
        {
            return (string)obj.GetValue(CompleteTextProperty);
        }

        public static void SetCompleteText(DependencyObject obj, string value)
        {
            obj.SetValue(CompleteTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for CompleteText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompleteTextProperty =
            DependencyProperty.RegisterAttached("CompleteText", typeof(string), typeof(WaterMarkTextExtension), new PropertyMetadata(null));

        public static Geometry GetRightWatermarkIconPath(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(RightWatermarkIconPathProperty);
        }

        public static void SetRightWatermarkIconPath(DependencyObject obj, Geometry value)
        {
            obj.SetValue(RightWatermarkIconPathProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightWatermarkIconPathProperty =
            DependencyProperty.RegisterAttached("RightWatermarkIconPath", typeof(Geometry), typeof(WaterMarkTextExtension), new PropertyMetadata(null));

        public static Visibility GetRightWatermarkIconVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(RightWatermarkIconVisibilityProperty);
        }

        public static void SetRightWatermarkIconVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(RightWatermarkIconVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightWatermarkIconVisibilityProperty =
            DependencyProperty.RegisterAttached("RightWatermarkIconVisibility", typeof(Visibility), typeof(WaterMarkTextExtension), new PropertyMetadata(Visibility.Visible));

        public static Brush GetWatermarkIconColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WatermarkIconColorProperty);
        }

        public static void SetWatermarkIconColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(WatermarkIconColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconUnfocusedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkIconColorProperty =
            DependencyProperty.RegisterAttached("WatermarkIconColor", typeof(Brush), typeof(WaterMarkTextExtension), new PropertyMetadata(null));
        #endregion

        #region Internal DependencyProperty

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        private static readonly DependencyProperty HasTextProperty =
            DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(WaterMarkTextExtension), new FrameworkPropertyMetadata(false));

        #endregion

        #region Implementation

        static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox txtBox = d as TextBox;

                if ((bool)e.NewValue)
                    txtBox.TextChanged += TextChanged;
                else
                    txtBox.TextChanged -= TextChanged;
            }
            else if (d is PasswordBox)
            {
                PasswordBox passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                    passBox.PasswordChanged += PasswordChanged;
                else
                    passBox.PasswordChanged -= PasswordChanged;
            }
        }

        static void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox == null) return;
            SetTextLength(txtBox, txtBox.Text.Length);
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;
            if (passBox == null) return;
            SetTextLength(passBox, passBox.Password.Length);
        }

        private static void OnKeyboardEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;
                if ((bool)e.NewValue)
                {
                    txtBox.GotKeyboardFocus += OnGotkeyboardFocus;
                    txtBox.LostFocus += OnLostFocus;
                }
                else
                {
                    txtBox.GotKeyboardFocus -= OnGotkeyboardFocus;
                    txtBox.LostFocus -= OnLostFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passwordbox = d as PasswordBox;
                if ((bool)e.NewValue)
                {
                    passwordbox.GotKeyboardFocus += OnGotkeyboardFocus;
                    passwordbox.LostFocus += OnLostFocus;
                }
                else
                {
                    passwordbox.GotKeyboardFocus -= OnGotkeyboardFocus;
                    passwordbox.LostFocus -= OnLostFocus;
                }
            }
        }

        private static void OnGotkeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //KeyboardLayouts layout = KeyboardLayouts.Alphanumeric;
            //if (sender is TextBox)
            //{
            //    var txtBox = sender as TextBox;
            //    if (txtBox == null) return;
            //    layout = GetKeyboardLayout(txtBox);
            //}
            //else if (sender is PasswordBox)
            //{
            //    var passwordbox = sender as PasswordBox;
            //    if (passwordbox == null) return;
            //    layout = GetKeyboardLayout(passwordbox);
            //}

            //WpfKb.Controls.OnScreenKeyboard.ShowKeyboard(App.Current.MainWindow, layout);
            ////Helpers.Messenger.Default.Publish(new Helpers.SimpleMessage("KeyboardOpened"));
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ////TextBox txtBox = sender as TextBox;
            ////if (txtBox == null) return;
            ////var layout = GetKeyboardLayout(txtBox);
            //WpfKb.Controls.OnScreenKeyboard.HideKeyboard();
            ////Helpers.Messenger.Default.Publish(new Helpers.SimpleMessage("KeyboardClosed"));
        }

        private static void OnEnableTextHidingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    //txtBox.TextChanged += TextHidingTextChanged;
                    txtBox.GotKeyboardFocus += OnGotkeyboardTextHidingFocus;
                    txtBox.LostKeyboardFocus += OnLostkeyboardTextHidingFocus;
                }
                else
                {
                    //txtBox.TextChanged -= TextHidingTextChanged;
                    txtBox.GotKeyboardFocus -= OnGotkeyboardTextHidingFocus;
                    txtBox.LostKeyboardFocus -= OnLostkeyboardTextHidingFocus;
                }
            }
            else if (d is PasswordBox)
            {
                //PasswordBox passBox = d as PasswordBox;

                //if ((bool)e.NewValue)
                //    passBox.PasswordChanged += PasswordChanged;
                //else
                //    passBox.PasswordChanged -= PasswordChanged;
            }
        }

        static void TextHidingTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox == null || (Keyboard.FocusedElement != txtBox && !txtBox.IsFocused)) return;
            SetCompleteText(txtBox, txtBox.Text);
        }

        private static void OnGotkeyboardTextHidingFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox txtBox = sender as TextBox;
                txtBox.Text = GetCompleteText(txtBox);
                txtBox.TextChanged += TextHidingTextChanged;
            }
        }

        private static void OnLostkeyboardTextHidingFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox txtBox = sender as TextBox;
                txtBox.TextChanged -= TextHidingTextChanged;
                string text = GetCompleteText(txtBox);
                string newtext = string.Empty;
                if (text.Length > 7)
                    newtext = text.Substring(0, 3) + "***" + text.Substring(text.Length - 3);
                else if (text.Length > 4)
                    newtext = "***" + text.Substring(text.Length - 3);
                else if (text.Length > 0)
                    newtext = "***";

                if (newtext?.Equals(text) == false)
                {
                    txtBox.Text = newtext;
                }
            }
        }
        #endregion
    }
}

