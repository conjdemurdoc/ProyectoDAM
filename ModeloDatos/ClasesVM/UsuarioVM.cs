using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos.ClasesVM
{
    public class UsuarioVM : VMBase
    {
        public TBLUSUARIOS Usuario { get; set; }
        public UsuarioVM()
        {
            // Initialise the entity or inserts will fail
            Usuario = new TBLUSUARIOS();
        }
    }
}
