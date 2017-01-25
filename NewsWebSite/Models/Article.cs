using NewsWebSite.Attributes;
using NewsWebSite.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsWebSite.Models
{
    public class Article
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string FullDescription { get; set; }
        public virtual string Image { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime LastUpdateDate { get; set; }
        public virtual int UserId { get; set; }

        public Article(string title, string fulldescription, string image)
        {
            Title = title;
            FullDescription = fulldescription;
            Image = image;
        }
        public Article()
        {
            Title = null;
            FullDescription = null;
            Image = null;
            CreateDate = DateTime.MinValue;
            LastUpdateDate = DateTime.MinValue;
        }
    }
}