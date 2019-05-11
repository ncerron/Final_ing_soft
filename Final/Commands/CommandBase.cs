using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Final.Commands
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private DispatcherTimer canExecuteChangedEventTimer = null;
        private Func<bool> canExecuteF = null;

        public CommandBase(Func<object, Task> target, Func<bool> canExecuteF)
        {
            thefunction = target;
            this.canExecuteF = canExecuteF;
            canExecuteChangedEventTimer = new DispatcherTimer();
            canExecuteChangedEventTimer.Tick += canExecuteChangedEventTimer_Tick;
            canExecuteChangedEventTimer.Interval = new TimeSpan(0, 0, 1);
            canExecuteChangedEventTimer.Start();
        }

        /// <summary>
        /// Variable que hace referencia a la función/tarea que 
        /// realmente se quiere ejecutar
        /// </summary>
        private Func<object, Task> thefunction;

        public bool CanExecute(object parameter)
        {
            if (canExecuteF == null)
                return true;
            else
                return canExecuteF();
        }

        public void Execute(object parameter)
        {
            thefunction(parameter);
        }

        void canExecuteChangedEventTimer_Tick(object sender, object e)
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
