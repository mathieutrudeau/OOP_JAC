using NoteMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NoteMaster.App;

namespace NoteMaster
{
    /// <summary>
    /// Note Page Class.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : ContentPage
    {

        #region Local Fields

        /// <summary>
        /// Note to be Edited.
        /// </summary>
        private Note editNote = null;

        #endregion Local Fields

        #region Constructors

        /// <summary>
        /// Constructs a new NotePage instance.
        /// </summary>
        public NotePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs a new NotePage instance with a Note object Displayed.
        /// </summary>
        /// <param name="note"></param>
        public NotePage(Note note)
        {
            InitializeComponent();

            // Save the note
            editNote = note;

            // Set the displayed text
            title.Text = editNote.Title;
            body.Text = editNote.Body;
        }

        #endregion Constructors

        #region Handlers

        protected override void OnDisappearing()
        {
            // Remove all the leading and trailing spaces from the title
            title.Text = title.Text.Trim();

            // Create the Note Object
            Note newNote = new Note
            {
                Title = title.Text,
                Body = body.Text
            };

            // Set a title if there is none
            if (string.IsNullOrEmpty(newNote.Title) && !string.IsNullOrEmpty(newNote.Body))
                newNote.Title = Note.DEFAULT_TITLE;


            if (!string.IsNullOrEmpty(newNote.Title) && !string.IsNullOrEmpty(newNote.Body))
            {
                // Editing an existing note
                if (editNote != null)
                {
                    // The Title Changed
                    if (newNote.Title != editNote.Title)
                    {
                        // Replace the Old file with a new one
                        NoteService.DeleteNote(editNote);
                        NoteService.CreateNote(newNote);
                    }
                    // Simply Edit the content of the existing file
                    else
                        NoteService.EditNote(newNote);
                }
                // Creating a new note
                else
                    NoteService.CreateNote(newNote);
            }

                base.OnDisappearing();
        }

        #endregion Handlers
    }
}