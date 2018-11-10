using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Utils
{
    public class DatosBotones
    {
        public string Nombre { get; set; }
        public Geometry Icono { get; set; }
        public CommandBotones<string> Comando { get; set; }
        public string CP { get; set; }
    }
}
