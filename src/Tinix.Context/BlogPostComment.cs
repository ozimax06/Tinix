using System;
using System.ComponentModel.DataAnnotations;

namespace Tinix.Context
{
    public class BlogPostComment
    {
        [Required]
        public string ID
        {
            get;
            set;
        } 

        [Required]
        public string BlogPostID
        {
            get;
            set;
        } 

        [Required]
        public string Comment
        {
            get;
            set;
        } 
    }
}