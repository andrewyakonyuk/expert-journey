using NewsWebSite.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class ArticleListModel
    {
        public PagedList<DemoArticle> ArticleList { get; set; }
        public int UsierId { get; set; }
        public string Type { get; set; } = "default";
    }
}