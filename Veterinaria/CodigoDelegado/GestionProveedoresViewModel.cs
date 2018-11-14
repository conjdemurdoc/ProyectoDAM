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
    class GestionProveedoresViewModel : CrudVMBase
    {
        public ProveedorVM ProveeodrSeleccionado { get; set; }
        public ObservableCollection<ProveedorVM> ListaProveedores { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        protected async override void GetData()
        {
            try
            {
                ObservableCollection<ProveedorVM> listaproveedores = new ObservableCollection<ProveedorVM>();
                var proveedores = await (from p in db.TBLPROVEEDORES
                                       orderby p.ID
                                       select p).ToListAsync();
                foreach (TBLPROVEEDORES proveedor in proveedores)
                {
                    listaproveedores.Add(new ProveedorVM { IsNew = false, ElProveedor = proveedor });
                }
                ListaProveedores = listaproveedores;
                RaisePropertyChanged("ListaProveedores");
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
            var insertado = (from c in ListaProveedores
                             where c.IsNew
                             select c).ToList();
            if (db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (ProveedorVM c in insertado)
                {
                    db.TBLPROVEEDORES.Add(c.ElProveedor);
                }
                try
                {
                    db.SaveChanges();
                    msg = "Todos los datos guardados";
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
        }
        protected override void BorrarActual()
        {
            string msg = string.Empty;
            if (ProveeodrSeleccionado != null)
            {
                int Existe = ComprobarExistencia();
                if (Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else if (Existe > 0)
                {
                    msg = string.Format("no se puede borrar, tiene {0} pedidos aun registradas", Existe);
                }
                else
                {
                    db.TBLPROVEEDORES.Remove(ProveeodrSeleccionado.ElProveedor);
                    ListaProveedores.Remove(ProveeodrSeleccionado);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaProveedores");
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
            var prod = db.TBLPRODUCTOS.Find(ProveeodrSeleccionado.ElProveedor.ID);
            if (prod == null || ProveeodrSeleccionado.IsNew)
            {
                return -1;
            }
            //TEMPORAL
            return 0;
        }
        public GestionProveedoresViewModel()
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
