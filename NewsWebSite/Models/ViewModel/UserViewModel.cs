using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ImageName { get; set; }
        [UIHint("TagList")]
        public ISet<Tag> UserTags { get; set; }
    }
}