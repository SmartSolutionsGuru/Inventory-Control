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
        ShellViewModel ViewModel { get; set; }
        #endregion

        #region Constructor
        public ShellView()
        {
            InitializeComponent();
            mWindow = this;
            DataContextChanged += OnDataContextChanged;
            ViewModel = new ShellViewModel(this);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as ShellViewModel;
        }
        #endregion

        #region Public Properties
        //public IDialogManager Dialog
        //{
        //    get { return _dialogManager; }
        //}
        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 1000;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 725;

        /// <summary>
        /// True if the window is currently being moved/dragged
        /// </summary>
        public bool BeingMoved { get; set; }

        private bool _IsLoggedIn;
        /// <summary>
        /// flage that checks that is it loggeed in or not
        /// </summary>
        public bool IsLoggedIn
        {
            get { return _IsLoggedIn; }
            set { _IsLoggedIn = value; NotifyOfPropertyChange(nameof(IsLoggedIn)); }
        }

        private bool m_IsLoading;
        public bool IsLoading
        {
            get { return m_IsLoading; }
            set { m_IsLoading = value; NotifyOfPropertyChange(nameof(IsLoading)); }
        }
        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless => (mWindow?.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked);

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder => mWindow?.WindowState == WindowState.Maximized ? 0 : 4;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(OuterMarginSize.Left + ResizeBorder,
                                                                OuterMarginSize.Top + ResizeBorder,
                                                                OuterMarginSize.Right + ResizeBorder,
                                                                OuterMarginSize.Bottom + ResizeBorder);

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSize
        {
            // If it is maximized or docked, no border
            get => mWindow?.WindowState == WindowState.Maximized ? mWindowResizer.CurrentMonitorMargin : (Borderless ? new Thickness(0) : mOuterMarginSize);
            set => mOuterMarginSize = value;
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get => Borderless ? 0 : mWindowRadius;
            set => mWindowRadius = value;
        }

        /// <summary>
        /// The rectangle border around the window when docked
        /// </summary>
        //public int FlatBorderThickness => Borderless && mWindow.WindowState != WindowState.Maximized ? 1 : 0;
        private int _FlatBorderThickness;

        public int FlatBorderThickness
        {
            get
            {
                if (Borderless && mWindow.WindowState != WindowState.Maximized)
                    _FlatBorderThickness = 1;
                else
                    _FlatBorderThickness = 0;
                return _FlatBorderThickness;
            }
            set { _FlatBorderThickness = value; NotifyOfPropertyChange(nameof(FlatBorderThickness)); }
        }


        private CornerRadius _WindowCornerRadius;
        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius
        {
            get { return _WindowCornerRadius = new CornerRadius(WindowRadius) ; }
            set { _WindowCornerRadius  = value; NotifyOfPropertyChange(nameof(WindowCornerRadius)); }
        }


        private int _TitleHeight = 42;
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight
        {
            get { return _TitleHeight; }
            set { _TitleHeight  = value; NotifyOfPropertyChange(nameof(TitleHeight)); }
        }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(TitleHeight + ResizeBorder);

        private bool _DimmableOverlayVisible;
        /// <summary>
        /// True if we should have a dimmed overlay on the window
        /// such as when a popup is visible or the window is not focused
        /// </summary>
        public bool DimmableOverlayVisible
        {
            get { return _DimmableOverlayVisible; }
            set { _DimmableOverlayVisible = value; NotifyOfPropertyChange(nameof(DimmableOverlayVisible)); }
        }

        #endregion

        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The window resizer helper that keeps the window size correct in various states
        /// </summary>
        private WindowResizer mWindowResizer;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private Thickness mOuterMarginSize = new Thickness(5);

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;


        //private readonly IDialogManager _dialogManager;


        #endregion

        private void OnActivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            DimmableOverlayVisible = false;
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            // Hide overlay if we are focused
            DimmableOverlayVisible = true;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var control = sender as ToggleButton;
            ViewModel = DataContext as ShellViewModel;
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
           // ViewModel.IsDisplayWindowShortcuts = control.IsChecked.Value;
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

