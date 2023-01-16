using System;
using System.Collections.Generic;
using System.Text;

namespace NoteMaster.Models
{
    /// <summary>
    /// Note Class.
    /// </summary>
    public class Note
    {
        public const string DEFAULT_TITLE = "NewNote";
        public const string DEFAULT_BODY = "";

        /// <summary>
        /// Construct a new Note instance.
        /// </summary>
        public Note(string title=DEFAULT_TITLE, string body=DEFAULT_BODY)
        {
            Title = title;
            Body = body;
        }   

        // Properties
        /// <summary>
        /// Title of the Note.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Body of the Note.
        /// </summary>
        public string Body { get; set; }


        public override string ToString()
        {
            return base.ToString();
        }
    }
}
