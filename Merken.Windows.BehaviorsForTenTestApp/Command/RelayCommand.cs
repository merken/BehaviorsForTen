using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Merken.Windev.BehaviorsForTenTestApp.Command
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
        private bool canExecuteVal;
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool originalVal = canExecuteVal;
            if (this.canExecute != null)
                canExecuteVal = this.canExecute(parameter);
            else
                canExecuteVal = true;
            if (originalVal != canExecuteVal && CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
            return canExecuteVal;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
