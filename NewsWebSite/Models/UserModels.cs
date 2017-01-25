using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models
{
    public class UserModels
    {
        public virtual int Id { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Secondname { get; set; }
        public virtual string Login { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public UserModels()
        {
            Firstname = null;
            Secondname = null;
            Email = null;
            Password = null;
        }
    }
}