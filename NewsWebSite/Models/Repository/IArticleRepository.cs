using NewsWebSite.Controllers;
using NewsWebSite.Models.ViewModel;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.Repository
{
    public interface IArticleRepository
    {

        Article GetItem(int id);
        int GetCountOfLines();
        PagedList<DemoArticle> GetDemoList(NewsCriteria cr);
        int Save(Article article);
        bool IsAuthor(int articleId, int userId);
        bool IsExist(int id);
        void Delete(int articleId);
    }

    public class PagedList<T> : List<T>
    {
        public int LinesCount { get; set; }
        public int PageCount { get; set; }
    }
}