using Caliburn.Micro;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    /// <summary>
    /// Class That Used For Displaying 
    /// Screen in Dialog Form
    /// </summary>
    [Export(typeof(IDialogManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogBaseViewModel : Conductor<IScreen>.Collection.OneActive, IDialogManager
    {
        #region Private Members
        private readonly IWindowManager _windowManager;
        private readonly Func<IMessageBox> createMessageBox;
        #endregion

        #region Constructor
        public DialogBaseViewModel() { }
        [ImportingConstructor]

        public DialogBaseViewModel(IWindowManager windowManager
                                 /*   ,Func<IMessageBox> messageBoxFactory*/)
        {
            _windowManager = windowManager;
            //createMessageBox = messageBoxFactory;
        }
        #endregion

        #region Events
        private System.Threading.ManualResetEvent showDialog_ResetEvent = null;
        #endregion

        #region Public Methods
        public async Task ShowDialogAsync(IScreen dialogModel)
        {
            try
            {
                System.Threading.ManualResetEvent showDialog_ResetEvent = null;
                await Task.Run(() =>
                {
                    dialogModel.Deactivated += (s, e) =>
                    {
                        if (e.WasClosed)
                        {
                            showDialog_ResetEvent?.Set();
                            showDialog_ResetEvent = null;
                        }
                    };
                    ActivateItem(dialogModel);
                    showDialog_ResetEvent = new System.Threading.ManualResetEvent(false);
                    showDialog_ResetEvent.WaitOne();
                });
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public override void ActivateItem(IScreen item)
        {
            base.ActivateItem(item);
        }
        public override void DeactivateItem(IScreen item, bool close)
        {
            base.DeactivateItem(item, close);
            if (ActiveItem == null && Items.Count > 0)
                ActivateItem(Items[Items.Count - 1]);
            else if (ActiveItem != null && ActiveItem.IsActive == false)
                ActiveItem.Activate();
        }

        public void ActivateItem(object item)
        {
            ActiveItem = item as IScreen;

            var child = ActiveItem as IChild;
            if (child != null)
                child.Parent = this;

            if (ActiveItem != null)
                ActiveItem.Activate();

            NotifyOfPropertyChange(() => ActiveItem);
            OnActivationProcessed(ActiveItem, true);
        }

        public void DeActivateItem(object item)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(result =>
                {
                    if (result)
                    {
                        CloseActiveItemCore();
                    }
                });
            }
        }
        #endregion

        #region Internal Methods
        void CloseActiveItemCore()
        {
            var oldItem = ActiveItem;
            ActivateItem(null);
            oldItem.Deactivate(true);
        }

        public void ShowMessageBox(string message, string title = "Smart Solutions", MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null, string yesText = null, string noText = null, string okText = null, string cancelText = null, bool alignCenter = false)
        {
            var box = createMessageBox();

            box.DisplayName = title;
            box.Options = options;
            box.Message = message;
            box.YesText = yesText;
            box.NoText = noText;
            box.OkText = okText;
            box.CancelText = cancelText;

            box.AlignCenter = alignCenter;

            if (callback != null)
                box.Deactivated += delegate { callback(box); };

            ActivateItem(box);
        }

        public async Task<MessageBoxOptions> ShowMessageBoxAsync(string message, string title = "Smart Solutions", MessageBoxOptions options = MessageBoxOptions.Ok, string yesText = null, string noText = null, string okText = null, string cancelText = null, bool alignCenter = false)
        {
            var box = createMessageBox();
            box.DisplayName = title;
            box.Options = options;
            box.Message = message;
            box.YesText = yesText;
            box.NoText = noText;
            box.OkText = okText;
            box.CancelText = cancelText;
            box.AlignCenter = alignCenter;

            box.Deactivated += (sender, args) =>
            {
                if (args.WasClosed)
                {
                    showDialog_ResetEvent?.Set();
                    showDialog_ResetEvent = null;
                }
            };

            await Task.Run(() =>
            {
                //showDialog_ResetEvent?.Set();
                showDialog_ResetEvent?.WaitOne();
                ActivateItem(box);
                showDialog_ResetEvent = new System.Threading.ManualResetEvent(false);
                showDialog_ResetEvent.WaitOne();
            });

            return box.Selection;
        }
        public void Cancel()
        {
            if (ActiveItem is IMessageBox)
                (ActiveItem as IMessageBox)?.Cancel();
            else if (ActiveItem is IScreen)
                (ActiveItem as IScreen)?.TryClose();
        }
        private void Dialog_Deactivated(object sender, DeactivationEventArgs e)
        {
            try
            {
                if (e.WasClosed)
                {
                    if (sender is IScreen)
                        (sender as IScreen).Deactivated -= Dialog_Deactivated;

                    showDialog_ResetEvent?.Set();
                    showDialog_ResetEvent = null;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Properties
        #endregion
    }
}
