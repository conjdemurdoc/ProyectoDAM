using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class CitaPreviaVM : VMBase 
    {
        public TBLCITAPREVIA LaCita { get; set; }
        public CitaPreviaVM()
        {
            LaCita = new TBLCITAPREVIA();
        }
    }
}
