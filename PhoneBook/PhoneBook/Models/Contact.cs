using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhoneBook.Models
{
    /// <summary>
    /// Contact Class.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Contact's Unique Identifier.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        /// <summary>
        /// Contact's First Name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Contact's Last Name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Contact's Phone Number.
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Contact's Email Address.
        /// </summary>
        public string EmailAddress { get; set; }
        
        /// <summary>
        /// Contact's Blocked Status.
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Contact's Full Name.
        /// </summary>
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
    }
}
