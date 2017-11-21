using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class PostTag
    {
        
        public int PostID { get; set; }
        public int TagID { get; set; }

        public Post Post { get; set; }
        public Tag Tag { get; set; }

    }
}
