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
    /// Lógica de interacción para GestionClientes.xaml
    /// </summary>
    public partial class GestionClientes : UserControl
    {
        GestionClientesViewModel viewModel;
        public GestionClientes()
        {
            InitializeComponent();
            viewModel = new GestionClientesViewModel();
            DataContext = viewModel;
        }
        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.Key.ToString());
            switch (e.Key)
            {
                case Key.Delete:
                    viewModel.ControlBotones("Delete");
                    e.Handled = true;
                    break;
                case Key.F5:
                    viewModel.ControlBotones("Refresh");
                    e.Handled = true;
                    break;
                case Key.F4:
                    viewModel.ControlBotones("Commit");
                    e.Handled = true;
                    break;
            }
        }
    }
}
