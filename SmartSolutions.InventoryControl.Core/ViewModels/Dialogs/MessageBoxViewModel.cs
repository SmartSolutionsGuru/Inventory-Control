using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    [Export(typeof(IMessageBox)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxViewModel : Screen, IMessageBox
    {
        #region Properties
        public string OkText { get; set; }
        public string CancelText { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
        public bool AlignCenter { get; set; }
        public string Message { get; set; }
        public MessageBoxOptions Options { get; set; }
        public MessageBoxOptions Selection { get; private set; }
        #endregion
        [ImportingConstructor]
        public MessageBoxViewModel()
        {

        }

        #region Methods
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (Execute.InDesignMode)
            {
                Options = MessageBoxOptions.YesNoCancel | MessageBoxOptions.Ok;
                NotifyOfPropertyChange(nameof(YesVisible));
                NotifyOfPropertyChange(nameof(NoVisible));
                NotifyOfPropertyChange(nameof(OkVisible));
                NotifyOfPropertyChange(nameof(CancelVisible));
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        public bool OkVisible
        {
            get { return IsVisible(MessageBoxOptions.Ok); }
        }
        public bool CancelVisible
        {
            get { return IsVisible(MessageBoxOptions.Cancel); }
        }
        public bool YesVisible
        {
            get { return IsVisible(MessageBoxOptions.Yes); }
        }
        public bool NoVisible
        {
            get { return IsVisible(MessageBoxOptions.No); }
        }
        public void Ok()
        {
            Select(MessageBoxOptions.Ok);
        }
        public void Cancel()
        {
            Select(MessageBoxOptions.Cancel);
        }
        public void Yes()
        {
            Select(MessageBoxOptions.Yes);
        }
        public void No()
        {
            Select(MessageBoxOptions.No);
        }
        public bool WasSelected(MessageBoxOptions option)
        {
            return (Selection & option) == option;
        }
        bool IsVisible(MessageBoxOptions option)
        {
            return (Options & option) == option;
        }
        void Select(MessageBoxOptions option)
        {
            Selection = option;
            TryClose();
        }
        #endregion
    }
}
