using System;
using System.IO;

namespace Tinix.Context
{
    public static class ApplicationContext
    {

        public static string PostsFolder => WebRootPath + Path.DirectorySeparatorChar + "Posts";

        public static string WebRootPath
        {
            get;
            set;
        }

        public static string Title
        {
            get;
            set;
        }


        public static string Subtitle
        {
            get;
            set;
        }

        
        public static int PostsPerPage
        {
            get;
            set;
        }

        public static string Layout
        {
            get;
            set;
            
        }


    }
}
