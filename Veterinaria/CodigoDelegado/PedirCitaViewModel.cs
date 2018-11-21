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
    class PedirCitaViewModel :CrudVMBase 
    {
        public CitaPreviaVM CitaSeleccionada { get; set; }
        public ObservableCollection<CitaPreviaVM> ListaCitas { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        public ObservableCollection<ClienteVM> ListaClientes { get; set; }
        protected async override void GetData()
        {
            try
            {
                CargarClientes();
                ObservableCollection<CitaPreviaVM> Listacitas = new ObservableCollection<CitaPreviaVM>();
                var citas = await (from p in db.TBLCITAPREVIA
                                      orderby p.ID
                                      select p).ToListAsync();
                foreach (TBLCITAPREVIA cita in citas)
                {
                    Listacitas.Add(new CitaPreviaVM { IsNew = false, LaCita = cita });
                }
                ListaCitas = Listacitas;
                RaisePropertyChanged("ListaCitas");
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
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
            var insertado = (from c in ListaCitas
                             where c.IsNew
                             select c).ToList();
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (CitaPreviaVM c in insertado)
                {
                    c.LaCita.FECHAREALIZADA = DateTime.Now;
                    db.TBLCITAPREVIA.Add(c.LaCita);
                }
                try
                {
                    db.SaveChanges();
                    msg = "Todos los datos guardados";
                    GetData();
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
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
            if (CitaSeleccionada != null)
            {
                int Existe = ComprobarExistencia();
                if (Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else
                {
                    db.TBLCITAPREVIA.Remove(CitaSeleccionada.LaCita);
                    ListaCitas.Remove(CitaSeleccionada);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaCitas");
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
            var prod = db.TBLCITAPREVIA.Find(CitaSeleccionada.LaCita.ID);
            if (prod == null || CitaSeleccionada.IsNew) //solo lo borra de la base de datos si no es null ni nuevo (IsNew)
            {
                return -1;
            }
            return 0;
        }
        private void CargarClientes()
        {
            try
            {
                ObservableCollection<ClienteVM> listaclientes = new ObservableCollection<ClienteVM>();
                var clientes = (from p in db.TBLCLIENTES
                                         orderby p.DNI
                                         select p).ToList();
                foreach (TBLCLIENTES a in clientes)
                {
                    listaclientes.Add(new ClienteVM
                    {
                        ElCliente = a,
                        IsNew = false
                    });
                }
                ListaClientes = listaclientes;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CargarClientes)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (CargarClientes)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
        }
        public PedirCitaViewModel()
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
