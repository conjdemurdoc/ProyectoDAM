using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class ClienteVM : VMBase
    {
        public TBLCLIENTES ElCliente { get; set; }
        public ClienteVM()
        {
            ElCliente = new TBLCLIENTES();
        }
    }
}
