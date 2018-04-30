using System;
using System.ComponentModel.DataAnnotations;


using System.Collections.Generic;




namespace Tinix.Context
{
    public class BlogPost
    {
   
        [Required]
        public string ID
        {
            get;
            set;
        } = DateTime.UtcNow.Ticks.ToString();

        [Required]
        public string Title
        {
            get;
            set;
        }

        public string Slug
        {
            get;
            set;
        }

        [Required]
        public string Excerpt
        {
            get;
            set;
        }

        [Required]
        public string Content
        {
            get;
            set;
        }

        public DateTime PubDate
        {
            get;
            set;
        } = DateTime.UtcNow;

        public DateTime LastModified
        {
            get;
            set;
        } = DateTime.UtcNow;

        public bool IsPublished
        {
            get;
            set;
        }

        public List<BlogPostComment> Comments
        {
            get;
            set;
        }

        public int NumberOfLikes
        {
            get;
            set;
        }
    }
}