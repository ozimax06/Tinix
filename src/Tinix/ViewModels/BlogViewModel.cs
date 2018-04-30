using System.Collections.Generic;
using Tinix.Context;

namespace Tinix.Models
{
    public class BlogViewModel
    {
        public List<BlogPost> Items
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int TotalPages
        {
            get;
            set;
        }


        public int TotalPosts
        {
            get;
            set;
        }

        public bool HasMorePages
        {
            get;
            set;
        }

    }
}