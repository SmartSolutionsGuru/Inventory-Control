using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartSolutions.InventoryControl.UI.CustomControls
{
    public class NotificationButton : ContentControl
    {

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
                typeof(EventHandler<FunctionEventArgs<int>>), typeof(NotificationButton));

        public event EventHandler<FunctionEventArgs<int>> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(NotificationButton), new PropertyMetadata("0"));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(NotificationButton), new PropertyMetadata(ValueBoxes.Int0Box, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (NotificationButton)d;
            var v = (int)e.NewValue;
            ctl.SetCurrentValue(TextProperty, v <= ctl.Maximum ? v.ToString() : $"{ctl.Maximum}+");
            if (ctl.IsInitialized)
            {
                ctl.RaiseEvent(new FunctionEventArgs<int>(ValueChangedEvent, ctl)
                {
                    Info = v
                });
            }
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(BadgeStatus), typeof(NotificationButton), new PropertyMetadata(default(BadgeStatus)));

        public BadgeStatus Status
        {
            get => (BadgeStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(int), typeof(NotificationButton), new PropertyMetadata(ValueBoxes.Int99Box, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (NotificationButton)d;
            var v = ctl.Value;
            ctl.SetCurrentValue(TextProperty, v <= ctl.Maximum ? v.ToString() : $"{ctl.Maximum}+");
        }

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty BadgeMarginProperty = DependencyProperty.Register(
            "BadgeMargin", typeof(Thickness), typeof(NotificationButton), new PropertyMetadata(default(Thickness)));

        public Thickness BadgeMargin
        {
            get => (Thickness)GetValue(BadgeMarginProperty);
            set => SetValue(BadgeMarginProperty, value);
        }

        public static readonly DependencyProperty ShowBadgeProperty = DependencyProperty.Register(
            "ShowBadge", typeof(bool), typeof(NotificationButton), new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowBadge
        {
            get => (bool)GetValue(ShowBadgeProperty);
            set => SetValue(ShowBadgeProperty, ValueBoxes.BooleanBox(value));
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize) => null;
    }
    public enum BadgeStatus
    {
        Text,
        Dot,
        Processing
    }

}
