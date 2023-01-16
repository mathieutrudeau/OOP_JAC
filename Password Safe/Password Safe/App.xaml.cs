using Password_Safe.Infrastructure;
using Password_Safe.Pages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Password_Safe
{
    /// <summary>
    /// App Class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Local instance of the Serivce.
        /// </summary>
        private static CredentialService service;

        /// <summary>
        /// Service used to interact with the SQLite Database.
        /// </summary>
        public static CredentialService Service
        {
            get
            {
                if (service == null)
                    service = new CredentialService();

                return service;
            }
        }

        /// <summary>
        /// Flag that is used to know if the data needs to be fetched again because it was changed
        /// </summary>
        public static bool Changed { get; set; }

        public App()
        {
            Changed = true;

            InitializeComponent();

            MainPage = new NavigationPage(new SafeLockPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            MainPage = new NavigationPage(new SafeLockPage());
        }
    }
}
