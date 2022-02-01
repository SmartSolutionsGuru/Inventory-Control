using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlExtensions
{
    public static class PasswordHelper
    {   
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));

        public static Visibility GetWatermarkIconVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(PasswordIconVisibilityProperty);
        }
        public static void SetPasswordIconVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(PasswordIconVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordIconVisibilityProperty =
            DependencyProperty.RegisterAttached("PasswordIconVisibility", typeof(Visibility), typeof(PasswordHelper), new PropertyMetadata(Visibility.Visible));

        public static Geometry GetPasswordIconPath(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(PasswordIconPathProperty);
        }

        public static void SetPasswordIconPath(DependencyObject obj, Geometry value)
        {
            obj.SetValue(PasswordIconPathProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkIconPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordIconPathProperty =
            DependencyProperty.RegisterAttached("PasswordIconPath", typeof(Geometry), typeof(PasswordHelper), new PropertyMetadata(null));



        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
