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
    class VerCitaViewModel : CrudVMBase
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
                                   //where p.ATENDIDA = 0
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
            //PENDIENTE modificar base de datos, añadir "ATENDIDA int default 0"
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (CitaPreviaVM c in insertado)
                {
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
        public VerCitaViewModel()
           : base()
        {
            BotonesCommand = new CommandBotones<string>(ControlBotones);
            ObservableCollection<DatosBotones> datos = new ObservableCollection<DatosBotones>
            {
               // new CommandVM{ CommandDisplay="Insert", IconGeometry=Application.Current.Resources["InsertIcon"] as Geometry , Message=new CommandMessage{ Command =CommandType.Insert}},
               // new CommandVM{ CommandDisplay="Edit", IconGeometry=Application.Current.Resources["EditIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Edit}},
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
                case "Commit": ConfirmarCambios(); break;
                case "Refresh": GetData(); MessageBox.Show("Actualizado"); break;
            }
        }
    }
}
