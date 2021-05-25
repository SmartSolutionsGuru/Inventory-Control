using Caliburn.Micro;
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
    }
}
