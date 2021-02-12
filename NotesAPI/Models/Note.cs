using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesAPI.Models
{
    public class Note
    {
        public int noteID { get; set; }
        public int projectID { get; set; }
        public string noteText { get; set; }
        public DateTime noteCreated { get; set; }
        public DateTime lastEdit { get; set; }
        public List<NoteAttribute> attributes { get; set; }
    }
}