using SmartSolutions.InventoryControl.Core.ViewModels;
using SmartSolutions.InventoryControl.UI.Helpers.PageHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using System.ComponentModel;

namespace SmartSolutions.InventoryControl.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : Window,INotifyPropertyChanged
    {
       
        #region Private Members
       private ShellViewModel ViewModel { get; set; }
        #endregion

        #region Constructor
        public ShellView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            ViewModel = new ShellViewModel(this);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as ShellViewModel;
        }
        #endregion

        #region Public Properties

        #endregion

        private void OnActivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            ViewModel.DimmableOverlayVisible = false;
           
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            // Hide overlay if we are focused
            ViewModel.DimmableOverlayVisible = true;
        }

        #region Window States like Maximized etc
        private void ClickMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                return;
            }
            else
                WindowState = WindowState.Maximized;
        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        /// <summary>
        /// Footer text Animation Like name address etc..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FooterText_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBlock;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -(control?.ActualWidth);
            doubleAnimation.To = selectedCanvas?.ActualWidth;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:15"));
            control?.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        private void canMain_Loaded(object sender, RoutedEventArgs e)
        {
            var canvasControl = sender as Canvas;
            selectedCanvas = canvasControl;

        }

        public Canvas selectedCanvas { get; set; }

        private void OnUncheckedToggleButton(object sender, RoutedEventArgs e)
        {
            var control = sender as ToggleButton;
            ViewModel = DataContext as ShellViewModel;
        }

        public void NotifyOfPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        #region Public Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}

