﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesAPI.Models
{
    public class GetNotesRequest
    {
        public string token { get; set; }
        public int projectID { get; set; }
        public List<int> attributeIDs { get; set; }
    }
}