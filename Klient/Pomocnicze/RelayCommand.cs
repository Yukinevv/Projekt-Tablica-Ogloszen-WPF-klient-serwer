using System;
using System.Windows.Input;

namespace Klient
{
    public class RelayCommand : ICommand
    {
        public Action<object> mAction;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> mAction)
        {
            this.mAction = mAction;
        }   

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            mAction(parameter);
        }
    }
}
