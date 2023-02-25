using System;
using System.Windows.Input;

namespace RadialMenuDemo.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;

        private readonly Func<bool> condition;

        public RelayCommand(Action action)
        {
            this.action = action;
            condition = () => true;
        }

        public RelayCommand(Action action, bool condition)
        {
            this.action = action;
            this.condition = () => condition;
        }

        public RelayCommand(Action action, Func<bool> condition)
        {
            this.action = action;
            this.condition = condition;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return condition.Invoke();
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}