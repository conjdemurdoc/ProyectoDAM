﻿using Logs;
using ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Utils;

namespace Veterinaria.CodigoDelegado
{
    class LoginViewModel : CrudVMBase
    {
        public LoginViewModel()
        {
            SalirCommand = new CommandCerrar(Salir);
            BotonesCommand = new CommandBotones<string>(ControlBotones);
        }
        public BitmapImage ImagePath
        {
            get { return new BitmapImage(new Uri("LogoPipo.png", UriKind.Relative)); }
        }
        private string usuario;
        public string Usuario
        {
            get { return usuario; }
            set
            {
                usuario = value;
            }
        }
        public string Password
        {
            get; set;
        }
        public static int NivelAcceso
        {
            get; set;
        }
        public event EventHandler RequestClose;
        public CommandCerrar SalirCommand
        { get; private set; }
        public CommandBotones<string> BotonesCommand { get; private set; }
        private void ControlBotones(string sender)
        {
            switch (sender)
            {
                case "CargarLogin":
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        EntrarPruebas(); return;
                    }
                    Entrar();
                    break;
                case "CerrarLogin":
                    Salir();
                    break;
            }
        }
        private void EntrarPruebas()
        {
            NivelAcceso = 1;
            MainWindow mw = new MainWindow();
            mw.Show();
            Salir();
        }
        public void Entrar()
        {
            try
            {
                var login = db.TBLUSUARIOS.Where(x => x.USUARIO == Usuario && x.PASS == Password).SingleOrDefault();

                if (login != null)
                {
                    NivelAcceso = login.NIVEL;
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    Salir();
                }
                else
                {
                    Logs.Logs.EscribirLog("Entrar", ToString() + ": Login incorrecto (" + Usuario + " --- " + Password + ")", constantes.DEBUG_TYPE, constantes.DEBUG_OUT);
                    MessageBox.Show("Los datos introducidos no son correctos", "Credenciales incorrectas");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Logs.Logs.EscribirLog(e.InnerException.Message + " --- " + e.Message, ToString() + " (Entrar)", Logs.constantes.EXCEPTION_TYPE);
                }
                else
                {
                    Logs.Logs.EscribirLog(e.Message, ToString() + " (Entrar)", Logs.constantes.EXCEPTION_TYPE);
                }
            }

        }
        private void Salir()
        {
            //EventHandler handler = this.RequestClose;
            //if(handler != null)
            //{
            //    handler(this, RoutedEventArgs.Empty);
            //} --> innecesario, la sentencia de abajo cumple la misma funcion

            RequestClose?.Invoke(this, EventArgs.Empty); // --> llamará a RequestClose con los argumentos pasados ("sólo si no es nulo" o "?")
        }
    }
}
