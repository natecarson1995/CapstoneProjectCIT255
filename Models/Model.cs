using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapstoneProject.Models
{
    public class Model
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [StringLength(750, ErrorMessage ="Description cannot be longer than 750 characters.")]
        [Display(Name = "Title")]
        public string Description { get; set; }

        private string _modelData;
        [Required]
        public string ModelData
        {
            get
            {
                var jsonData = JsonConvert.DeserializeObject(_modelData);
                return JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            }
            set
            {
                var jsonData = JsonConvert.DeserializeObject(value);
                _modelData = JsonConvert.SerializeObject(jsonData, Formatting.None);
            }
        }


    }
}
