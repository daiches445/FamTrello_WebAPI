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
        //public User(string username, string password, string email, string name , string fam_ID)
        //{
        //    this.username = username;
        //    this.password = password;
        //    this.email = email;
        //    this.name = name;
        //    this.fam_ID = fam_ID;
        //}
    }
}