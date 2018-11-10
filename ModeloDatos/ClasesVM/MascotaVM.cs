using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class MascotaVM : VMBase
    {
        public TBLMASCOTAS LaMascota { get; set; }
        public MascotaVM()
        {
            LaMascota = new TBLMASCOTAS();
        }
    }
}
