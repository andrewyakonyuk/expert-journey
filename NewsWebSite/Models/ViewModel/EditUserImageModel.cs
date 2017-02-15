using NewsWebSite.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.ViewModel
{
    public class EditUserImageModel
    {
        public int Id { get; set; }

        [Display(Name = "Изображениел пользователя")]
        [ValidImage(maxSizeMB = 5)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }

        public string ImageName { get; set; }
    }
}