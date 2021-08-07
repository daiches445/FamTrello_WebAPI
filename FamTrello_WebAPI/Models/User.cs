using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamTrello_WebAPI.Models
{
    public class User
    {
        public string username { get; set; }

        public string password { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public short age { get; set; }
        public string fam_ID { get; set; }
        public string role { get; set; }
        public bool isAdmin { get; set; }
        public bool isApproved { get; set; }
        public string push_token { get; set; }

    }
}