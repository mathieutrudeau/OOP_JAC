   using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NoteMaster.Models
{
    /// <summary>
    /// Service for making data from the Note Class persistant.
    /// </summary>
    public class NotesService
    {
        #region Constructors

        /// <summary>
        /// Constructs a new NotesService instance.
        /// </summary>
        public NotesService()
        {
            NoteIO = new NoteIO();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Note File IO handler class.
        /// </summary>
        private NoteIO NoteIO { get; set; }

        #endregion Properties

        #region Service Operations

        /// <summary>
        /// Gets all the Notes.
        /// </summary>
        /// <returns>All existing Notes.</returns>
        public ObservableCollection<Note> GetNotes()
        {
            return new ObservableCollection<Note>(NoteIO.RetrieveAllNotes());
        }

        /// <summary>
        /// Gets all the Notes that have a Title that contains the filter.
        /// </summary>
        /// <param name="filter">Substring that the Title must contain.</param>
        /// <returns>All Notes that have a Title that contains the filter.</returns>
        public ObservableCollection<Note> GetFilteredNotes(string filter)
        {
            return string.IsNullOrEmpty(filter) ? 
                GetNotes() : 
                new ObservableCollection<Note>(NoteIO.RetrieveAllNotes().Where(n => n.Title.ToLower().Contains(filter.ToLower())));
        }

        /// <summary>
        /// Creates a Note.
        /// </summary>
        /// <param name="note">Note Object to Create.</param>
        public void CreateNote(Note note)
        {
            NoteIO.SaveNewNote(note);
        }

        /// <summary>
        /// Edits a Note.
        /// </summary>
        /// <param name="note">Note Object to Edit.</param>
        public void EditNote(Note note)
        {
            NoteIO.SaveEditedNote(note);
        }

        /// <summary>
        /// Deletes a Note.
        /// </summary>
        /// <param name="note">Note Object to Delete.</param>
        public void DeleteNote(Note note)
        {
            NoteIO.DeleteNote(note);
        }

        #endregion Service Operations
    }
}
