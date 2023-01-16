using PhoneBook.Data.Services;
using PhoneBook.Data.SQLite;
using PhoneBook.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhoneBook
{
    public partial class App : Application
    {
        /// <summary>
        /// Local ContactsService Property
        /// </summary>
        private static ContactsService service;

        /// <summary>
        /// Service for the Contacts.
        /// </summary>
        public static ContactsService Service
        {
            get
            {
                if (service == null)
                    service = new ContactsService();

                return service;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ContactsPage());
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
