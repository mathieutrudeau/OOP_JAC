using Password_Safe.Models.Credentials;
using Password_Safe.Models.Database;
using Password_Safe.Models.Lock_Combinations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Password_Safe.Infrastructure
{
    /// <summary>
    /// CredentialService Class.
    /// </summary>
    public class CredentialService
    {

        #region Properties

        /// <summary>
        /// SQLite Database where the Credentials a saved.
        /// </summary>
        private SQLiteContext Database { get; set; }

        /// <summary>
        /// Key used for Encrypting and Decrypting the Credentials.
        /// </summary>
        private string Key { get; set; }

        /// <summary>
        /// Name of the SQLite Database.
        /// </summary>
        private string DatabaseName { get { return "Credential.db3"; } }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new CredentialService instance.
        /// </summary>
        public CredentialService()
        {
            Database = new SQLiteContext(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),DatabaseName));
            
            // Key Basic Value
            Key = "Key";
        }

        #endregion Constructors

        #region Service Operations

        #region Credential 

        /// <summary>
        /// Saves a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Saved.</param>
        public async Task<int> SaveCredential(Credential credential)
        {
            // Encrypt the credential
            credential = EncryptCredential(credential, Key);

            // Save the credential
            await Database.SaveCredentialAsync(credential);

            // Save the security questions if needed
            if (credential is LevelThreeCredential)
            {
                foreach (CustomFields securityQuestion in (credential as LevelThreeCredential).AddedFields)
                {
                    securityQuestion.CredentialId = credential.Id;
                    await Database.SaveSecurityQuestionAsync(securityQuestion);
                }
            }

            return credential.Id;
        }

        /// <summary>
        /// Gets all Credential Objects in the Database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Credential>> GetAllCredentials()
        {
            // List of credentials
            List<Credential> credentials = new List<Credential>();

            // Add all the Questions of Credentials to the list 
            foreach (Credential credential in await Database.GetAllLevelOneCredentialsAsync())
                credentials.Add(DecryptCredential(credential, Key));
            foreach (Credential credential in await Database.GetAllLevelTwoCredentialsAsync())
                credentials.Add(DecryptCredential(credential, Key));
            foreach (Credential credential in await Database.GetAllLevelThreeCredentialsAsync())
                credentials.Add(DecryptCredential(credential, Key));


            return credentials;
        }

        /// <summary>
        /// Gets the Credentials as an ObservableCollection.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Credential>> GetAllObservableCredentials()
        {
            return new ObservableCollection<Credential>(await GetAllCredentials());
        }

        /// <summary>
        /// Gets all SecurityQuestions for a certain Credential.
        /// </summary>
        /// <param name="id">Id of the Credential for the SecurityQuestions to be retrieved.</param>
        /// <returns></returns>
        public async Task<List<CustomFields>> GetCredentialsAddedFields(int id)
        {
            List<CustomFields> customFields = new List<CustomFields>(); 
            
            foreach(CustomFields customField in await Database.GetSecurityQuestionsForCredential(id))
            {
                customField.Type = EncryptionService.SecureDecrypt(customField.Type, Key);
                customField.Info = EncryptionService.SecureDecrypt(customField.Info, Key);

                customFields.Add(customField);
            }
            
            return customFields;
        }

        /// <summary>
        /// Deletes a Credential Object in the Database.
        /// </summary>
        /// <param name="credential">Credential to be Deleted.</param>
        public async Task<int> DeleteCredential(Credential credential)
        {
            // Deletes the Security Questions associated with the credential
            if (credential is LevelThreeCredential)
                foreach (CustomFields securityQuestion in await GetCredentialsAddedFields(credential.Id))
                    await Database.DeleteSecurityQuestionAsync(securityQuestion);

            // Deletes the credential
            return await Database.DeleteCredentialAsync(credential);
        }

        /// <summary>
        /// Delete a Custom Field in the Database
        /// </summary>
        /// <param name="field">Field to be Deleted.</param>
        /// <returns></returns>
        public async Task<int> DeleteCustomField(CustomFields field)
        {
            return await Database.DeleteSecurityQuestionAsync(field);
        }

        #endregion Credential 

        #region Combination

        /// <summary>
        /// Saves a LockCombination Object in the Database.
        /// </summary>
        /// <param name="lockCombination"></param>
        public async void SaveLockCombination(LockCombination lockCombination)
        {
            // A new Key shall be used
            if(Key!=lockCombination.Combination && Key!="Key")
            {
                List<Credential> credentials = await GetAllCredentials();
                Key = lockCombination.Combination;
                foreach (Credential credential in credentials)
                    await SaveCredential(credential);
            }
            else
                Key = lockCombination.Combination;
            
            await Database.SaveLockCombinationAsync(lockCombination);
        }

        /// <summary>
        /// Gets the LockCombination Object from the Database.
        /// </summary>
        /// <returns></returns>
        public async Task<LockCombination> GetLockCombination()
        {
            LockCombination lockCombination =  await Database.GetLockCombinationAsync();

            if (lockCombination != null)
                Key = lockCombination.Combination;

            return lockCombination;
        }

        #endregion Combination

        #region Encryption Operations

        /// <summary>
        /// Encrypts a Credential Object.
        /// </summary>
        /// <param name="credential">Credential to be Encrypted.</param>
        /// <param name="key">Key for encrypting the Credential.</param>
        /// <returns></returns>
        private Credential EncryptCredential(Credential credential, string key)
        {
            credential.Name = EncryptionService.SecureEncrypt(credential.Name, key);
            credential.Username = EncryptionService.SecureEncrypt(credential.Username, key);
            credential.Password = EncryptionService.SecureEncrypt(credential.Password, key);

            if ((credential is LevelTwoCredential) || (credential is LevelThreeCredential))
            {
                (credential as LevelTwoCredential).PhoneNumber = EncryptionService.SecureEncrypt((credential as LevelTwoCredential).PhoneNumber, key);
                (credential as LevelTwoCredential).Email = EncryptionService.SecureEncrypt((credential as LevelTwoCredential).Email, key);

                if (credential is LevelThreeCredential)
                {
                    foreach (CustomFields customField in (credential as LevelThreeCredential).AddedFields)
                    {
                        customField.Type = EncryptionService.SecureEncrypt(customField.Type, key);
                        customField.Info = EncryptionService.SecureEncrypt(customField.Info, key);
                    }
                }
            }

            return credential;
        }

        /// <summary>
        /// Decrypts a Credential Object
        /// </summary>
        /// <param name="credential">Credential to be Decrypted.</param>
        /// <param name="key">Key for decrypting the Credential.</param>
        /// <returns></returns>
        private Credential DecryptCredential(Credential credential, string key)
        {
            credential.Name = EncryptionService.SecureDecrypt(credential.Name, key);
            credential.Username = EncryptionService.SecureDecrypt(credential.Username, key);
            credential.Password = EncryptionService.SecureDecrypt(credential.Password, key);

            if ((credential is LevelTwoCredential) || (credential is LevelThreeCredential))
            {
                (credential as LevelTwoCredential).PhoneNumber = EncryptionService.SecureDecrypt((credential as LevelTwoCredential).PhoneNumber, key);
                (credential as LevelTwoCredential).Email = EncryptionService.SecureDecrypt((credential as LevelTwoCredential).Email, key);

                if (credential is LevelThreeCredential)
                {
                    foreach (CustomFields customField in (credential as LevelThreeCredential).AddedFields)
                    {
                        customField.Type = EncryptionService.SecureDecrypt(customField.Type, key);
                        customField.Info = EncryptionService.SecureDecrypt(customField.Info, key);
                    }
                }
            }

            return credential;
        }

        #endregion Encryption Operations

        #endregion Service Operations

    }
}
