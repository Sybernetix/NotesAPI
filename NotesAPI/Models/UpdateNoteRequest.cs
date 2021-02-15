using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesAPI.Models
{
    public class UpdateNoteRequest
    {
        public string token { get; set; }
        public int noteID { get; set; }
        public string noteText { get; set; }
    }
}