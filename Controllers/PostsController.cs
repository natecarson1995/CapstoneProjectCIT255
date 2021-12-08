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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdditionalText,IsPrivatePost")] Post post, [Bind("Description,ModelData")] Model model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                post.ModelID = model.ID;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            post.Model = model;
            return View(post);
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
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ModelID,AdditionalText,IsPrivatePost")] Post post, [Bind("Description,ModelData")] Model model)
        {
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.ID = post.ModelID;
                    _context.Update(model);
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ID))
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
            post.Model = model;
            return View(post);
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
