using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamTrello_WebAPI.Models
{
    public class Note
    {
        
        public int id { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public DateTime created { get; set; }
        public string username { get; set; }
        public string  fam_ID { get; set; }
        public string status { get; set; }


    }
}