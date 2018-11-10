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
using System.Windows.Shapes;
using Veterinaria.CodigoDelegado;

namespace Veterinaria.Vistas
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginViewModel viewModel;
        public Login()
        {
            InitializeComponent();
            viewModel = new LoginViewModel();
            viewModel.RequestClose += delegate
            {
                Close();
            };//new EventHandler(CerrarLogin); --> no es necesario un metodo en la vista si no lo vas a llamar desde aquí, se puede generar metodos sin nombre que cumplan la funcion necesitada
            DataContext = viewModel;
            txtUsuario.Focus();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = (sender as PasswordBox).Password;
            //((dynamic)DataContext).Password = ((PasswordBox)sender).Password;  --> si no tienes declarado el viewmodel en la clase, el datacontext dará error aunque lo enlaces en el constructor, para eso es el "dynamic"
        }
    }
}
