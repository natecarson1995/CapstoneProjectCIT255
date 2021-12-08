using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class PostIndexResult
    {
        public IEnumerable<Post> Posts { get; set; }
        public string? SearchQuery { get; set; }
        public PostCategory? SelectedCategory { get; set; }
        public IEnumerable<PostCategory> AllCategories { get; set; }
        public string? SortProperty { get; set; }
        public bool IsSortAscending { get; set; }
    }
}
