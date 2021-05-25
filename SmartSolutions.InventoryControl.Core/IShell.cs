using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels;

namespace SmartSolutions.InventoryControl.Core
{
    public interface IShell :IConductor
    {
        IDialogManager Dialog { get; set; }
    }
}
