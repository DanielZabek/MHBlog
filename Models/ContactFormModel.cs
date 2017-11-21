using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MHBlog.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; }

        
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [BindProperty]
        public ContactFormModel Contact { get; set; }
    }
}
