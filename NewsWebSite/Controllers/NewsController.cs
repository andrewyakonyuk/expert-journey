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
using NHibernate;


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


        public ActionResult Index()
        {
         
            var list = repo.GetDemoList(new NewsCriteria() {StartFrom = 0, Count = NumberOfItemsOnPage, LastId = 0 });
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
        [ValidateInput(false)]
        public ActionResult CreateArticle(CreateArticleModel a , string[] tags)
        {

            if (ModelState.IsValid)
            {
                
                Article newArticle = new Article(a.Title, a.FullDescription, a.Image.FileName, User.Identity.GetUserId<int>());
                newArticle.Tags.Clear();
                IEnumerable<Tag> articleTags = TagsHelper.FormTagList(tags, tagRepo);
                TagsHelper.SetTagForModel(newArticle, articleTags);
                var id = repo.Save(newArticle);
                FileHelper fileHelper = new FileHelper();
                fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), a.Image, id);
                if (a.Title.Length > 0 && a.FullDescription.Length > 0)
                {
                    return RedirectToAction("Article", new { Id = id });
                }
                else ModelState.AddModelError("length", "Поля содержат недопустимые значения!");
            }
            return View(a);
        }


        

        public ActionResult InterestingNews()
        {
            AppUser currentUser = userRepo.GetById(User.Identity.GetUserId<int>());
            List<ArticleForView> interestingArticles = new List<ArticleForView>();
            foreach(Tag tag in currentUser.Tags)
            {
                foreach(Article article in tag.Articles)
                {
                    if(!interestingArticles.Select(m=>m.Id).Contains(article.Id))
                    {
                        interestingArticles.Add(new ArticleForView(article));
                    }
                }
            }
            return View(interestingArticles);
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
        [ValidateInput(false)]
        public ActionResult EditArticle(EditArticleModel edited , string[] tags)
        {
            if (!ModelState.IsValid) return View(edited);
            var baseArticle = repo.GetItem(edited.Id);

            if (baseArticle == null || baseArticle.UserId != User.Identity.GetUserId<int>()) return HttpNotFound();

            var changesExist = false;
            if (edited.Image != null)
            {
                var fileHelper = new FileHelper();
                var isChanged = fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), edited.Image, baseArticle.Id);
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
          
            if (baseArticle.FullDescription != edited.FullDescription)
            {
                baseArticle.FullDescription = edited.FullDescription;
                changesExist = true;
            }
            baseArticle.Tags.Clear();
            if(tags!=null)
            {
                IEnumerable<Tag> newTags = TagsHelper.FormTagList(tags, tagRepo);
                TagsHelper.SetTagForModel(baseArticle, newTags);
		        changesExist = true;
            }
            if (changesExist) repo.Save(baseArticle);
            if (edited.Title.Length > 0 && edited.FullDescription.Length > 0)
                return RedirectToAction("Article", new { Id = edited.Id });
            return View(edited);
        }



        [HttpPost]
        [Authorize]
        public ActionResult Delete(int articleId = 0)
        {
            if (articleId > 1)
            {
                var userId = User.Identity.GetUserId<int>();
                if (repo.IsAuthor(articleId, userId))
                {
                    repo.Delete(articleId);
                    return RedirectToAction("Index");
                }
            }
            return HttpNotFound();
        }





        #region ForAjaxRequests

        [HttpPost]
        public string GetArticles(int page = 1, int n = 1, int lastId = 0)
        {
            if (page < 1) return "";
            var cr = new NewsCriteria() { StartFrom = page * NumberOfItemsOnPage, Count = n * NumberOfItemsOnPage, LastId = lastId };
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
