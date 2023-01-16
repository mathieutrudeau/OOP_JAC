using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Password_Safe.Models.Lock_Combinations
{
    /// <summary>
    /// LockCombination Class.
    /// </summary>
    public class LockCombination
    {
        /// <summary>
        /// Primary Key Id.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Combination to the Lock.
        /// </summary>
        public string Combination { get; set; }

        /// <summary>
        /// Combination Option used.
        /// </summary>
        public int Option { get; set; }

        /// <summary>
        /// Amount of attempts at entering a combination until the safe gets locked.
        /// </summary>
        public int AmountOfTries { get; set; }

        /// <summary>
        /// True when the safe is locked, otherwise false.
        /// </summary>
        public bool IsLocked { get; set; }
        
        /// <summary>
        /// Time until which the safe remains locked.
        /// </summary>
        public DateTime LockedUntil { get; set; }

        /// <summary>
        /// The amount of minutes that the safe will be locked if the combination fails.
        /// </summary>
        public int MinsLocked { get; set; }
    }
}
