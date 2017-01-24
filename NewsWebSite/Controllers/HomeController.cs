using NewsWebSite.Models.ViewModel;
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
using NewsWebSite.Models.Repository;

namespace NewsWebSite.Controllers
{
    public class HomeController : Controller
    {
        readonly int NumberOfItemsOnPage = int.Parse(ConfigurationManager.AppSettings["NumberOfItemsOnPage"]);


        readonly IRepository repo;
        readonly IUserRepository user_repo;

        public HomeController(IRepository repo , IUserRepository user_repo)
        {
            this.user_repo = user_repo;
            this.repo = repo;
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
                repo.Save(a);

            }
            return Content("ok");
        }

        #endregion


        public ActionResult Index()
        {
            var list = repo.GetDemoList(0, NumberOfItemsOnPage);
            var model = new ListItemPageModel(NumberOfItemsOnPage, list);
            return View(model);
        }


        [HttpGet]
        public ActionResult Article(int id = 0)
        {
            if (id > 0)
            {
                var article = new ArticleForView(repo.GetItem(id));
                return View(article);
            }
            return HttpNotFound();
        }


        public ActionResult CreateArticle()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateArticle(CreateArticleModel a)
        {
            if (!ModelState.IsValid) return View(a);
            Article newArticle = new Article(a.Title, a.FullDescription, a.Image.FileName);
            var id = repo.Save(newArticle);
            FIleHelper fileHelper = new FIleHelper();
            fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), a.Image, id);
            return RedirectToAction("Article", new { Id = id });
        }

        public ActionResult UserRegistration()
        {
            return View("CreateUser");
        }

        [HttpPost]
        public ActionResult UserRegistration(CreateUserModels user)
        {
            if (ModelState.IsValid)
            {
                UserModels userModel = new UserModels { Firstname = user.Firstname, Secondname = user.Secondname, Login=user.Login, Email = user.Email, Password = user.Password };
                user_repo.Save(userModel);
                return RedirectToAction("Index");
            }
            else return View("Index");
        }

        [HttpGet]
        public ActionResult EditArticle(int id = 0)
        {
            if (id < 1) return HttpNotFound();
            var article = repo.GetItem(id);
            if (article == null) return HttpNotFound();
            return View(new EditArticleModel(article));
        }


        [HttpPost]
        public ActionResult EditArticle(EditArticleModel edited)
        {
            if (!ModelState.IsValid) return View(edited);
            var baseArticle = repo.GetItem(edited.Id);
            if (baseArticle == null) return HttpNotFound();
            var changesExist = false;
            if (edited.Image != null)
            {
                var fileHelper = new FIleHelper();
                var isChanged = fileHelper.SaveOrUpdateArticleImage(Server.MapPath(ConfigurationManager.AppSettings["UserImagesFolder"]), edited.Image, baseArticle.Id);
                if (isChanged)
                {
                    baseArticle.Image = edited.Image.FileName;
                    changesExist = true;
                }
            }
            if (edited.Title != null && baseArticle.Title != edited.Title)
            {
                baseArticle.Title = edited.Title;
                changesExist = true;
            }
            if (edited.FullDescription != null && baseArticle.FullDescription != edited.FullDescription)
            {
                baseArticle.FullDescription = edited.FullDescription;
                changesExist = true;
            }
            if (changesExist) repo.Save(baseArticle);
            return RedirectToAction("Article", new { Id = edited.Id });
        }



        #region ForAjaxRequests
       
        [HttpPost]
        public string GetArticles(int page = 1, int n = 1)
        {
            if (page < 1) return "";
            var lst = repo.GetDemoList(page * NumberOfItemsOnPage, n * NumberOfItemsOnPage);// as IList<DemoArticle>;
            return JsonConvert.SerializeObject(lst);
        }

        #endregion
    }
}
