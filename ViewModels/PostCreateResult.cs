using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject.ViewModels
{
    public class PostCreateResult
    {
        public IEnumerable<PostCategory> AllCategories { get; set; }
        public string ModelData { get; set; }
        [Display(Name="Title")]
        public string Description { get; set; }
        [Display(Name = "Description")]
        public string AdditionalText { get; set; }
        public int CategoryID { get; set; }
        public bool IsPrivatePost { get; set; }
    }
}
