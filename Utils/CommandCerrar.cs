using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utils
{
    class CommandCerrar : ICommand
    {
        private Action _Accion;

        public CommandCerrar()
        {
        }

        public CommandCerrar(Action accion)
        {
            _Accion = accion;
        }

        public bool CanExecute(object parameter)
        {
            if (_Accion != null)
            {
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            //if (_Accion != null)
            //{
                _Accion?.Invoke(); 
            //}
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
