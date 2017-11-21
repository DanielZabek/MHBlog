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
    [ViewComponent(Name = "CategoryList")]
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;
        
        public CategoryListViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool isDropdown)
       {
            string MyView = "Default";

            if (isDropdown == true)
            {
                MyView = "DropdownCategoryList";
            }
         var categories = await db.Categories.ToListAsync();
         return View(MyView, categories);

       }
    }
}
