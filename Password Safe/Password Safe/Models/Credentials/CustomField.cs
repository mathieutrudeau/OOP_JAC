using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Credentials
{
    /// <summary>
    /// CustomField Class.
    /// </summary>
    public class CustomFields
    {
        /// <summary>
        /// The SecurityQuestion's id.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        /// <summary>
        /// Id of the credential to which the CustomField belongs.
        /// </summary>
        public int CredentialId { get; set; }
        
        /// <summary>
        /// Custom Field's Type question.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Custom Field's Info.
        /// </summary>
        public string Info { get; set; }

        public override string ToString()
        {
            return $"- {Type}:{Info}";
        }
    }
}
