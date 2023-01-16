using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NoteMaster.Models
{
    /// <summary>
    /// Handles the File IO Operations for the Note Class.
    /// </summary>
    class NoteIO
    {

        #region Constructors

        /// <summary>
        /// Constructs a new NoteIO instance.
        /// </summary>
        public NoteIO()
        {
            FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Title of the current Note.
        /// </summary>
        private string FileName { get; set; }
        
        /// <summary>
        /// Path of the folder where the Notes are saved.
        /// </summary>
        private string FolderPath { get; set; }
        
        /// <summary>
        /// Full path to the where the current Note is saved.
        /// </summary>
        private string FileLocation { get { return Path.Combine(FolderPath,FileName); } }

        #endregion Properties

        #region CRUD Operations

        /// <summary>
        /// Saves a new Note.
        /// </summary>
        /// <param name="note">Note object to Save as a new Note.</param>
        public void SaveNewNote(Note note)
        {
            // Save the Note Title has the FileName
            FileName = note.Title;

            // Validate the file name
            ValidateFileName();

            // Save the Note to a File
            File.AppendAllText(FileLocation, note.Body + "\n");
        }

        /// <summary>
        /// Updates an existing Note.
        /// </summary>
        /// <param name="note">Existing Note object to be Updated.</param>
        public void SaveEditedNote(Note note)
        {
            // Save the Note Title has the FileName
            FileName = note.Title;

            // Save the Note to a File
            File.WriteAllText(FileLocation, note.Body + "\n");
        }

        /// <summary>
        /// Retrieves all the Notes that are currently saved.
        /// </summary>
        /// <returns>All saved Notes.</returns>
        public List<Note> RetrieveAllNotes()
        {
            // List where the Note objects will be stored
            List<Note> notes = new List<Note>();
            // Locations of all the Files that are Notes
            string[] fileLocations = Directory.GetFiles(FolderPath);

            // Get the Note object for all file locations
            foreach(string fileLocation in fileLocations)
            {
                // Save the Note FileName
                FileName = fileLocation.Split('/').Last();
                // Add the Note to the List
                notes.Add(new Note { Title=FileName, Body = File.ReadAllText(FileLocation)});
            }
            // Return the List of Notes
            return notes;
        }

        /// <summary>
        /// Deletes an existing Note.
        /// </summary>
        /// <param name="note">Note object to be deleted.</param>
        public void DeleteNote(Note note)
        {
            // Save the Note Title has the FileName
            FileName = note.Title;

            // Delete the File where the Note is Saved.
            File.Delete(FileLocation);
        }

        #endregion CRUD Operations

        #region Helper Methods

        /// <summary>
        /// Sets the FileName value to a unique name.
        /// </summary>
        /// <param name="suffix">Suffix that is added at the end of the FileName. Should start with a value of 0.</param>
        /// <returns>True if the FileName is unique, otherwise false.</returns>
        private bool ValidateFileName(int suffix = 0)
        {
            // File Name local value
            string fileName;

            // Add the suffix to the filename if needed
            if (suffix != 0)
                fileName = FileName + suffix;
            else
                fileName = FileName;

            // Get the file location of the new file
            string fileLocation = Path.Combine(FolderPath,fileName);

            // Add to suffix if the file name exists
            if (Directory.GetFiles(FolderPath).Contains(fileLocation))
                return ValidateFileName((suffix + 1));
            else 
            {
                FileName = fileName;
                return true;
            }
        }

        #endregion Helper Methods
    }
}
