using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class CreateViewModel
    {

        public Post Post { get; set; }
        public IFormFile File { get; set; }
        public List<Category> Categories { get; set; }
        public string tagsString { get; set; }

    
    }
}
