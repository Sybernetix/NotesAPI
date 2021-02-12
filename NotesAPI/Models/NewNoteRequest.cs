using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesAPI.Models
{
    public class NewNoteRequest
    {
        public int projectID { get; set; }
        public string noteText { get; set; }
        public List<string> noteAttributes { get; set; }
    }
}