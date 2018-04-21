using System.ComponentModel.DataAnnotations;

namespace Tinix.Models
{
    public class EditPostViewModel
    {
        [Required]
        public string BlogPostID
        {
            get;
            set;
        }


        [Required(ErrorMessage = "Please enter the blog post title")]
        [MaxLength(200)]
        public string Title
        {
            get;
            set;
        }


        public string Content
        {
            get;
            set;
        }
    }
}