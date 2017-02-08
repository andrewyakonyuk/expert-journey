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
     
        readonly IArticleRepository repo;

        readonly ICommentsRepository commentsRepository;

        readonly ITagRepository tagRepo;
        readonly IUserRepository userRepo;

        public NewsController(IArticleRepository repo,IUserRepository userRepo , ITagRepository tagRepo, ICommentsRepository commentsRepository)

        {
            this.userRepo = userRepo;
            this.tagRepo = tagRepo;
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
                a.UserId = 11;
                repo.Save(a);

            }
            return Content("ok");
        }

        #endregion


        public ActionResult Index(bool isUserNews = false, bool isInterestingNews = false)
        {
            var list = new PagedList<DemoArticle>();
            int userId = 0;
            AppUser currentUser = userRepo.GetById(User.Identity.GetUserId<int>());
            if (isUserNews == true)
            {
                userId = currentUser.Id;
            }
            if (isInterestingNews != true)
            {
                list = repo.GetDemoList(new ArticleCriteria() { StartFrom = 0, UserId = userId, Count = NumberOfItemsOnPage, LastId = 0 });
            }
            else
            {
                list = repo.GetArticleByTags(currentUser.Tags, new ArticleCriteria() { StartFrom = 0, UserId = 0, Count = NumberOfItemsOnPage, LastId = 0 });
            }
            return View(list);
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
                return View(viewArticle);
            }
            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateArticle()
        {
            CreateArticleModel createArticle = new CreateArticleModel { AllTags = tagRepo.GetAllTags() };
            return View(createArticle);
        }


        [HttpPost]
        [Authorize]
        public ActionResult CreateArticle(CreateArticleModel a, string[] tags)
        {
            if (!ModelState.IsValid) return View(a);
            if (tags == null || tags.Length == 0)
            {
                ModelState.AddModelError("tags", "Tag field required!");
                return View(a);
            }
            Article newArticle = new Article
            {
                Title = a.Title,
                ShortDescription = a.ShortDescription,
                FullDescription = a.FullDescription,
                UserId = User.Identity.GetUserId<int>()
            };
            if (a.Image != null)
            {
                newArticle.Image = a.Image.FileName;
            }
            else newArticle.Image = "Empty";
            newArticle.Tags.Clear();
            IEnumerable<Tag> articleTags = TagsHelper.CreateTagList(tags, tagRepo);
            TagsHelper.SetTagForModel(newArticle, articleTags);
            var id = repo.Save(newArticle);
            if (newArticle.Image != "Empty")
            {
                FileHelper fileHelper = new FileHelper();
                fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["ArticlImagesFolder"]), a.Image, id);
            }
            return RedirectToAction("Article", new { Id = id });
        }


        [HttpGet]
        [Authorize]
        public ActionResult EditArticle(int id = 0)
        {
            if (id < 1) return HttpNotFound();
            var article = repo.GetItem(id);
            EditArticleModel editArticle = new EditArticleModel(article);
            editArticle.AllTags = tagRepo.GetAllTags();
            if (article == null || article.UserId != User.Identity.GetUserId<int>()) return HttpNotFound();
            return View(editArticle);
        }
        

        [HttpPost]
        [Authorize]
        public ActionResult EditArticle(EditArticleModel edited , string[] tags, string imageCondition)
        {
            if (!ModelState.IsValid) return View(edited);
            var baseArticle = repo.GetItem(edited.Id);

            if (baseArticle == null || baseArticle.UserId != User.Identity.GetUserId<int>()) return HttpNotFound();

            var changesExist = false;
            if (imageCondition == "Empty")
            {
                baseArticle.Image = "Empty";
                changesExist = true;
            }

            
            if (edited.Image != null)
            {
                var fileHelper = new FileHelper();
                var isChanged = fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["ArticlImagesFolder"]), edited.Image, baseArticle.Id);
                if (isChanged)
                {
                    baseArticle.Image = edited.Image.FileName;
                    changesExist = true;
                }
            }
            if (baseArticle.Title != edited.Title)
            {
                baseArticle.Title = edited.Title;
                changesExist = true;
            }
            if (baseArticle.ShortDescription != edited.ShortDescription)
            {
                baseArticle.ShortDescription = edited.ShortDescription;
                changesExist = true;
            }
            if (baseArticle.FullDescription != edited.FullDescription)
            {
                baseArticle.FullDescription = edited.FullDescription;
                changesExist = true;
            }
            baseArticle.Tags.Clear();
            if(tags!=null)
            {
                IEnumerable<Tag> newTags = TagsHelper.CreateTagList(tags, tagRepo);
                TagsHelper.SetTagForModel(baseArticle, newTags);
		        changesExist = true;
            }
            if (changesExist) repo.Save(baseArticle);
            return RedirectToAction("Article", new { Id = edited.Id });
        }


        #region ForAjaxRequests

        [HttpPost]
        public string GetArticles(int page = 1, int n = 1, int lastId = 0, int userId = 0)
        {
            if (page < 1) return "";
            var cr = new ArticleCriteria() { StartFrom = page * NumberOfItemsOnPage, UserId = userId, Count = n * NumberOfItemsOnPage, LastId = lastId };
            var lst = repo.GetDemoList(cr);// as IList<DemoArticle>;
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
