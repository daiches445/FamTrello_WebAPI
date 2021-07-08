using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamTrello_WebAPI.Models
{
    public class Family
    {
        public string fam_ID { get; set; }
        public string name { get; set; }
        public List<Note> notes { get; set; }
        public List<User> members { get; set; }
    }
}