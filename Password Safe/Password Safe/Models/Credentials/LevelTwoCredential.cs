using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Credentials
{
    /// <summary>
    /// LevelTwoCredential Class.
    /// </summary>
    class LevelTwoCredential : LevelOneCredential
    {
        public LevelTwoCredential(): base() {
            Email = "";
            PhoneNumber = "";
        }

        public LevelTwoCredential(LevelOneCredential credential) 
            : base(credential) {
            Email = "";
            PhoneNumber = "";
        }

        public LevelTwoCredential(LevelTwoCredential credential)
            : base(credential)
        {
            Email = credential.Email;
            PhoneNumber = credential.PhoneNumber;
        }

        /// <summary>
        /// Email for the credential.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Phone Number for the credential.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
