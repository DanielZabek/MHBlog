using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class Post
    {

        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Body { get; set; }

        public int CategoryID { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ImageName { get; set; }
        public int ViewsCouter { get; set; }

        public Category Category { get; set; }
        public ICollection<PostTag> PostTags { get; } = new List<PostTag>();

    }
}
