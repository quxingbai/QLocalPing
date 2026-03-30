using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace QLocalPing.Helper
{
    public class ActionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<object> _action;
        public ActionCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}
