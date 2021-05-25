using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartSolutions.InventoryControl.UI.Helpers.ControlHelpers
{
    public class ScrollHelper : ItemsControl
    {


        public static ICommand GetNextPageCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(NextPageCommandProperty);
        }

        public static void SetNextPageCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(NextPageCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.RegisterAttached("NextPageCommand", typeof(ICommand), typeof(ScrollHelper), new PropertyMetadata(null));

        public static bool GetEnablePagination(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnablePaginationProperty);
        }

        public static void SetEnablePagination(DependencyObject obj, bool value)
        {
            obj.SetValue(EnablePaginationProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnablePagination.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnablePaginationProperty =
            DependencyProperty.RegisterAttached("EnablePagination", typeof(bool), typeof(ScrollHelper), new PropertyMetadata(false, EnablePaginationChanged));

        private static void EnablePaginationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ItemsControl;
            if (control?.IsLoaded == true)
            {
                OnLoadedUnloaded(control, true);
            }
            else if (control?.IsLoaded == false && e.NewValue != e.OldValue)
            {
                control.Loaded += Control_Loaded;
            }

            if (control != null)
                control.Unloaded += Control_Unloaded;
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl)
            {
                OnLoadedUnloaded(sender as ItemsControl, true);
                (sender as ItemsControl).Loaded -= Control_Loaded;
            }
        }

        private static void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl)
            {
                OnLoadedUnloaded(sender as ItemsControl, false);
                (sender as ItemsControl).Unloaded -= Control_Unloaded;
            }
        }

        private static void OnLoadedUnloaded(ItemsControl control, bool enable)
        {
            //var firstchild = control?.Template?.VisualTree?.FirstChild;
            var scrollviewer = control?.FindChild<ScrollViewer>();
            //var scrollviewer = d as ScrollViewer;
            if (scrollviewer != null)
            {
                if (GetEnablePagination(control) && enable)
                {
                    scrollviewer.Tag = control;
                    scrollviewer.ScrollChanged += Scrollviewer_ScrollChanged;
                }
                else
                {
                    scrollviewer.ScrollChanged -= Scrollviewer_ScrollChanged;
                }
            }
        }

        private static void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollviewer = sender as ScrollViewer;

            if (scrollviewer != null && scrollviewer.CanContentScroll == false && e.VerticalChange > 0 && scrollviewer.VerticalOffset >= scrollviewer.ScrollableHeight - 100)
            {
                var control = scrollviewer.Tag as ItemsControl;
                ICommand command = GetNextPageCommand(control);
                if (command?.CanExecute(null) == true)
                    command?.Execute(null);
                //control?.RaiseEvent(new RoutedEventArgs(ScrollHelper.LoadNextPageEvent, control));
            }
            else if (scrollviewer != null && scrollviewer.CanContentScroll == true && e.VerticalChange > 0 && scrollviewer.VerticalOffset >= scrollviewer.ScrollableHeight - 2)
            {
                var control = scrollviewer.Tag as ItemsControl;
                ICommand command = GetNextPageCommand(control);
                if (command?.CanExecute(null) == true)
                    command?.Execute(null);
                //control?.RaiseEvent(new RoutedEventArgs(ScrollHelper.LoadNextPageEvent, control));
            }
        }

        public static readonly RoutedEvent LoadNextPageEvent = EventManager.RegisterRoutedEvent("LoadNextPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ScrollHelper));

        public static void AddLoadNextPageHandler(DependencyObject o, RoutedEventHandler handler)
        {
            ((ScrollViewer)o).AddHandler(ScrollHelper.LoadNextPageEvent, handler);
        }

        public static void RemoveLoadNextPageHandler(DependencyObject o, RoutedEventHandler handler)
        {
            ((ScrollViewer)o).RemoveHandler(ScrollHelper.LoadNextPageEvent, handler);
        }
    }
}
