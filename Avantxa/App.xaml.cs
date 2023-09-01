using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace Avantxa
{
    public partial class App : Application
    {
        static Base.AvantxaDB database;
        public App()
        {
            InitializeComponent();

            Device.SetFlags(new string[] { "RadioButton_Experimental" });
            MainPage = new NavigationPage(new MainPage());
        }
        public static async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }
        public static Base.AvantxaDB Database
        {
            get
            {
                if (database == null)
                {
                    database = new Base.AvantxaDB();
                }
                return database;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
