using Caliburn.Micro;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    public interface IMessageBox : IScreen
    {
        string Message { get; set; }
        string OkText { get; set; }
        string CancelText { get; set; }
        string YesText { get; set; }
        string NoText { get; set; }
        bool AlignCenter { get; set; }
        MessageBoxOptions Options { get; set; }
        MessageBoxOptions Selection { get; }

        void Ok();
        void Cancel();
        void Yes();
        void No();

        bool WasSelected(MessageBoxOptions option);
    }
}
