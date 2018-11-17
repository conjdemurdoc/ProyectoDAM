using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class ProductoVM :VMBase
    {
        public TBLPRODUCTOS ElProducto { get; set; }
        public ProductoVM()
        {
            ElProducto = new TBLPRODUCTOS();
        }
    }
}
