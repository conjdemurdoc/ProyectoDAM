using ModeloDatos;
using ModeloDatos.ClasesVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Utils;

namespace Veterinaria.CodigoDelegado
{
    class GestionUsuariosViewModel : CrudVMBase
    {
        public UsuarioVM UsuarioSeleccionado { get; set; }
        public ObservableCollection<UsuarioVM> ListaUsuarios { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        protected async override void GetData()
        {
            try
            {
                ObservableCollection<UsuarioVM> listausuarios = new ObservableCollection<UsuarioVM>();
                var usuarios = await (from p in db.TBLUSUARIOS
                                      orderby p.IDUSUARIO
                                      select p).ToListAsync();
                foreach (TBLUSUARIOS usuario in usuarios)
                {
                    listausuarios.Add(new UsuarioVM { IsNew = false, Usuario = usuario });
                }
                ListaUsuarios = listausuarios;
                RaisePropertyChanged("ListaUsuarios");
            }
            catch (Exception e)
            {
                Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (GetData)", Logs.constantes.EXCEPTION_TYPE);
            }
        }
        protected override void ConfirmarCambios()
        {
            string msg = string.Empty;
            var insertado = (from c in ListaUsuarios
                             where c.IsNew
                             select c).ToList();
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (UsuarioVM c in insertado)
                {
                    db.TBLUSUARIOS.Add(c.Usuario);
                }
                try
                {
                    db.SaveChanges();
                    msg = "Todos los datos guardados";
                }
                catch (Exception e)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (ConfirmarCambios)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
            else
            {
                msg = "Nada que insertar";
            }
            if (msg != string.Empty)
                MessageBox.Show(msg);
        }
        protected override void BorrarActual()
        {
            string msg = string.Empty;
            if (UsuarioSeleccionado != null)
            {
                int Existe = ComprobarExistencia();
                if (Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else
                {
                    db.TBLUSUARIOS.Remove(UsuarioSeleccionado.Usuario);
                    ListaUsuarios.Remove(UsuarioSeleccionado);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaUsuarios");
                    msg = "Borrado";
                }
            }
            else
            {
                msg = "Ningun usuario seleccionado";
            }
            MessageBox.Show(msg);
        }
        private int ComprobarExistencia()
        {
            var prod = db.TBLUSUARIOS.Find(UsuarioSeleccionado.Usuario.IDUSUARIO);
            if (prod == null || UsuarioSeleccionado.IsNew) //solo lo borra de la base de datos si no es null ni nuevo (IsNew)
            {
                return -1;
            }
            return 0;
        }
        public GestionUsuariosViewModel()
           : base()
        {
            BotonesCommand = new CommandBotones<string>(ControlBotones);
            ObservableCollection<DatosBotones> datos = new ObservableCollection<DatosBotones>
            {
               // new CommandVM{ CommandDisplay="Insert", IconGeometry=Application.Current.Resources["InsertIcon"] as Geometry , Message=new CommandMessage{ Command =CommandType.Insert}},
               // new CommandVM{ CommandDisplay="Edit", IconGeometry=Application.Current.Resources["EditIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Edit}},
                new DatosBotones{ Nombre="Borrar", Icono=Application.Current.Resources["DeleteIcon"] as Geometry , Comando=BotonesCommand, CP="Delete"},
                new DatosBotones{ Nombre="Guradar", Icono=Application.Current.Resources["SaveIcon"] as Geometry , Comando=BotonesCommand, CP="Commit" },
                new DatosBotones{ Nombre="Actualizar", Icono=Application.Current.Resources["RefreshIcon"] as Geometry , Comando=BotonesCommand, CP="Refresh" }

            };
            Datos = datos;
            RaisePropertyChanged("Datos");
        }
        public void ControlBotones(string obj)
        {
            switch (obj)
            {
                case "Delete": BorrarActual(); break;
                case "Commit": ConfirmarCambios(); break;
                case "Refresh": GetData(); MessageBox.Show("Actualizado"); break;
            }
        }
    }
}
