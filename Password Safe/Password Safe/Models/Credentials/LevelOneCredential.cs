using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Credentials
{
    /// <summary>
    /// LevelOneCredential Class.
    /// </summary>
    class LevelOneCredential : Credential
    {
        public LevelOneCredential() {
            Username = "";
            Password = "";
        }

        public LevelOneCredential(LevelOneCredential credential)
        {
            Name = credential.Name;
            Username = credential.Username;
            Password = credential.Password;
        }

        /// <summary>
        /// The credential's id.
        /// </summary>
        [AutoIncrement, PrimaryKey]
        public override int Id { get; set; }
        
        /// <summary>
        /// Name of the credential.
        /// </summary>
        public override string Name { get; set; }
        
        /// <summary>
        /// Username for the credential.
        /// </summary>
        public override string Username { get; set; }
        
        /// <summary>
        /// Password for the credential
        /// </summary>
        public override string Password { get; set; }
    }
}
