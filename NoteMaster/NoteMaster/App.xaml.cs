using NoteMaster.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteMaster
{
    public partial class App : Application
    {

        private static NotesService noteService;

        public static NotesService NoteService
        {
            get
            {
                if (noteService == null)
                    noteService = new NotesService();

                return noteService;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new NotesPage());
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
