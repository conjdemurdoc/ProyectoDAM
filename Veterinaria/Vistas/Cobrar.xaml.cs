using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Veterinaria.CodigoDelegado;

namespace Veterinaria.Vistas
{
    /// <summary>
    /// Lógica de interacción para Cobrar.xaml
    /// </summary>
    public partial class Cobrar : UserControl
    {
        CobrarViewModel viewModel;
        public Cobrar()
        {
            InitializeComponent();
            viewModel = new CobrarViewModel();
            DataContext = viewModel;
        }
    }
}
