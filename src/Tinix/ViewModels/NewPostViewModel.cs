using System.ComponentModel.DataAnnotations;

namespace Tinix.Models
{
    public class NewPostViewModel
    {

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