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
    class GestionClientesViewModel :CrudVMBase
    {
        public ClienteVM ClienteSeleccionado { get; set; }
        public ObservableCollection<ClienteVM> ListaClientes { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        protected async override void GetData()
        {
            try
            {
                ObservableCollection<ClienteVM> listausuarios = new ObservableCollection<ClienteVM>();
                var clientes = await (from p in db.TBLCLIENTES
                                      orderby p.DNI
                                      select p).ToListAsync();
                foreach (TBLCLIENTES cliente in clientes)
                {
                    listausuarios.Add(new ClienteVM { IsNew = false, ElCliente = cliente });
                }
                ListaClientes = listausuarios;
                RaisePropertyChanged("ListaClientes");
            }
            catch (Exception e)
            {
                if(e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (GetData)", Logs.constantes.EXCEPTION_TYPE);

                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (GetData)", Logs.constantes.EXCEPTION_TYPE);

                }
            }
        }
        protected override void ConfirmarCambios()
        {
            string msg = string.Empty;
            var insertado = (from c in ListaClientes
                             where c.IsNew
                             select c).ToList();
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (ClienteVM c in insertado)
                {
                    db.TBLCLIENTES.Add(c.ElCliente);
                }
                try
                {
                    db.SaveChanges();
                    msg = "Todos los datos guardados";
                }
                catch (Exception e)
                {
                    if(e.InnerException != null)
                    {
                        Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (ConfirmarCambios)", Logs.constantes.EXCEPTION_TYPE);
                    }
                    else
                    {
                        Logs.Logs.EscribirLog(e.Message, ToString() + " (ConfirmarCambios)", Logs.constantes.EXCEPTION_TYPE);
                    }
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
            if (ClienteSeleccionado != null)
            {
                int Existe = ComprobarExistencia();
                if (Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else if (Existe > 0)
                {
                    msg = string.Format("no se puede borrar, tiene {0} mascotas aun registradas", Existe);
                }
                else
                {
                    db.TBLCLIENTES.Remove(ClienteSeleccionado.ElCliente);
                    ListaClientes.Remove(ClienteSeleccionado);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaClientes");
                    msg = "Borrado";
                }
            }
            else
            {
                msg = "Ningun cliente seleccionado";
            }
            MessageBox.Show(msg);
        }
        private int ComprobarExistencia()
        {
            var prod = db.TBLCLIENTES.Find(ClienteSeleccionado.ElCliente.DNI);
            if (prod == null || ClienteSeleccionado.IsNew) //solo lo borra de la base de datos si no es null ni nuevo (IsNew)
            {
                return -1;
            }
            int linesCount = db.Entry(prod)
                               .Collection(p => p.TBLMASCOTAS)
                               .Query()
                               .Count();
            return linesCount;
        }
        public GestionClientesViewModel()
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
