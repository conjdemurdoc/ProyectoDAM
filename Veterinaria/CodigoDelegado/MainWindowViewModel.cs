using ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Utils;
using Veterinaria.Vistas;

namespace Veterinaria.CodigoDelegado
{
    class MainWindowViewModel : VMBase
    {
        public object VistaContenida
        {
            get; set;
        }
        public CommandBotones<string> NavCommand { get; private set; }
        public int NivelAcceso // --> se puede establece un int a una propiedad booleana (0 es false, el resto es true)
        {
            get; set;
        }
        public MainWindowViewModel()
        {
            NivelAcceso = LoginViewModel.NivelAcceso;
            NavCommand = new CommandBotones<string>(_ControlBotones);
            Path p = new Path
            {
                Data = Application.Current.Resources["PawIcon"] as Geometry,
                Fill = Application.Current.Resources["CasiNegroBrush"] as SolidColorBrush,
                Stretch = Stretch.Uniform,
                Width = 300
            };
            VistaContenida = p;
        }

        private void _ControlBotones(string obj)
        {
            switch (obj)
            {
                case "btnUsuarios":
                    VistaContenida = new GestionUsuarios();
                    RaisePropertyChanged("VistaContenida");
                    break;
                case "btnClientes":
                    VistaContenida = new GestionClientes();
                    RaisePropertyChanged("VistaContenida");
                    break;
                case "btnMascotas":
                    VistaContenida = new GestionMascotas();
                    RaisePropertyChanged("VistaContenida");
                    break;
                case "btnProveedores":
                    VistaContenida = new GestionProveedores();
                    RaisePropertyChanged("VistaContenida");
                    break;
                case "btnArticulos":
                    VistaContenida = new GestionArticulos();
                    RaisePropertyChanged("Vistacontenida");
                    break;
                case "btnServicios":
                    VistaContenida = new GestionServicios();
                    RaisePropertyChanged("Vistacontenida");
                    break;
                case "btnPedirCita":
                    VistaContenida = new PedirCita();
                    RaisePropertyChanged("Vistacontenida");
                    break;
                case "btnVerCita":
                    VistaContenida = new VerCita();
                    RaisePropertyChanged("Vistacontenida");
                    break;
                case "btmCobrar":
                    VistaContenida = new Cobrar();
                    RaisePropertyChanged("Vistacontenida");
                    break;
                default:
                    Console.WriteLine("boton no implementado, date caña julian");
                    break;
            }
        }
    }
}