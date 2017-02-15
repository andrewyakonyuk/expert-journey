using NewsWebSite.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class ListItemPageModel
    {
        public int NumberOfItemsOnPage { get; set; }
        public PagedList<DemoArticle> ArticleList { get; set; }
        public LoginViewModel LoginForm { get; set; }
        public bool OnlyMyArticles { get; set; }
        public string TagList { get; set; }
        public string TagListToSHow { get; set; }


        public ListItemPageModel(int numberOfItemsOnPage, PagedList<DemoArticle> articlesList, bool onlyMy, string tagList, string tagListToShow)
        {
            NumberOfItemsOnPage = numberOfItemsOnPage;
            ArticleList = articlesList;
            OnlyMyArticles = onlyMy;
            TagList = tagList;
            TagListToSHow = tagListToShow;
        }
    }
}