using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Credentials
{
    /// <summary>
    /// LevelThreeCredential Class.
    /// </summary>
    class LevelThreeCredential : LevelTwoCredential
    {
        public LevelThreeCredential(): base() {
            AddedFields = new List<CustomFields>();
        }

        public LevelThreeCredential(LevelTwoCredential credential)
            : base(credential) {

            AddedFields = new List<CustomFields>();
        }

        /// <summary>
        /// 
        /// </summary>
        [Ignore]
        public List<CustomFields> AddedFields { get; set; }
    }
}
