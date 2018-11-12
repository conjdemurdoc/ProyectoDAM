using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class ProveedorVM :VMBase
    {
        public TBLPROVEEDORES ElProveedor { get; set; }
        public ProveedorVM()
        {
            ElProveedor = new TBLPROVEEDORES();
        }
    }
}
