using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class Tag
    {
        public int TagID { get; set; }
        public string Name { get; set; }
        public int Counter { get; set; }
        public ICollection<PostTag> PostTags { get; } = new List<PostTag>();

        public Tag()
        {
            Counter = 1;
        }
    }
}
