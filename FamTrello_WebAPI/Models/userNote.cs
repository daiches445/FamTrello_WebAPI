using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamTrello_WebAPI.Models
{
    public class UserNote
    {
        public User u = new User();
        public Note n = new Note();

        public UserNote()
        {
            u.age = 0;
            u.first_name = "mock";
            u.password = "mock";
            u.role = "mock";
        }
    }
}