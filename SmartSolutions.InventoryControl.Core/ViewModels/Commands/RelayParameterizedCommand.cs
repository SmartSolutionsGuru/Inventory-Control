using System;
using System.Windows.Input;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Commands
{
    class RelayParameterizedCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action<object> mAction;
        private readonly Predicate<object> mCanAction;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        #endregion

        #region Constructor

        public RelayParameterizedCommand(Action<object> action) : this(action, null)
        {

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayParameterizedCommand(Action<object> action,Predicate<object>canAction)
        {
            mAction = action;
            mCanAction = canAction;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if(mCanAction==null)
            return true;
            return mCanAction(parameter);
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            //CommandManager.InvalidateRequerySuggested();
        }
        #endregion
    }
}

