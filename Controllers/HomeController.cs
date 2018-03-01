using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MHBlog.Models;
using MHBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using MHBlog.Extensions;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MHBlog.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration Configuration;

        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
            ViewBag.Categories = _context.Categories.ToList();

        }

        public async Task<IActionResult> Index(int? page, string searchString)
        {
            IQueryable<Post> posts = _context.Posts
                .OrderByDescending(post => post.ReleaseDate)
                .Include(post => post.Category)
                .Include(post => post.PostTags)
                    .ThenInclude(pt => pt.Tag);

            //SEARCH
            if (!String.IsNullOrEmpty(searchString))
            {

                posts = posts.Where(post => post.PostTags.Any(pt => pt.Tag.Name.Contains(searchString)) || post.Title.Contains(searchString));

                //POPULARITY INCREESE
                var tag = _context.Tags.SingleOrDefault(t => t.Name == searchString);
                if (tag != null)
                {
                    tag.Counter += 1;
                    _context.Update(tag);
                    _context.SaveChanges();
                }
            }

            int pageSize = 6;
            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), page ?? 1, pageSize));

        }

        public async Task<IActionResult> CategoryView(int id, string name, int? page)
        {
            IQueryable<Post> posts = _context.Posts
                          .OrderByDescending(post => post.ReleaseDate)
                          .Include(post => post.Category)
                          .Include(post => post.PostTags)
                              .ThenInclude(pt => pt.Tag);

            posts = posts.Where(post => post.CategoryID == id);

            string friendlyName = FriendlyUrlHelper.GetFriendlyTitle(name);
            if (!string.Equals(friendlyName, name, StringComparison.Ordinal))
            {
                return RedirectToRoute("category", new { id = id, name = friendlyName });
            }

            ViewData["Name"] = name;
            int pageSize = 4;

            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), page ?? 1, pageSize));
        }



        public async Task<IActionResult> TagView(int id, string name, int? page)
        {
            IQueryable<Post> posts = _context.Posts
             .Include(p => p.PostTags)
                 .ThenInclude(t => t.Tag)
            .Include(p => p.Category);

            posts = posts.Where(post => post.PostTags.Any(pt => pt.Tag.TagID == id));

            //POPULARITY INCREESE
            var tag = _context.Tags.SingleOrDefault(t => t.TagID == id);
            if (tag != null)
            {
                tag.Counter += 1;
                _context.Update(tag);
                _context.SaveChanges();
            }

            string friendlyName = FriendlyUrlHelper.GetFriendlyTitle(name);
            if (!string.Equals(friendlyName, name, StringComparison.Ordinal))
            {
                // If the title is null, empty or does not match the friendly title, return a 301 Permanent
                // Redirect to the correct friendly URL.
                return RedirectToRoute("tag", new { id = id, name = friendlyName });
            }

            ViewData["Name"] = "#" + name;
            int pageSize = 4;

            return View("Index", await PaginatedList<Post>.CreateAsync(posts.AsNoTracking(), page ?? 1, pageSize));
        }

        public IActionResult Contact()
        {
            ContactFormModel Contact = new ContactFormModel();
            return View(Contact);
        }

        [HttpPost]
        public IActionResult Contact(ContactFormModel Contact)
        {

            if (ModelState.IsValid)
            {

                var message = new MailMessage(Contact.Email, Configuration["AppSettings:AdminUserEmail"]);
                message.To.Add(new MailAddress(Configuration["AppSettings:AdminUserEmail"]));
                message.From = new MailAddress(Contact.Email);
                message.Subject = Contact.Subject;
                message.Body = "Od: " + Contact.Email + Environment.NewLine + Contact.Name + " " + Contact.LastName + Environment.NewLine + "Temat: " + Contact.Subject + Environment.NewLine + Contact.Message;
                var smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(Configuration["AppSettings:AdminUserEmail"], Configuration["AppSettings:UserPassword"]);
                smtpClient.Send(message);           
                return RedirectToAction(nameof(Index));

            }
            return View(Contact);

        }

        public async Task<IActionResult> Details(int id, string title)
        {
            var post = _context.Posts
                   .Include(p => p.PostTags)
                              .ThenInclude(pt => pt.Tag)
                .SingleOrDefaultAsync(m => m.ID == id).Result;

            if (post == null)
            {
                return NotFound();
            }

            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(post.Title, true);
            if (!string.Equals(friendlyTitle, title, StringComparison.Ordinal))
            {           
                return RedirectToRoute("details", new { id = id, title = friendlyTitle });
            }
       
            post.ViewsCouter += 1;
            _context.Update(post);
            _context.SaveChanges();

            return View(post);
        }   

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
