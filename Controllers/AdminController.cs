using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MHBlog.Data;
using MHBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections;
using Microsoft.AspNetCore.Http;

namespace MHBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _enviroment;


        public AdminController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _enviroment = env;
        }

        // GET: Admin
        [Route("admin")]
        public async Task<IActionResult> Index(int? page)
        {
            var posts = _context.Posts
                .Include(c => c.Category)
                .AsNoTracking();

            return View(await posts.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        // GET: Admin/Create
        public IActionResult Create()
        {
            var model = new CreateViewModel();
            model.Categories = _context.Categories.ToList();

            return View(model);
        }


        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //ADD IMAGE
                if (model.File.Length > 0)
                {
                    var uploadPath = Path.Combine(_enviroment.WebRootPath, "images");
                    var fileName = Path.GetFileName(model.File.FileName);

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                        model.Post.ImageName = fileName;
                    }
                }

                //ADD TAGS
                List<string> stringTags = model.tagsString.Split(",").ToList();
                foreach (var item in stringTags)
                {
                    var a = _context.Tags.SingleOrDefault(t => t.Name == item);
                    if (a != null)
                    {
                        _context.Add(new PostTag { Post = model.Post, Tag = a });
                        a.Counter += 1;
                    }
                    else
                    {
                        _context.Add(new PostTag { Post = model.Post, Tag = new Tag { Name = item } });
                    }
                }

                Category category = await _context.Categories.SingleOrDefaultAsync(s => s.CategoryID == model.Post.CategoryID);
                category.isUsed = true;
                model.Post.ReleaseDate = DateTime.Now;
                model.Post.Body = Request.Form["Body"].ToString();
               
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult CreateCategory()
        {
            var model = new Category();
            return View(model);
        }


        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryID,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var model = new CreateViewModel();
            model.Categories = _context.Categories.ToList();
            if (id == null)
            {
                return NotFound();
            }

            model.Post = await _context.Posts.SingleOrDefaultAsync(m => m.ID == id);
            if (model.Post == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Description,Body,CategoryID,ReleaseDate,ImageName")] Post post)
        {
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Body = Request.Form["Body"].ToString();
                    
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
            return View(post);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.SingleOrDefaultAsync(m => m.ID == id);

            string path = Path.Combine(_enviroment.WebRootPath, "images");
            string imagePath = Path.Combine(path, post.ImageName);
            System.IO.File.Delete(imagePath);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            List<Category> categories = _context.Categories.ToList();
            foreach (var item in categories)
            {
                int x = _context.Posts.Count(w => w.CategoryID == item.CategoryID);
                if (x == 0)
                {
                    item.isUsed = false;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }

        // GET: Admin/DeleteCategory/5
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .SingleOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        // POST: Admin/DeleteCategory/5
        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryID == id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
