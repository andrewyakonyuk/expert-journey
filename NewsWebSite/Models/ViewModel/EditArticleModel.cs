using NewsWebSite.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Security.Application;

namespace NewsWebSite.Models.ViewModel
{
    public class EditArticleModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Заголовок статьи")]
        [StringLength(50, ErrorMessage = "Description Max Length is 50")]

        private string title;
        [Required]
        [Display(Name = "Заголовок")]
        [StringLength(50, ErrorMessage = "Description Max Length is 50")]
        public string Title { get { return title; } set { title = Sanitizer.GetSafeHtmlFragment(value); } }

        /*private string shortdescription;
        [Required]
        [Display(Name = "Короткое описание")]
        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Short Description Max Length is 200")]
        public string ShortDescription { get { return shortdescription; } set { shortdescription = Sanitizer.GetSafeHtmlFragment(value); } }
        */
        private string fulldescription;
        [Required]
        [Display(Name = "Текст статьи")]
        [DataType(DataType.MultilineText)]
        [StringLength(10000, ErrorMessage = "Description Max Length is 10000")]
        public string FullDescription { get { return fulldescription; } set { fulldescription = Sanitizer.GetSafeHtmlFragment(value); } }


        [Display(Name = "Изображение")]
        [ValidImage(maxSizeMB = 5)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name ="Теги статьи")]
        public ISet<Tag> ArticleTags { get; set; }
        public IEnumerable<Tag> AllTags { get; set; }

        public string ImagePath { get; set; }

        public EditArticleModel(Article a)
        {
            Id = a.Id;
            Title = a.Title;
            FullDescription = a.FullDescription;
            ArticleTags = a.Tags;
            ImagePath = a.Image;
        }
        public EditArticleModel()
        {
        }
    }
}