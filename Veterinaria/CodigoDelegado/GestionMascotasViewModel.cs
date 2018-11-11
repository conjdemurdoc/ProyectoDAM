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
    class GestionMascotasViewModel :CrudVMBase
    {
        public MascotaVM MascotaSeleccionada { get; set; }
        public ObservableCollection<MascotaVM> ListaMascotas { get; set; }
        public ObservableCollection<ClienteVM> ListaPropietarios { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        public string DniPropietario { get; set; }
        protected async override void GetData()
        {
            try
            {
                CargarPropietarios();
                ObservableCollection<MascotaVM> listamascotas = new ObservableCollection<MascotaVM>();
                var mascotas = await (from p in db.TBLMASCOTAS
                                      orderby p.IDMASCOTA
                                      select p).ToListAsync();
                foreach (TBLMASCOTAS mascota in mascotas)
                {
                    listamascotas.Add(new MascotaVM
                    {
                        IsNew = false,
                        LaMascota = mascota
                    });
                }
                ListaMascotas = listamascotas;
                RaisePropertyChanged("ListaMascotas");
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
            var insertado = (from c in ListaMascotas
                             where c.IsNew
                             select c).ToList();
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (MascotaVM c in insertado)
                {
                    //var a = db.TBLCLIENTES.Where(x => x.NOMBRE == c.PropietarioSeleccionado.ElCliente.NOMBRE);
                    //c.LaMascota.PROPIETARIO = c.PropietarioSeleccionado.ElCliente.DNI;
                    db.TBLMASCOTAS.Add(c.LaMascota);
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
            if (MascotaSeleccionada != null)
            {
                int Existe = ComprobarExistencia();
                if (Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else
                {
                    db.TBLMASCOTAS.Remove(MascotaSeleccionada.LaMascota);
                    ListaMascotas.Remove(MascotaSeleccionada);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaMascotas");
                    msg = "Borrado";
                }
            }
            else
            {
                msg = "Ninguna mascota seleccionado";
            }
            MessageBox.Show(msg);
        }
        private int ComprobarExistencia()
        {
            var prod = db.TBLCLIENTES.Find(MascotaSeleccionada.LaMascota.IDMASCOTA);
            if (prod == null || MascotaSeleccionada.IsNew) //solo lo borra de la base de datos si no es null ni nuevo (IsNew)
            {
                return -1;
            }
            return 0;
        }
        private void CargarPropietarios()
        {
            try
            {
                ObservableCollection<ClienteVM> Listapropietarios = new ObservableCollection<ClienteVM>();
                var listapropietarios = (from p in db.TBLCLIENTES
                                         orderby p.DNI
                                         select p).ToList();
                foreach (TBLCLIENTES a in listapropietarios)
                {
                    Listapropietarios.Add(new ClienteVM
                    {
                        ElCliente = a,
                        IsNew = false
                    });
                }
                ListaPropietarios = Listapropietarios;
            }
            catch (Exception e)
            {
                Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CargarPropietarios)", Logs.constantes.EXCEPTION_TYPE);
            }
        }
        public GestionMascotasViewModel()
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
