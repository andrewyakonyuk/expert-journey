using NewsWebSite.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Security.Application;

namespace NewsWebSite.Models.ViewModel
{
    public class CreateArticleModel
    {

        private string title;
        [Required]
        [Display(Name = "Заголовок")]
        [StringLength(50, ErrorMessage = "Description Max Length is 50")]
        public string Title { get { return title; } set { title = Sanitizer.GetSafeHtmlFragment(value); } }

        /*private string shortdescription;
        [Required]
        [Display(Name = "Короткое описание")]
        [StringLength(200, ErrorMessage = "Short Description Max Length is 200")]
        public string ShortDescription { get { return shortdescription; } set { shortdescription = Sanitizer.GetSafeHtmlFragment(value); } }
        */
        private string fulldescription;
        [Required]
        [Display(Name = "Текст статьи")]
        [DataType(DataType.MultilineText)]
        [StringLength(10000, ErrorMessage = "Description Max Length is 10000")]
        public string FullDescription { get { return fulldescription; } set { fulldescription = Sanitizer.GetSafeHtmlFragment(value); } }



        /*private string tags;
        [Required(ErrorMessage = "поле обязательно")]
        [Display(Name = "Теги статьи")]
        [StringLength(100, ErrorMessage = "Tags Max Length is 100")]
        public string Tags { get { return tags; } set { tags = Sanitizer.GetSafeHtmlFragment(value); } }*/

        //public string Tags { get; set; }

        [Required]
        [Display(Name = "Изображение")]
        [ValidImage]
        [AllowedExtensions(new string[] {".jpg", ".png" })]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name ="Теги статьи")]
        public IEnumerable<Tag> AllTags { get; set; }
    }
}
