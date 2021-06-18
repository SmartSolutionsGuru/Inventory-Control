using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers;
using SmartSolutions.InventoryControl.Core.Helpers.WindowHelper;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(BaseViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class BaseViewModel : Conductor<Screen>, INotifyPropertyChanged
    {


        #region Private Member
        ///// <summary>
        ///// The window this view model controls
        ///// </summary>
        protected Window mWindow;

        ///// <summary>
        ///// The window resizer helper that keeps the window size correct in various states
        ///// </summary>
        protected WindowResizer mWindowResizer;

        ///// <summary>
        ///// The margin around the window to allow for a drop shadow
        ///// </summary>
        private Thickness mOuterMarginSize = new Thickness(5);

        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        private int mWindowRadius = 10;

        ///// <summary>
        ///// The last known dock position
        ///// </summary>
        protected WindowDockPosition mDockPosition = WindowDockPosition.Undocked;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BaseViewModel() 
        {
           
        }

        public BaseViewModel(Window window)
        {
            mWindow = window;
            // Listen out for the window resizing
            if (mWindow != null)
            {
                mWindow.StateChanged += (sender, e) =>
                {
                    // Fire off events for all properties that are affected by a resize
                    WindowResized();
                };

                mWindowResizer = new WindowResizer(mWindow);

                // Listen out for dock changes
                mWindowResizer.WindowDockChanged += (dock) =>
                {
                    // Store last position
                    mDockPosition = dock;

                    // Fire off resize events
                    WindowResized();
                };

                // On window being moved/dragged
                mWindowResizer.WindowStartedMove += () =>
                {
                    // Update being moved flag
                    BeingMoved = true;
                };

                // Fix dropping an undocked window at top which should be positioned at the
                // very top of screen
                mWindowResizer.WindowFinishedMove += () =>
                {
                    // Update being moved flag
                    BeingMoved = false;

                    // Check for moved to top of window and not at an edge
                    if (mDockPosition == WindowDockPosition.Undocked && mWindow.Top == mWindowResizer.CurrentScreenSize.Top)
                        // If so, move it to the true top (the border size)
                        mWindow.Top = -OuterMarginSize.Top;
                };
            }
            else
                return;

        }
        #endregion

        #region Public Properties
        private double _WindowMinimumWidth = 1000;
        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth
        {
            get { return _WindowMinimumWidth; }
            set { _WindowMinimumWidth = value; NotifyOfPropertyChange(nameof(WindowMinimumWidth)); }
        }

        private double _WindowMinimumHeight = 725;
        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight
        {
            get { return _WindowMinimumHeight; }
            set { _WindowMinimumHeight = value; NotifyOfPropertyChange(nameof(WindowMinimumHeight)); }
        }
        private bool _BeingMoved;
        ///// <summary>
        ///// True if the window is currently being moved/dragged
        ///// </summary>
        public bool BeingMoved
        {
            get { return _BeingMoved; }
            set { _BeingMoved = value; NotifyOfPropertyChange(nameof(BeingMoved)); }
        }


        /// <summary>
        /// flage that checks that is it loggeed in or not
        /// </summary>
        private bool _IsLoggedIn;

        public bool IsLoggedIn
        {
            get { return _IsLoggedIn; }
            set { _IsLoggedIn = value; NotifyOfPropertyChange(nameof(IsLoggedIn)); }
        }


        private bool m_IsLoading;
        /// <summary>
        /// Base Loading Property true if any thing Loading
        /// </summary>
        public bool IsLoading
        {
            get { return m_IsLoading; }
            set { m_IsLoading = value; NotifyOfPropertyChange(nameof(IsLoading)); }
        }
        private string _LoadingMessage = "Loading...";
        /// <summary>
        /// Friendly Message if Anything Loading
        /// </summary>
        public string LoadingMessage
        {
            get { return _LoadingMessage; }
            set { _LoadingMessage = value; NotifyOfPropertyChange(nameof(LoadingMessage)); }
        }

        ///// <summary>
        ///// True if the window should be borderless because it is docked or maximized
        ///// </summary>
        public bool Borderless => (mWindow?.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked);

        ///// <summary>
        ///// The size of the resize border around the window
        ///// </summary>
        public int ResizeBorder => mWindow?.WindowState == WindowState.Maximized ? 0 : 4;

        ///// <summary>
        ///// The size of the resize border around the window, taking into account the outer margin
        ///// </summary>
        public Thickness ResizeBorderThickness => new Thickness(OuterMarginSize.Left + ResizeBorder,
                                                                OuterMarginSize.Top + ResizeBorder,
                                                                OuterMarginSize.Right + ResizeBorder,
                                                                OuterMarginSize.Bottom + ResizeBorder);

        ///// <summary>
        ///// The padding of the inner content of the main window
        ///// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        ///// <summary>
        ///// The margin around the window to allow for a drop shadow
        ///// </summary>
        public Thickness OuterMarginSize
        {
            // If it is maximized or docked, no border
            get => mWindow?.WindowState == WindowState.Maximized ? mWindowResizer.CurrentMonitorMargin : (Borderless ? new Thickness(0) : mOuterMarginSize);
            set => mOuterMarginSize = value;
        }
        
        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get => Borderless ? 0 : mWindowRadius;
            set => mWindowRadius = value;
        }

        ///// <summary>
        ///// The rectangle border around the window when docked
        ///// </summary>
        public int FlatBorderThickness => Borderless && mWindow.WindowState != WindowState.Maximized ? 1 : 0;

        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);


        private int _TitleHeight = 42;
        ///// <summary>
        ///// The height of the title bar / caption of the window
        ///// </summary>
        public int TitleHeight
        {
            get { return _TitleHeight; }
            set { _TitleHeight = value; NotifyOfPropertyChange(nameof(TitleHeight)); }
        }

        ///// <summary>
        ///// The height of the title bar / caption of the window
        ///// </summary>
        public GridLength TitleHeightGridLength => new GridLength(TitleHeight + ResizeBorder);

        private bool _DimmableOverlayVisible;
        ///// <summary>
        ///// True if we should have a dimmed overlay on the window
        ///// such as when a popup is visible or the window is not focused
        ///// </summary>
        public bool DimmableOverlayVisible
        {
            get { return _DimmableOverlayVisible; }
            set { _DimmableOverlayVisible = value; NotifyOfPropertyChange(nameof(DimmableOverlayVisible)); }
        }

        #endregion

        #region Events
        //public event PropertyChangedEventHandler PropertyChanged;
        //#endregion

        //#region Public Methods
        //public void NotifyOfPropertyChange(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        protected virtual void OnActivated(EventArgs e) { }
        #endregion

        #region Private Helpers
        ///// <summary>
        ///// Gets the current mouse position on the screen
        ///// </summary>
        ///// <returns></returns>
        public Point GetMousePosition()
        {
            return mWindowResizer.GetCursorPosition();
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        protected void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            NotifyOfPropertyChange(nameof(Borderless));
            NotifyOfPropertyChange(nameof(FlatBorderThickness));
            NotifyOfPropertyChange(nameof(ResizeBorderThickness));
            NotifyOfPropertyChange(nameof(OuterMarginSize));
            NotifyOfPropertyChange(nameof(WindowRadius));
            NotifyOfPropertyChange(nameof(WindowCornerRadius));
        }
        #endregion

        #region Command Helpers
        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task RunCommandAsync(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // Lock to ensure single access to check
            lock (updatingFlag)
            {
                // Check if the flag property is true (meaning the function is already running)
                if (updatingFlag.GetPropertyValue())
                    return;

                // Set the property flag to true to indicate we are running
                updatingFlag.SetPropertyValue(true);
            }

            try
            {
                // Run the passed in action
                await action();
            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
            }
        }

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <typeparam name="T">The type the action returns</typeparam>
        /// <returns></returns>
        protected async Task<T> RunCommandAsync<T>(Expression<Func<bool>> updatingFlag, Func<Task<T>> action, T defaultValue = default(T))
        {
            // Lock to ensure single access to check
            lock (updatingFlag)
            {
                // Check if the flag property is true (meaning the function is already running)
                if (updatingFlag.GetPropertyValue())
                    return defaultValue;

                // Set the property flag to true to indicate we are running
                updatingFlag.SetPropertyValue(true);
            }

            try
            {
                // Run the passed in action
                return await action();
            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
            }
        }
        #endregion
    }
}
