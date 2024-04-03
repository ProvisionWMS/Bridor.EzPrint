using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Bridor.EzPrint.Helpers
{
    public class RelayCommand : ICommand
    {
        #region -  Fields readonly  -
        /// <summary>
        /// The action to be executed
        /// </summary>
        private Action<object> execute;
        /// <summary>
        /// The flag to determin if the action can be executed
        /// </summary>
        private readonly Predicate<object> canExecute;
        #endregion

        #region -  Constructors  -
        /// <summary>
        /// Initialize an instances of the command
        /// </summary>
        /// <param name="execute">Action to execute</param>
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        /// <summary>
        /// Initialize an instances of the command
        /// </summary>
        /// <param name="execute">Action to execute</param>
        /// <param name="canExecute">Predicate to determine execution</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }
        #endregion

        #region -  ICommand Members  -
        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            // If a predicate was not send, always allow execution
            return this.canExecute == null ? true : this.canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) {
            // Execute the action
            this.execute(parameter);
        }
        #endregion
    }
}
