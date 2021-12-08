using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject.ViewModels
{
    public class PostDetailResult
    {
        public int ID { get; set; }
        public int ModelID { get; set; }
        [Display(Name="Title")]
        public string Description { get; set; }
        [Display(Name = "Description")]
        public string AdditionalText { get; set; }
        public string ModelData { get; set; }
        public string CategoryName { get; set; }
        public string EmbedLink { get; set; }
        public string EmbedCode { 
            get {
                return "<iframe width='720' height='720' src='" + EmbedLink + "' frameborder = '0' allowfullscreen></iframe>";
            } 
        }
    }
}
