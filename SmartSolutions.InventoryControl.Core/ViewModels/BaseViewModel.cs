using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    public class BaseViewModel : Conductor<Screen>, INotifyPropertyChanged
    {

        //#region Private Member

        ///// <summary>
        ///// The window this view model controls
        ///// </summary>
        //private Window mWindow;

        ///// <summary>
        ///// The window resizer helper that keeps the window size correct in various states
        ///// </summary>
        //private WindowResizer mWindowResizer;

        ///// <summary>
        ///// The margin around the window to allow for a drop shadow
        ///// </summary>
        //private Thickness mOuterMarginSize = new Thickness(5);

        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        //private int mWindowRadius = 10;

        ///// <summary>
        ///// The last known dock position
        ///// </summary>
        //private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;
        ////private readonly IDialogManager _dialogManager;
        ////private readonly string mConnectionString = @"Data Source=C:\Users\sbutt\source\repos\ShortCuts\ShortCuts.UI\Assets\ShortCuts.db;";

        //#endregion

        //#region Constructor
        //public BaseViewModel() { }

        //public BaseViewModel(Window window)
        //{
        //    mWindow = window;
        //    // Listen out for the window resizing
        //    if (mWindow != null)
        //    {
        //        mWindow.StateChanged += (sender, e) =>
        //        {
        //            // Fire off events for all properties that are affected by a resize
        //            WindowResized();
        //        };

        //        mWindowResizer = new WindowResizer(mWindow);

        //        // Listen out for dock changes
        //        mWindowResizer.WindowDockChanged += (dock) =>
        //        {
        //            // Store last position
        //            mDockPosition = dock;

        //            // Fire off resize events
        //            WindowResized();
        //        };

        //        // On window being moved/dragged
        //        mWindowResizer.WindowStartedMove += () =>
        //        {
        //            // Update being moved flag
        //            BeingMoved = true;
        //        };

        //        // Fix dropping an undocked window at top which should be positioned at the
        //        // very top of screen
        //        mWindowResizer.WindowFinishedMove += () =>
        //        {
        //            // Update being moved flag
        //            BeingMoved = false;

        //            // Check for moved to top of window and not at an edge
        //            if (mDockPosition == WindowDockPosition.Undocked && mWindow.Top == mWindowResizer.CurrentScreenSize.Top)
        //                // If so, move it to the true top (the border size)
        //                mWindow.Top = -OuterMarginSize.Top;
        //        };
        //    }
        //    else
        //        return;

        //}
        //#endregion

        //#region Public Properties
        ////public IDialogManager Dialog
        ////{
        ////    get { return _dialogManager; }
        ////}
        ///// <summary>
        ///// The smallest width the window can go to
        ///// </summary>
        //public double WindowMinimumWidth { get; set; } = 1000;

        ///// <summary>
        ///// The smallest height the window can go to
        ///// </summary>
        //public double WindowMinimumHeight { get; set; } = 725;

        ///// <summary>
        ///// True if the window is currently being moved/dragged
        ///// </summary>
        //public bool BeingMoved { get; set; }

        ///// <summary>
        ///// flage that checks that is it loggeed in or not
        ///// </summary>
        //private bool _IsLoggedIn;

        //public bool IsLoggedIn
        //{
        //    get { return _IsLoggedIn; }
        //    set { _IsLoggedIn = value; NotifyOfPropertyChange(nameof(IsLoggedIn)); }
        //}


        //private bool m_IsLoading;
        //public bool IsLoading
        //{
        //    get { return m_IsLoading; }
        //    set { m_IsLoading = value; NotifyOfPropertyChange(nameof(IsLoading)); }
        //}
        ///// <summary>
        ///// True if the window should be borderless because it is docked or maximized
        ///// </summary>
        //public bool Borderless => (mWindow?.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked);

        ///// <summary>
        ///// The size of the resize border around the window
        ///// </summary>
        //public int ResizeBorder => mWindow?.WindowState == WindowState.Maximized ? 0 : 4;

        ///// <summary>
        ///// The size of the resize border around the window, taking into account the outer margin
        ///// </summary>
        //public Thickness ResizeBorderThickness => new Thickness(OuterMarginSize.Left + ResizeBorder,
        //                                                        OuterMarginSize.Top + ResizeBorder,
        //                                                        OuterMarginSize.Right + ResizeBorder,
        //                                                        OuterMarginSize.Bottom + ResizeBorder);

        ///// <summary>
        ///// The padding of the inner content of the main window
        ///// </summary>
        //public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        ///// <summary>
        ///// The margin around the window to allow for a drop shadow
        ///// </summary>
        //public Thickness OuterMarginSize
        //{
        //    // If it is maximized or docked, no border
        //    get => mWindow?.WindowState == WindowState.Maximized ? mWindowResizer.CurrentMonitorMargin : (Borderless ? new Thickness(0) : mOuterMarginSize);
        //    set => mOuterMarginSize = value;
        //}

        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        //public int WindowRadius
        //{
        //    // If it is maximized or docked, no border
        //    get => Borderless ? 0 : mWindowRadius;
        //    set => mWindowRadius = value;
        //}

        ///// <summary>
        ///// The rectangle border around the window when docked
        ///// </summary>
        //public int FlatBorderThickness => Borderless && mWindow.WindowState != WindowState.Maximized ? 1 : 0;

        ///// <summary>
        ///// The radius of the edges of the window
        ///// </summary>
        //public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);

        ///// <summary>
        ///// The height of the title bar / caption of the window
        ///// </summary>
        //public int TitleHeight { get; set; } = 42;
        ///// <summary>
        ///// The height of the title bar / caption of the window
        ///// </summary>
        //public GridLength TitleHeightGridLength => new GridLength(TitleHeight + ResizeBorder);

        //private bool _DimmableOverlayVisible;
        ///// <summary>
        ///// True if we should have a dimmed overlay on the window
        ///// such as when a popup is visible or the window is not focused
        ///// </summary>
        //public bool DimmableOverlayVisible
        //{
        //    get { return _DimmableOverlayVisible; }
        //    set { _DimmableOverlayVisible = value; NotifyOfPropertyChange(nameof(DimmableOverlayVisible)); }
        //}

        //#endregion

        //#region Events
        ////public event PropertyChangedEventHandler PropertyChanged;
        //#endregion

        //#region Public Methods
        ////public void NotifyOfPropertyChange(string propertyName)
        ////{
        ////    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        ////}
        //protected virtual void OnActivated(EventArgs e) { }

        //#endregion

        //#region Private Helpers

        ///// <summary>
        ///// Gets the current mouse position on the screen
        ///// </summary>
        ///// <returns></returns>
        //public Point GetMousePosition()
        //{
        //    return mWindowResizer.GetCursorPosition();
        //}

        ///// <summary>
        ///// If the window resizes to a special position (docked or maximized)
        ///// this will update all required property change events to set the borders and radius values
        ///// </summary>
        //private void WindowResized()
        //{
        //    // Fire off events for all properties that are affected by a resize
        //    NotifyOfPropertyChange(nameof(Borderless));
        //    NotifyOfPropertyChange(nameof(FlatBorderThickness));
        //    NotifyOfPropertyChange(nameof(ResizeBorderThickness));
        //    NotifyOfPropertyChange(nameof(OuterMarginSize));
        //    NotifyOfPropertyChange(nameof(WindowRadius));
        //    NotifyOfPropertyChange(nameof(WindowCornerRadius));
        //}


        //#endregion

    }
}
