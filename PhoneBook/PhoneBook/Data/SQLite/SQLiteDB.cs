using PhoneBook.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook.Data.SQLite
{
    /// <summary>
    /// SQLiteDB Class.
    /// </summary>
    public class SQLiteDB
    {

        #region Local Fields

        /// <summary>
        /// SQLite Async Connection Object.
        /// </summary>
        private readonly SQLiteAsyncConnection _database;

        #endregion Local Fields

        #region Constructors

        /// <summary>
        /// Constructs a new SQLiteDB instance.
        /// </summary>
        /// <param name="dbPath">Path to where the data is stored.</param>
        public SQLiteDB(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Contact>().Wait();
        }

        #endregion Constructors

        #region CRUD Operations

        #region Read Operations

        /// <summary>
        /// Gets All Contacts from the Database.
        /// </summary>
        /// <returns>All Contacts</returns>
        public Task<List<Contact>> GetAllContactsAsync()
        {
            return _database.Table<Contact>().ToListAsync();
        }

        /// <summary>
        /// Gets a Single Contact with a Matching Id from the Database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Contact</returns>
        public Task<Contact> GetContactByIdAsync(int id)
        {
            return _database.Table<Contact>().Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();
        }

        #endregion Read Operations

        #region Create Operations

        /// <summary>
        /// Creates a Contact in the Database.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>Number of Created Rows.</returns>
        public Task<int> CreateContactAsync(Contact contact)
        {
            return _database.InsertAsync(contact);
        }

        #endregion Create Operations

        #region Update Operations

        /// <summary>
        /// Updates a Contact in the Database.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>Number of Updated Rows.</returns>
        public Task<int> UpdateContactAsync(Contact contact)
        {
            return _database.UpdateAsync(contact);
        }

        /// <summary>
        /// Creates or Updates a Contact in the Database. Creates it if it does not exist, otherwise Updates it.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>Number of Rows Created or Updated.</returns>
        public Task<int> SaveContactAsync(Contact contact)
        {
            return contact.Id == 0 ? CreateContactAsync(contact) : UpdateContactAsync(contact);
        }

        #endregion Update Operations

        #region Delete Operations

        /// <summary>
        /// Deletes a Contact from the Database.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>Number of Deleted Rows.</returns>
        public Task<int> DeleteContactAsync(Contact contact)
        {
            return _database.DeleteAsync(contact);
        }

        /// <summary>
        /// Deletes a Contact by Id from the Database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Number of Deleted Rows.</returns>
        public Task<int> DeleteContactById(int id)
        {
            return _database.DeleteAsync(id);
        }

        #endregion Delete Operations

        #endregion CRUD Operations

    }
}
