using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rg.Plugins.Popup.Extensions;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Parsing;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class PDFViewer : ContentPage
    {
        MemoryStream FileStream { get; set; } = new MemoryStream();
        readonly string jwt, rol;
        readonly int id = 0, donde = 0, UHId = 0;

        public PDFViewer(string Token, int Recid, string tipo, int Donde, int UH)
        {
            InitializeComponent();
            jwt = Token;
            id = Recid;
            donde = Donde;
            rol = tipo;
            UHId = UH;

            Title = "PDF";

            if (tipo == "C")
                BtnFactura.IsVisible = true;
            else
                BtnModificar.IsVisible = true;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var Recibo = await Constants.ObtenerReciboId(jwt, id);

            if (Recibo == null)
            {
                var pop = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else
            {
                foreach (var item in Recibo)
                {
                    FileStream = new MemoryStream(item.RecArchivo);
                    
                    if (item.RecConcepto.Trim() != "Renta")
                        BtnFactura.IsVisible = false;
                }

                try
                {
                    pdfViewerControl.LoadDocument(FileStream);
                }
                catch(Exception es)
                {
                    Console.WriteLine(es);
                }
            }
        }

        protected void Volver(object sender, EventArgs e)
        {
            //Se identifica de que pagina viene la informacion para visualizar el recibo
            //Donde = 0, indica que proviene de la página RecibosPagos
            //Donde = 1, indica que proviene de la página AgregaRecibos
            //Esto se hace para remover la página anterior del stack de páginas y poder regresar directo a la página de RecibosPagos
            if (donde == 1 || donde == 2)
            {
                var page = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "Recibo");
                if (page != null)
                {
                    Navigation.RemovePage(page);
                }
                if(donde == 2)
                {
                    var page1 = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "PDF");
                    if (page1 != null)
                    {
                        Navigation.RemovePage(page1);
                    }
                }
            }
            Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            if (donde == 1 || donde == 2)
            {
                var page = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "Recibo");
                if (page != null)
                {
                    Navigation.RemovePage(page);
                }
                if (donde == 2)
                {
                    var page1 = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "PDF");
                    if (page1 != null)
                    {
                        Navigation.RemovePage(page1);
                    }
                }
            }

            return base.OnBackButtonPressed();
        }

        protected void Factura(object sender, EventArgs e)
        {
            string filePath = DependencyService.Get<ISave>().Save(FileStream);

            Navigation.PushAsync(new ReqFactura(filePath));
        }

        protected void Modifica(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregaRecibo(UHId, jwt, rol, id));
        }
    }
}
