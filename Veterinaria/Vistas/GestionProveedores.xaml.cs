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
    /// Lógica de interacción para GestionProveedores.xaml
    /// </summary>
    public partial class GestionProveedores : UserControl
    {
        GestionProveedoresViewModel viewModel;
        public GestionProveedores()
        {
            InitializeComponent();
            viewModel = new GestionProveedoresViewModel();
            DataContext = viewModel;
        }
        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
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
                case Key.S:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        viewModel.ControlBotones("Commit");
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}
