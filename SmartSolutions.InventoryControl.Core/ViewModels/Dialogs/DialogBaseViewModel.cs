using Caliburn.Micro;
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
        private readonly IScreen _dialog;
        #endregion

        #region Constructor
        public DialogBaseViewModel() { }

        [ImportingConstructor]
        public DialogBaseViewModel(IScreen dlg)
        {
            _dialog = dlg;
        }
        #endregion

        #region Events
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

        public void DeactivateItem(object item, bool close)
        {
            var guard = item as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(r =>
                {
                    if (r)
                        CloseActiveItemCore();
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

        #endregion

        #region Properties
        #endregion
    }
}
