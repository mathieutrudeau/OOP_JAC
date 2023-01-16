using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Credentials
{
    public abstract class Credential
    {
        [PrimaryKey, AutoIncrement]
        public abstract int Id { get; set; }

        public abstract string Name { get; set; }

        public abstract string Username { get; set; }

        public abstract string Password { get; set; }
    }
}
