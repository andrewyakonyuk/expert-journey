using NewsWebSite.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class CreateArticleModel
    {

        //public int Id { get; set; }
        [Required]
        [Display(Name = "Заголовок")]
        [StringLength(50, ErrorMessage = "Description Max Length is 50")]
        public string Title { get; set; }

        [Required]
        [Display(Name ="Краткое описание статьи")]
        [StringLength(200 , ErrorMessage ="Максимальная длина описания статьи 200 символов")]
        public string ShortDescription { get; set; }

        [Required]
        [Display(Name = "Текст статьи")]
        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage = "Description Max Length is 2000")]
        public string FullDescription { get; set; }

        public string Tags { get; set; }

        [Display(Name = "Изображение")]
        [ValidImage]
        [AllowedExtensions(new string[] {".jpg", ".png" })]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name ="Теги статьи")]
        public IEnumerable<Tag> AllTags { get; set; }
    }
}
