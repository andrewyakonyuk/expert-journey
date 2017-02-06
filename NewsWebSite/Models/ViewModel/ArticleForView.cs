using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class ArticleForView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Image { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public bool Editable { get; set; }

        public ISet<Tag> ArticleTags { get; set; }

        public ArticleForView(Article a)
        {
            Id = a.Id;
            Title = a.Title;
            FullDescription = a.FullDescription;
            Image = a.Image;
            ArticleTags = a.Tags;
            CreateDate = a.CreateDate.ToString("dd.MM.yyyy H:mm:ss");
            UpdateDate = (a.CreateDate != a.LastUpdateDate ? a.LastUpdateDate.ToString("dd.MM.yyyy H:mm:ss") : null);
            Editable = false;
        }
        public ArticleForView() { }
    }
    
}