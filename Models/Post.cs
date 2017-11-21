using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class Post
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public int CategoryID { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ImageName { get; set; }
        public int ViewsCouter { get; set; }

        public Category Category { get; set; }
        public ICollection<PostTag> PostTags { get; } = new List<PostTag>();

    }
}
