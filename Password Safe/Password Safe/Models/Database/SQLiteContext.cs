using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Password_Safe.Infrastructure;
using Password_Safe.Models.Credentials;
using Password_Safe.Models.Lock_Combinations;
using SQLite;

namespace Password_Safe.Models.Database
{
    /// <summary>
    /// SQLiteContext Class.
    /// </summary>
    class SQLiteContext
    {

        #region Properties

        /// <summary>
        /// Connection to the SQLite Database.
        /// </summary>
        private SQLiteAsyncConnection Database { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new SQLiteContext instance.
        /// </summary>
        /// <param name="dbPath">Path to the Database.</param>
        public SQLiteContext(string dbPath)
        {
            Database = new SQLiteAsyncConnection(dbPath);

            Database.BackupAsync(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BackupDB.db3");

            // Credential Tables
            Database.CreateTableAsync<LevelOneCredential>().Wait();
            Database.CreateTableAsync<LevelTwoCredential>().Wait();
            Database.CreateTableAsync<LevelThreeCredential>().Wait();
            Database.CreateTableAsync<CustomFields>().Wait();

            // Combination Table
            Database.CreateTableAsync<LockCombination>().Wait();
        }

        #endregion Constructors

        #region CRUD Operations

        #region Read Operations

        /// <summary>
        /// Gets all the LevelOneCredential Objects.
        /// </summary>
        /// <returns></returns>
        public Task<List<LevelOneCredential>> GetAllLevelOneCredentialsAsync()
        {
            return Database.Table<LevelOneCredential>().ToListAsync();
        }

        /// <summary>
        /// Gets all the LevelTwoCredential Objects.
        /// </summary>
        /// <returns></returns>
        public Task<List<LevelTwoCredential>> GetAllLevelTwoCredentialsAsync()
        {
            return Database.Table<LevelTwoCredential>().ToListAsync();
        }

        /// <summary>
        /// Gets all the LevelThreeCredential Objects.
        /// </summary>
        /// <returns></returns>
        public Task<List<LevelThreeCredential>> GetAllLevelThreeCredentialsAsync()
        {
            return Database.Table<LevelThreeCredential>().ToListAsync();
        }

        /// <summary>
        /// Gets all the SecurityQuestion Objects.
        /// </summary>
        /// <returns></returns>
        public Task<List<CustomFields>> GetAllSecurityQuestionsAsync()
        {
            return Database.Table<CustomFields>().ToListAsync();
        }

        /// <summary>
        /// Gets the security questions belongning to a certain credential
        /// </summary>
        /// <param name="id">id of the credential</param>
        /// <returns></returns>
        public Task<List<CustomFields>> GetSecurityQuestionsForCredential(int id)
        {
            return Database.Table<CustomFields>().Where(s => s.CredentialId.Equals(id)).ToListAsync();
        }

        /// <summary>
        /// Gets the LockCombination Object.
        /// </summary>
        /// <returns></returns>
        public async Task<LockCombination> GetLockCombinationAsync()
        {
            int count = await Database.Table<LockCombination>().CountAsync();

            if (count == 0)
                return null;
            else
            {
                LockCombination lockCombination = await Database.Table<LockCombination>().FirstOrDefaultAsync();
                lockCombination.Combination = EncryptionService.BasicDecrypt(lockCombination.Combination);
                return lockCombination;
            }
        }

        #endregion Read Operations

        #region Create Operations

        /// <summary>
        /// Creates a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Created.</param>
        /// <returns></returns>
        public Task<int> CreateCredentialAsync(Credential credential)
        {
            return Database.InsertAsync(credential);
        }

        /// <summary>
        /// Creates a SecurityQuestion Object in the Database.
        /// </summary>
        /// <param name="securityQuestion">SecurityQuestion to be Created.</param>
        /// <returns></returns>
        public Task<int> CreateSecurityQuestionAsync(CustomFields securityQuestion)
        {
            return Database.InsertAsync(securityQuestion);
        }

        /// <summary>
        /// Creates a LockCombination Object in the Database.
        /// </summary>
        /// <param name="lockCombination">LockCombination to be Created.</param>
        /// <returns></returns>
        public Task<int> CreateLockCombinationAsync(LockCombination lockCombination)
        {
            lockCombination.Combination = EncryptionService.BasicEncrypt(lockCombination.Combination);
            return Database.InsertAsync(lockCombination);
        }

        #endregion Create Operations

        #region Update Operations

        /// <summary>
        /// Updates a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Updated.</param>
        /// <returns></returns>
        public Task<int> UpdateCredentialAsync(Credential credential)
        {
            return Database.UpdateAsync(credential);
        }

        /// <summary>
        /// Updates a SecurityQuestion Object in the Database.
        /// </summary>
        /// <param name="securityQuestion">SecurityQuestion to be Updated.</param>
        /// <returns></returns>
        public Task<int> UpdateSecurityQuestionAsync(CustomFields securityQuestion)
        {
            return Database.UpdateAsync(securityQuestion);
        }

        /// <summary>
        /// Updates the LockCombination Object in the Database.
        /// </summary>
        /// <param name="lockCombination"></param>
        /// <returns></returns>
        public Task<int> UpdateLockCombinationAsync(LockCombination lockCombination)
        {
            lockCombination.Combination = EncryptionService.BasicEncrypt(lockCombination.Combination);
            return Database.UpdateAsync(lockCombination);
        }

        #endregion Update Operations

        #region Save Operations

        /// <summary>
        /// Saves a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Saved.</param>
        /// <returns></returns>
        public Task<int> SaveCredentialAsync(Credential credential)
        {
            return credential.Id == 0 ? CreateCredentialAsync(credential) : UpdateCredentialAsync(credential);
        }

        /// <summary>
        /// Saves a SecurityQuestion Object in the Database.
        /// </summary>
        /// <param name="securityQuestion">SecurityQuestion to be Saved.</param>
        /// <returns></returns>
        public Task<int> SaveSecurityQuestionAsync(CustomFields securityQuestion)
        {
            return securityQuestion.Id == 0 ? CreateSecurityQuestionAsync(securityQuestion) : UpdateSecurityQuestionAsync(securityQuestion);
        }

        /// <summary>
        /// Saves a LockCombination Object in the Database.
        /// </summary>
        /// <param name="lockCombination">LockCombination to be Saved.</param>
        /// <returns></returns>
        public async Task<int> SaveLockCombinationAsync(LockCombination lockCombination)
        {
            return await GetLockCombinationAsync() != null ? await UpdateLockCombinationAsync(lockCombination) : await CreateLockCombinationAsync(lockCombination);
        }

        #endregion Save Operations

        #region Delete Operations

        /// <summary>
        /// Deletes a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Deleted.</param>
        /// <returns></returns>
        public Task<int> DeleteCredentialAsync(Credential credential)
        {
            return Database.DeleteAsync(credential);
        }

        /// <summary>
        /// Deletes a SecurityQuestion Object in the Database.
        /// </summary>
        /// <param name="securityQuestion">SecurityQuestion to be Deleted.</param>
        /// <returns></returns>
        public Task<int> DeleteSecurityQuestionAsync(CustomFields securityQuestion)
        {
            return Database.DeleteAsync(securityQuestion);
        }

        /// <summary>
        /// Deletes a LockCombination Object in the Database.
        /// </summary>
        /// <param name="lockCombination">LockCombination to be Deleted.</param>
        /// <returns></returns>
        public Task<int> DeleteLockCombinationAsync(LockCombination lockCombination)
        {
            return Database.DeleteAsync(lockCombination);
        }

        #endregion Delete Operations

        #endregion CRUD Operations
    }
}
