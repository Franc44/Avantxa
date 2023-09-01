using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Avantxa.Vistas;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        TablaUsuario sesion = new TablaUsuario();
        public static string User = "";

        public MainPage()
        {
            InitializeComponent();
            Load();
        }
        async void Load()
        {
            //await Task.Delay(2000);
            
            sesion = await App.Database.GetItemsAsync();
            
            if (sesion == null)
            {
                await Navigation.PushAsync(new Inicio());
            }
            else
            {
                User = sesion.Usuario;
                string Token = await Constants.RefreshToken();
                if (string.IsNullOrEmpty(Token))
                {
                    var p1 = new MessageBox("Error", "No se ha podido conectar con el servidor o su teléfono celular tiene problemas para conectarse a internet, intentelo más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    Environment.Exit(0);
                }
                else
                {
                    string tipo = sesion.Tipo.Trim();
                    TablaUsuario Refrescar = new TablaUsuario
                    {
                        Usuario = sesion.Usuario,
                        Nombre = sesion.Nombre,
                        Tipo = sesion.Tipo,
                        UHid = sesion.UHid,
                        Departamento = sesion.Departamento.Trim(),
                        Token = Token
                    };
                    await App.Database.DeleteItemAsync();
                    await App.Database.SaveItemAsync(Refrescar);

                    if (tipo == "S")
                        Application.Current.MainPage = new NavigationPage(new MenuSU(Refrescar));
                    else
                        Application.Current.MainPage = new NavigationPage(new Menu());
    
                }                
            }

        }

    }
}
/*
foreach (TablaUsuario usu in sesion)
{ 
    //Obtencion de mensajes
    int result = await Constants.Mensajes(usu.Usuario, usu.Nombre, usu.UHid.ToString(), usu.Tipo, usu.Departamento, Token);
    if (result == 0)
    {
        await DisplayAlert("Error", "Ocurrio un error al conectar, por favor verifique que su dispositivo este conectado a internet.", "Aceptar");
    }
}*/