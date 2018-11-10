using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utils 
{
    public class CommandBotones<T> : ICommand
    {
        private Action<T> _AccionRealizar;
        private Func<T, bool> _AccionSePuedeHacer;

        public CommandBotones(Action<T> executeMethod)
        {
            _AccionRealizar = executeMethod;
        }

        public CommandBotones(Action<T> MetodoAccion, Func<T, bool> MetodoSePuede)
        {
            _AccionRealizar = MetodoAccion;
            _AccionSePuedeHacer = MetodoSePuede;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Members

        bool ICommand.CanExecute(object parametro)
        {
            if (_AccionSePuedeHacer != null)
            {
                T tparm = (T)parametro;
                return _AccionSePuedeHacer(tparm);
            }

            if (_AccionRealizar != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parametro)
        {
            //if (_AccionRealizar != null)
            //{
                _AccionRealizar?.Invoke((T)parametro);
            //}
        }
        #endregion
    }
}
