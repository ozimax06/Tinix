using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tinix.Models;
using Microsoft.AspNetCore.Hosting;
using Tinix.Context;
using Microsoft.Extensions.Caching.Memory;
using System.IO;


namespace Tinix.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private IBlog blog;

        public HomeController(IHostingEnvironment hostingEnvironment, IMemoryCache memoryCache, IBlog blog)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.blog = blog;
            ApplicationContext.WebRootPath = hostingEnvironment.WebRootPath;

        }


        public async Task<IActionResult> Detail(string id,string name)
        {
            BlogPost post = await blog.GetPostById(id);

            if (post == null)
            {
                return RedirectToAction("Index");
            }


            return View(post);
        }

        public async Task<IActionResult> Comment(string blogPostID, string comment)
        {
            BlogPost post = await blog.GetPostById(blogPostID);
       
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                await blog.AddComment(blogPostID, comment);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return RedirectToAction("Detail", new { 
                id = blogPostID, 
                name = post.Title
            });

        }


        public async Task<IActionResult> Index(int? index)
        {
          
                
            index = index ?? 0;


            if (index > 1)
            {
                index = --index;
            }
            else
            {
                index = 0;
            }

            int skipItems = 0;

            if (index > 0)
            {
                skipItems = ApplicationContext.PostsPerPage * (int)index;
            }
            else
            {
                skipItems = 0;
            }

            List<BlogPost> posts = await blog.GetPosts(ApplicationContext.PostsPerPage, skipItems);

            int total = blog.GetTotalPostsCount();

            BlogViewModel viewModel = new BlogViewModel
            {
                Items = posts,
                TotalPosts = total,
                PageIndex = ((int)index + 1),
                TotalPages = total / ApplicationContext.PostsPerPage,
                HasMorePages = skipItems < total
            };

            return View(viewModel);
        }
    }
}
