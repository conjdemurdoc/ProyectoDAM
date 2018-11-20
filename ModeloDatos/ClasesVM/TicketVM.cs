using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class TicketVM : VMBase 
    {
        public TBLTICKETS ElTicket { get; set; }
        public TicketVM()
        {
            ElTicket = new TBLTICKETS();
        }
    }
}
