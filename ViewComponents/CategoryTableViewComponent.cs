using MHBlog.Data;
using MHBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.ViewComponents
{
    [ViewComponent(Name = "CategoryTable")]
    public class CategoryTableViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
      
        public CategoryTableViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
                        
            var categories = await db.Categories.ToListAsync();
            return View(categories);

        }
    }
}