using NoteMaster.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NoteMaster.App;

namespace NoteMaster
{
    /// <summary>
    /// Page where the Notes are listed.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : ContentPage
    {
        #region Constructors

        /// <summary>
        /// Constructs a new NotesPage instance.
        /// </summary>
        public NotesPage()
        {
            InitializeComponent();

            // Show the existing notes
            notesList.ItemsSource = NoteService.GetNotes();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Handler for Deleting a Note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            // Note to be deleted
            Note note = (sender as MenuItem).CommandParameter as Note;

            // Ask user to confirm deletion.
            string result = await DisplayActionSheet("Do you want to delete this Note?", "No","Yes",new string[]{ });

            // If answer is yes, Delete the Note
            if(result=="Yes")
                NoteService.DeleteNote(note);
            
            // Refresh the list of notes.
            notesList.ItemsSource = NoteService.GetNotes();
        }

        /// <summary>
        /// Handler for Editing a Note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Clicked(object sender, EventArgs e)
        {
            // Note to be edited
            Note note = (sender as MenuItem).CommandParameter as Note;
            
            // Go to the Note Page
            Navigation.PushAsync(new NotePage(note));
        }

        /// <summary>
        /// Handler for Adding a Note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Clicked(object sender, EventArgs e)
        {
            // Go to the Note Page
            Navigation.PushAsync(new NotePage());
        }

        /// <summary>
        /// Handler for Refreshing the notes list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notesList_Refreshing(object sender, EventArgs e)
        {
            // Refresh the list of notes
            notesList.ItemsSource = NoteService.GetNotes();
            notesList.EndRefresh();
        }

        /// <summary>
        /// Handler for the Search Bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filter the notes
            notesList.ItemsSource = NoteService.GetFilteredNotes(e.NewTextValue);
        }

        protected override void OnAppearing()
        {
            // Refresh the list of notes.
            notesList.ItemsSource = NoteService.GetNotes();
            base.OnAppearing();
        }

        #endregion Events

    }
}