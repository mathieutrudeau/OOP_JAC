using PhoneBook.Data.SQLite;
using PhoneBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Data.Services
{
    /// <summary>
    /// ContactsService Class.
    /// </summary>
    public class ContactsService
    {

        #region Properties

        /// <summary>
        /// Local Database instance.
        /// </summary>
        private SQLiteDB Database { get; set; }

        /// <summary>
        /// Tracks if the Database has been Modified.
        /// </summary>
        private bool IsModified { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new ContactsService instance.
        /// </summary>
        public ContactsService()
        {
            IsModified = true;
            // Initialize the local database instance
            Database = new SQLiteDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Contact.db3"));
        }

        #endregion Constructors

        #region Operations

        /// <summary>
        /// Gets all Contacts.
        /// </summary>
        /// <returns>All Contacts</returns>
        public ObservableCollection<Contact> GetAllContacts()
        {
            IsModified = false;
            return new ObservableCollection<Contact>(Database.GetAllContactsAsync().Result);
        }

        /// <summary>
        /// Saves a Contact.
        /// </summary>
        /// <param name="contact">Contact Object to be Created or Updated.</param>
        public async Task<int> SaveContact(Contact contact)
        {
            IsModified = true;
            return await Database.SaveContactAsync(contact);
        }

        /// <summary>
        /// Deletes a Contact.
        /// </summary>
        /// <param name="contact">Contact Object to be Deleted.</param>
        public async Task<int> DeleteContact(Contact contact)
        {
            IsModified = true;
            return await Database.DeleteContactAsync(contact);
        }

        /// <summary>
        /// Checks if any Data has been modified.
        /// </summary>
        /// <returns>True if Data has been modified, otherwise false.</returns>
        public bool IsReloadNeeded()
        {
            return IsModified;
        }

        #endregion Operations

    }
}
