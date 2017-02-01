﻿using NewsWebSite.Models.ViewModel;
using NewsWebSite.Models;
using Newtonsoft.Json;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.AspNet.Identity;
using NewsWebSite.Models.Repository;

namespace NewsWebSite.Controllers
{
    public class NewsController : Controller
    {
        readonly int NumberOfItemsOnPage = int.Parse(ConfigurationManager.AppSettings["NumberOfItemsOnPage"]);

        readonly TagsHelper th = new TagsHelper();
        readonly IArticleRepository repo;
        readonly ICommentsRepository commentsRepository;


        public NewsController(IArticleRepository repo, ICommentsRepository commentsRepository)
        {
            this.repo = repo;
            this.commentsRepository = commentsRepository;
        }




        #region ForDebug

        [HttpGet]
        public ActionResult CreateLines(int n = 0)
        {

            for (int i = 1; i <= n; i++)
            {
                var a = new Article();
                a.Title = i.ToString();
                a.FullDescription = a.Title;
                a.Tags = ",tag,tag1,";
                a.UserId = 11;
                repo.Save(a);

            }
            return Content("ok");
        }

        #endregion


        public ActionResult Index(bool onlyMyArt = false, string tags = "")
        {
            int userId = 0;

            if (onlyMyArt && User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId<int>();
            }
            string tagline = "";
            if (tags.Length > 0)
            {

                tagline = th.GetLine(tags);

            }
            var list = repo.GetDemoList(0, NumberOfItemsOnPage, 0, th.GetArray(tagline));
            var model = new ListItemPageModel(NumberOfItemsOnPage, list, onlyMyArt, tagline, th.GetLineToShow(tagline));
            return View(model);
        }

        [HttpGet]
        public ActionResult Article(int id = 0)
        {
            if (id > 0)
            {
                var article = repo.GetItem(id);
                var viewArticle = new ArticleForView(article);
                if (article.UserId == User.Identity.GetUserId<int>())
                    viewArticle.Editable = true;
                viewArticle.Tags = th.GetLineToShow(viewArticle.Tags);
                return View(viewArticle);
            }
            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateArticle()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult CreateArticle(CreateArticleModel a)
        {
            if (!ModelState.IsValid) return View(a);
            var tags = th.GetLine(a.Tags);
            Article newArticle = new Article(a.Title, a.FullDescription, a.Image.FileName, tags, User.Identity.GetUserId<int>());
            var id = repo.Save(newArticle);
            FileHelper fileHelper = new FileHelper();
            fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), a.Image, id);
            return RedirectToAction("Article", new { Id = id });
        }


        [HttpGet]
        [Authorize]
        public ActionResult EditArticle(int id = 0)
        {
            if (id < 1) return HttpNotFound();
            var article = repo.GetItem(id);

            if (article == null || article.UserId != User.Identity.GetUserId<int>()) return HttpNotFound();
            return View(new EditArticleModel(article));
        }


        [HttpPost]
        [Authorize]
        public ActionResult EditArticle(EditArticleModel edited)
        {
            if (!ModelState.IsValid) return View(edited);
            var baseArticle = repo.GetItem(edited.Id);
            var changed = false;
            if (baseArticle == null || baseArticle.UserId != User.Identity.GetUserId<int>()) return HttpNotFound();


            var fileHelper = new FileHelper();
            if (edited.Image != null &&
            fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), edited.Image, baseArticle.Id))
                changed = true;

            if (baseArticle.Title != edited.Title)
            {
                baseArticle.Title = edited.Title;
                changed = true;
            }

            if (baseArticle.FullDescription != edited.FullDescription)
            {
                baseArticle.FullDescription = edited.FullDescription;
                changed = true;
            }


            baseArticle.Tags = th.GetLine(edited.Tags);

            repo.Save(baseArticle);
            return RedirectToAction("Article", new { Id = edited.Id });
        }


        #region ForAjaxRequests

        [HttpPost]
        public string GetArticles(int page = 1, int n = 1, int lastId = 0, bool onlyMyArticles = false, string tagLine = "")
        {
            if (page < 1) return "";
            var lst = repo.GetDemoList(page * NumberOfItemsOnPage, n * NumberOfItemsOnPage, lastId, th.GetArray(tagLine));// as IList<DemoArticle>;
            return JsonConvert.SerializeObject(lst);
        }

        [HttpPost]
        public string GetComments(int articleId)
        {
            var list = commentsRepository.GetList(articleId);
            return JsonConvert.SerializeObject(list);
        }

        #endregion
    }
}
