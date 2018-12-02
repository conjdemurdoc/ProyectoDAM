using ModeloDatos;
using ModeloDatos.ClasesVM;
using NsExcel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Utils;
using System.Globalization;

namespace Veterinaria.CodigoDelegado
{
    class CobrarViewModel : CrudVMBase
    {
        #region variables privadas
        private string _DniCliente { get; set; }
        #endregion
        #region variables publicas
        public ProductoVM ProductoSeleccionado { get; set; }
        public TicketVM TicketSeleccionado { get; set; }
        public string DniCliente
        {
            get
            {
                return _DniCliente;
            }
            set
            {
                _DniCliente = value;
                RaisePropertyChanged("DniCliente");
                ClienteSeleccionado.ElCliente = db.TBLCLIENTES.Find(_DniCliente);
                NombreCompleto = string.Format("{0} {1}", ClienteSeleccionado.ElCliente.NOMBRE, ClienteSeleccionado.ElCliente.APELLIDOS);
                RaisePropertyChanged("ClienteSeleccionado");
                RaisePropertyChanged("NombreCompleto");
                CargarTickets();
            }
        }
        public ClienteVM ClienteSeleccionado { get; set; }
        public string NombreCompleto { get; set; }
        public float TotalTicket { get; set; }

        public ObservableCollection<ProductoVM> ListaProductos { get; set; }
        public ObservableCollection<TicketVM> ListaTickets { get; set; }
        public ObservableCollection<ClienteVM> ListaClientes { get; set; }
        public ObservableCollection<DatosBotones> Datos1 { get; set; }
        public ObservableCollection<DatosBotones> Datos2 { get; set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        #endregion
        private void CrearTicket()
        {
            try
            {
                if (ProductoSeleccionado != null)
                {
                    TBLTICKETS t;
                    if (ListaTickets == null)
                    {
                        ListaTickets = new ObservableCollection<TicketVM>();
                    }
                    t = new TBLTICKETS { CLIENTE = DniCliente, FECHA = DateTime.Now, PENDIENTE = 1, PRODUCTO = ProductoSeleccionado.ElProducto.ID };
                    db.TBLTICKETS.Add(t);
                    ListaTickets.Add(new TicketVM { ElTicket = t, IsNew = false });
                    RaisePropertyChanged("ListaTickets");
                    db.SaveChanges();
                    var coste = ProductoSeleccionado.ElProducto.COSTE;
                    if (coste != null)
                    {
                        TotalTicket += (float)coste;
                        RaisePropertyChanged("TotalTicket");
                    }

                }
                else
                {
                    MessageBox.Show("ningun producto seleccionado");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CrearTicket)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (CrearTicket)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
        }
        private void CargarTickets()
        {
            try
            {
                if (ListaTickets != null && ListaTickets.Count > 0  )
                {
                    ListaTickets.Clear();
                    RaisePropertyChanged("ListaTickets");
                }
                TotalTicket = 0;
                RaisePropertyChanged("TotalTicket");
                ObservableCollection<TicketVM> coleccion = new ObservableCollection<TicketVM>();
                var pendientes = (from a in db.TBLTICKETS
                                  where a.CLIENTE == DniCliente
                                  && a.PENDIENTE != 0
                                  orderby a.ID
                                  select a).ToList();
                if (pendientes.Count > 0)
                {
                    foreach (TBLTICKETS ti in pendientes)
                    {
                        coleccion.Add(new TicketVM { ElTicket = ti, IsNew = false });
                        var coste = db.TBLPRODUCTOS.Find(ti.PRODUCTO).COSTE;
                        if (coste != null)
                        {
                            TotalTicket += (float)coste;
                        }
                    }
                    ListaTickets = coleccion;
                    RaisePropertyChanged("TotalTicket");
                    RaisePropertyChanged("ListaTickets");
                }
                else
                {
                    ListaTickets = new ObservableCollection<TicketVM>();
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CargarTickets)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (CargarTickets)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
        }
        protected override void GetData()
        {
            try
            {
                ObservableCollection<ProductoVM> listaproductos = new ObservableCollection<ProductoVM>();
                var productos = (from p in db.TBLPRODUCTOS
                                 where p.DISPONIBLE != 0
                                 orderby p.ID
                                 select p).ToList();
                foreach (TBLPRODUCTOS articulo in productos)
                {
                    listaproductos.Add(new ProductoVM { IsNew = false, ElProducto = articulo });
                }
                ListaProductos = listaproductos;
                RaisePropertyChanged("ListaArticulos");
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
        protected override void BorrarActual()
        {
            string msg = string.Empty;
            if (TicketSeleccionado != null)
            {
                var precioabajar = db.TBLPRODUCTOS.Find(TicketSeleccionado.ElTicket.PRODUCTO).COSTE;
                if (precioabajar != null)
                {
                    TotalTicket -= (float)precioabajar;
                }
                RaisePropertyChanged("TotalTicket");
                db.TBLTICKETS.Remove(TicketSeleccionado.ElTicket);
                ListaTickets.Remove(TicketSeleccionado);
                db.SaveChanges();
                RaisePropertyChanged("ListaTickets");
                msg = "Borrado";
            }
            else
            {
                msg = "Ningun ticket seleccionado";
            }
            MessageBox.Show(msg);
        }
        protected override void ConfirmarCambios()
        {
            string msg = string.Empty;
            if (ListaTickets.Count > 0)
            {
                var respuesta = MessageBox.Show("Se va a generar una factura de todo lo indicado en la lista, continuar?", "Productos y servicios a cobrar", MessageBoxButton.YesNo);
                if (respuesta == MessageBoxResult.Yes)
                {
                    foreach (TicketVM tick in ListaTickets)
                    {
                        tick.ElTicket.PENDIENTE = 0;
                    }
                    try
                    {
                        db.SaveChanges();
                        msg = "Cobrado correctamente";

                        //PENDIENTE: generar el codigo necesario para la facturacion
                        GenerarFactura();
                        CargarTickets();
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
            else
            {
                msg = "Nada que cobrar";
            }
            if (msg != string.Empty)
                MessageBox.Show(msg);
        }

        private void GenerarFactura()
        {
            string usuarioactual;
            NsExcel.Application objExcel = null;
            NsExcel.Workbook objWorkbook = null;
            try
            {
                usuarioactual = Environment.UserName;
                objExcel = new NsExcel.Application(); ;
                objWorkbook = objExcel.Workbooks.Open("C:\\Users\\" + usuarioactual + "\\Documents\\plantilla.xlsx",
            0, false, 5, "", "", false, NsExcel.XlPlatform.xlWindows, "",
            true, false, 0, true, false, false);
                var objWorksheet = objWorkbook.Worksheets[1];
                
                var hora1 = string.Format("{0:HH:mm}", DateTime.Now);
                var hora2 = string.Format("{0:HH-mm}", DateTime.Now);
                objWorksheet.Cells[11, 3] = NombreCompleto;
                objWorksheet.Cells[11, 6] = DniCliente;
                objWorksheet.Cells[12, 3] = ClienteSeleccionado.ElCliente.CORREO;
                objWorksheet.Cells[12, 6] = ClienteSeleccionado.ElCliente.TELEFONO;
                objWorksheet.Cells[13, 3] = DateTime.Now.Date.ToString("dd/MM/yyyy");
                objWorksheet.Cells[13, 6] = hora1;

                for (int i = 0; i < ListaTickets.Count; i++)
                {
                    var producto = db.TBLPRODUCTOS.Find(ListaTickets[i].ElTicket.PRODUCTO);
                    objWorksheet.Cells[i + 17, 2] = producto.ID;
                    objWorksheet.Cells[i + 17, 3] = producto.NOMBRE;
                    objWorksheet.Cells[i + 17, 6] = 0.21;
                    objWorksheet.Cells[i + 17, 7] = producto.COSTE;
                }
                objWorksheet.Cells[44, 6] = TotalTicket;
                objWorkbook.SaveAs("C:\\Users\\" + usuarioactual + "\\Documents\\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "_" + hora2 + "_" + ClienteSeleccionado.ElCliente.NOMBRE + ".xlsx");
            }
            catch(Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (GenerarFactura)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (GenerarFactura)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
            finally
            {
                
                if (objExcel != null)
                {
                    if(objWorkbook != null)
                    {
                        objWorkbook.Close();
                    }
                    objExcel.Quit();
                }
            }
            
        }

        private void CargarClientes()
        {
            try
            {
                ObservableCollection<ClienteVM> listaclientes = new ObservableCollection<ClienteVM>();
                var listapropietarios = (from p in db.TBLCLIENTES
                                         orderby p.DNI
                                         select p).ToList();
                foreach (TBLCLIENTES a in listapropietarios)
                {
                    listaclientes.Add(new ClienteVM
                    {
                        ElCliente = a,
                        IsNew = false
                    });
                }
                ListaClientes = listaclientes;
                RaisePropertyChanged("ListaClientes");
                ClienteSeleccionado = new ClienteVM();
                DniCliente = ListaClientes[0].ElCliente.DNI;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (CargarPropietarios)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (CargarPropietarios)", Logs.constantes.EXCEPTION_TYPE);
                }
            }
        }
        private void BorrarTodos()
        {
            string msg = string.Empty;
            if (ListaTickets.Count > 0)
            {
                var respuesta = MessageBox.Show("Se va a borrar todos los registros indicados en la lista, continuar?", "Borrar todo", MessageBoxButton.YesNo);
                if (respuesta == MessageBoxResult.Yes)
                {
                    foreach (TicketVM tick in ListaTickets)
                    {
                        db.TBLTICKETS.Remove(tick.ElTicket);
                    }
                    try
                    {
                        ListaTickets.Clear();
                        db.SaveChanges();
                        msg = "Borrado correctamente";
                        RaisePropertyChanged("ListaTickets");
                        TotalTicket = 0;
                        RaisePropertyChanged("TotalTicket");
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
            else
            {
                msg = "Nada que borrar";
            }
            if (msg != string.Empty)
                MessageBox.Show(msg);
        }
        public CobrarViewModel()
            : base()
        {
            BotonesCommand = new CommandBotones<string>(ControlBotones);
            ObservableCollection<DatosBotones> datos = new ObservableCollection<DatosBotones>
            {
                               // new CommandVM{ CommandDisplay="Insert", IconGeometry=Application.Current.Resources["InsertIcon"] as Geometry , Message=new CommandMessage{ Command =CommandType.Insert}},
               // new CommandVM{ CommandDisplay="Edit", IconGeometry=Application.Current.Resources["EditIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Edit}},
                new DatosBotones{ Nombre="Borrar", Icono=Application.Current.Resources["DeleteIcon"] as Geometry , Comando=BotonesCommand, CP="Delete"},
                new DatosBotones{ Nombre="Borrar todo", Icono=Application.Current.Resources["DeleteAllIcon"] as Geometry , Comando=BotonesCommand, CP="DeleteAll" },
                new DatosBotones{ Nombre="Cobrar", Icono=Application.Current.Resources["ConfirmIcon"] as Geometry , Comando=BotonesCommand, CP="Commit" }
            };
            Datos1 = datos;
            RaisePropertyChanged("Datos1");
            datos = new ObservableCollection<DatosBotones>
            {
                               // new CommandVM{ CommandDisplay="Insert", IconGeometry=Application.Current.Resources["InsertIcon"] as Geometry , Message=new CommandMessage{ Command =CommandType.Insert}},
               // new CommandVM{ CommandDisplay="Edit", IconGeometry=Application.Current.Resources["EditIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Edit}},
                new DatosBotones{ Nombre="Añadir al ticket", Icono=Application.Current.Resources["AddIcon"] as Geometry , Comando=BotonesCommand, CP="Add"},
                new DatosBotones{ Nombre="Recargar", Icono=Application.Current.Resources["RefreshIcon"] as Geometry , Comando=BotonesCommand, CP="Refresh"},
            };
            Datos2 = datos;
            RaisePropertyChanged("Datos2");
            CargarClientes();
        }



        public void ControlBotones(string obj)
        {
            switch (obj)
            {
                case "Delete": BorrarActual(); break;
                case "DeleteAll": BorrarTodos(); break;
                case "Commit": ConfirmarCambios(); break;
                case "Add": CrearTicket(); break;
                case "Refresh": GetData(); MessageBox.Show("Actualizado"); break;
            }
        }


    }
}
