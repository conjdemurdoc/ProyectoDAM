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
    class GestionArticulosViewModel :CrudVMBase
    {
        public ProductoVM ArticuloSeleccionado { get; set; }
        public ObservableCollection<ProductoVM> ListaArticulos { get; set; }
        public ObservableCollection<ProveedorVM> ListaProveedores { get; set; }
        public ObservableCollection<DatosBotones> Datos { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        protected async override void GetData()
        {
            try
            {
                CargarProveedores();
                ObservableCollection<ProductoVM> listaproductos = new ObservableCollection<ProductoVM>();
                var productos = await (from p in db.TBLPRODUCTOS
                                       where p.TIPO == 0
                                       orderby p.ID
                                       select p).ToListAsync();
                foreach(TBLPRODUCTOS articulo in productos)
                {
                    listaproductos.Add(new ProductoVM { IsNew = false, ElProducto = articulo });
                }
                ListaArticulos = listaproductos;
                RaisePropertyChanged("ListaArticulos");
            }catch (Exception e)
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
            var insertado = (from c in ListaArticulos
                             where c.IsNew
                             select c).ToList();
            if(db.ChangeTracker.HasChanges() || insertado.Count > 0)
            {
                foreach (ProductoVM c in insertado)
                {
                    c.ElProducto.TIPO = 0;
                    db.TBLPRODUCTOS.Add(c.ElProducto);
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
            if(ArticuloSeleccionado != null)
            {
                int Existe = ComprobarExistencia();
                if(Existe < 0)
                {
                    msg = "No se puede borrar porque no se ha insertado en la base de datos";
                }
                else if (Existe > 0)
                {
                    msg = string.Format("no se puede borrar, tiene {0} pedidos aun registrados", Existe);
                }
                else
                {
                    db.TBLPRODUCTOS.Remove(ArticuloSeleccionado.ElProducto);
                    ListaArticulos.Remove(ArticuloSeleccionado);
                    db.SaveChanges();
                    RaisePropertyChanged("ListaArticulos");
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
            var prod = db.TBLPRODUCTOS.Find(ArticuloSeleccionado.ElProducto.ID);
            if (prod == null || ArticuloSeleccionado.IsNew)
            {
                return -1;
            }
            int linesCount = db.Entry(prod)
                               .Collection(p => p.TBLTICKETS)
                               .Query()
                               .Count();
            return linesCount;
        }
        private void CargarProveedores()
        {
            try
            {
                ObservableCollection<ProveedorVM> Listaproveedores = new ObservableCollection<ProveedorVM>();
                var listaproveeodres = (from p in db.TBLPROVEEDORES
                                         orderby p.NIF
                                         select p).ToList();
                foreach (TBLPROVEEDORES a in listaproveeodres)
                {
                    Listaproveedores.Add(new ProveedorVM
                    {
                        ElProveedor = a,
                        IsNew = false
                    });
                }
                ListaProveedores = Listaproveedores;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CargarProveedores)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (CargarProveedores)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
        }
        public GestionArticulosViewModel()
            :base()
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
