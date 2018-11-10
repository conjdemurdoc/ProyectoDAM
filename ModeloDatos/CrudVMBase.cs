using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ModeloDatos
{
    public class CrudVMBase : NotifyUIBase
    {
        protected VeterinariaEntities db = new VeterinariaEntities();
        
        protected virtual void ConfirmarCambios()
        {
        }
        protected virtual void BorrarActual()
        {
        }
        protected virtual void RefreshData()
        {
            GetData();
            //MessageBox.show( "Data Refreshed" );
        }
        protected virtual void GetData()
        {
        }
        protected CrudVMBase()
        {
            GetData();
        }
    }
}
