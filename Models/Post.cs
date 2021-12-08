using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject.Models
{
    public class Post
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int PostCategoryID { get; set; }
        [ForeignKey("PostCategoryID")]
        public PostCategory PostCategory{ get; set; }

        [Required]
        public int ModelID { get; set; }
        [ForeignKey("ModelID")]
        public Model Model { get; set; }

        [StringLength(750, ErrorMessage ="Cannot add more than 750 characters of extra text.")]
        [Display(Name = "Description")]
        public string AdditionalText { get; set; }

        public bool IsPrivatePost { get; set; } = false;
    }
}
