using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfApp1.Commands
{
    public class Command : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;

        public Command(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
