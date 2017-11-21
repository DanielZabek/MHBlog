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
    [ViewComponent(Name = "TagsCloud")]
    public class TagsCloudViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        

        public TagsCloudViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new TagsCloudViewComponentModel();
            if (db.Tags.Count() > 0)
            {

                model.counter = db.Tags.Max(t => t.Counter);
            }

            model.tags = await db.Tags
                .ToListAsync();
            model.tags = model.tags.Where(tag => tag.Counter > model.counter/8).ToList();

            return View(model);
            
           
            
           
        }
    }
}
