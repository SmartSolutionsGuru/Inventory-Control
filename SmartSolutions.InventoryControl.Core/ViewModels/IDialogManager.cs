using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Dialogs;
using System;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    public interface IDialogManager
    {
        /// <summary>
        /// Display The Screen in Dialog Form
        /// </summary>
        /// <param name="dialogModel"></param>
        /// <returns></returns>
        Task ShowDialogAsync(IScreen screen);

        void ShowMessageBox(string message, string title = "Smart Solutions", MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null, string yesText = null, string noText = null, string okText = null, string cancelText = null, bool alignCenter = false);
        Task<MessageBoxOptions> ShowMessageBoxAsync(string message, string title = "Smart Solutions", MessageBoxOptions options = MessageBoxOptions.Ok, string yesText = null, string noText = null, string okText = null, string cancelText = null, bool alignCenter = false);
    }
}
