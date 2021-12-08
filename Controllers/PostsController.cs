using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Data;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;

namespace CapstoneProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostingContext _context;

        public PostsController(PostingContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string? searchQuery, int? selectedCategory, string? sortProperty, bool? isSortAscending)
        {
            IEnumerable<Post> posts = await _context.Posts
                .Where(p => !p.IsPrivatePost)
                .Include(p => p.Model).Include(p => p.PostCategory)
                .ToListAsync();

            PostIndexResult indexViewModel = new PostIndexResult
            {
                AllCategories = await _context.PostCategories.ToListAsync(),
                SortProperty = sortProperty,
                IsSortAscending= isSortAscending==true,
            };
            if (selectedCategory!=null)
            {
                posts = posts.Where(p => p.PostCategory.ID == selectedCategory);
                indexViewModel.SelectedCategory = await _context.PostCategories.FirstOrDefaultAsync(category => category.ID == selectedCategory);
            }

            if (!String.IsNullOrEmpty(searchQuery))
            {
                posts = posts.Where(p => p.Model.Description.Contains(searchQuery) || p.AdditionalText.Contains(searchQuery));
                indexViewModel.SearchQuery = searchQuery;
            }
            switch (sortProperty)
            {
                case "Title":
                    posts = posts.OrderBy(p => p.Model.Description);
                    break;
                case "Description":
                    posts = posts.OrderBy(p => p.AdditionalText);
                    break;
                case "Category":
                    posts = posts.OrderBy(p => p.PostCategory.Name);
                    break;
                default:
                    break;
            }
            if (isSortAscending == true)
            {
                posts = posts.Reverse();
            }

            indexViewModel.Posts = posts;
            return View(indexViewModel);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Model)
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            var postViewModel = new PostDetailResult
            {
                ID = post.ID,
                EmbedLink = $"{Request.Scheme}://{Request.Host}/Posts/Embed/{post.ID}",
                Description = post.Model.Description,
                AdditionalText = post.AdditionalText,
                ModelID = post.ModelID,
                ModelData = post.Model.ModelData,
                CategoryName=post.PostCategory.Name
            };
            return View(postViewModel);
        }
        public async Task<IActionResult> Embed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Model)
                .FirstOrDefaultAsync(p => p.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Raw/5
        public async Task<IActionResult> ModelData(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Model)
                .FirstOrDefaultAsync(p => p.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post.Model.ModelData);
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create()
        {
            PostCreateResult createViewModel = new PostCreateResult
            {
                AllCategories = await _context.PostCategories.ToListAsync(),
            };
            return View(createViewModel);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelData,Description,CategoryID,AdditionalText,IsPrivatePost")] PostCreateResult postViewModel)
        {
            if (ModelState.IsValid)
            {
                var model = new Model { Description = postViewModel.Description, ModelData = postViewModel.ModelData };
                _context.Add(model);
                await _context.SaveChangesAsync();
                var post = new Post { ModelID = model.ID, AdditionalText = postViewModel.AdditionalText, PostCategoryID = postViewModel.CategoryID, IsPrivatePost = postViewModel.IsPrivatePost };
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postViewModel);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Model)
                .FirstOrDefaultAsync(p=>p.ID==id);
            if (post == null)
            {
                return NotFound();
            }

            PostEditResult editViewModel = new PostEditResult
            {
                AllCategories = await _context.PostCategories.ToListAsync(),
                ID=post.ID,
                ModelID=post.ModelID,
                ModelData=post.Model.ModelData,
                AdditionalText=post.AdditionalText,
                Description=post.Model.Description,
                IsPrivatePost=post.IsPrivatePost
            };
            return View(editViewModel);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ModelID,ModelData,Description,AdditionalText,IsPrivatePost,CategoryID")] PostEditResult postViewModel)
        {
            if (id != postViewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var modelToUpdate = await _context.Models.FirstOrDefaultAsync(model => model.ID == postViewModel.ModelID);
                    modelToUpdate.ModelData = postViewModel.ModelData;
                    modelToUpdate.Description = postViewModel.Description;
                    _context.Update(modelToUpdate);
                    await _context.SaveChangesAsync();

                    var postToUpdate = await _context.Posts.FirstOrDefaultAsync(post => post.ID == postViewModel.ID);
                    postToUpdate.AdditionalText = postViewModel.AdditionalText;
                    postToUpdate.PostCategoryID = postViewModel.CategoryID;
                    postToUpdate.IsPrivatePost = postViewModel.IsPrivatePost;
                    _context.Update(postToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(postViewModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postViewModel);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Model)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.Include(p=>p.Model).FirstOrDefaultAsync(p=>p.ID==id);
            _context.Posts.Remove(post);
            _context.Models.Remove(post.Model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }
    }
}
