using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tinix.Context
{
    public class FileBlogService : IBlog
    {
        private const string BLOG_POSTS = "blog_cache";

        private IMemoryCache cache;

        private ILogger<FileBlogService> log;


        public FileBlogService(IMemoryCache cache, ILogger<FileBlogService> log)
        {
            this.cache = cache;

            this.log = log;
        }

        public void Delete(string id)
        {
            List<BlogPost> posts = GetCachedPosts();

            BlogPost blogPost = posts.FirstOrDefault(post => post.ID == id);

            if (blogPost == null)
            {
                throw new ArgumentException($"Blog post with id {id} not found ");
            }


            string filePath = ApplicationContext.PostsFolder + @"\" + blogPost.ID + ".xml";

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    posts.Remove(blogPost);

                    Sort(ref posts);
                }
                else
                {
                    log.LogError($"File {filePath} not found");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }

        public async Task SavePost(string content, string title)
        {

            string id = Guid.NewGuid().ToString();

            DateTime now = DateTime.UtcNow;

            XDocument doc = new XDocument(
                            new XElement("post",
                                new XElement("title", title),
                                new XElement("slug", string.Empty),
                                new XElement("pubDate", now.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("lastModified", now.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement("excerpt", string.Empty),
                                new XElement("content", content),
                                new XElement("ispublished", true),
                                new XElement("categories", string.Empty),
                                new XElement("comments", string.Empty)
                            ));

            //XElement categories = doc.XPathSelectElement("post/categories");
            //foreach (string category in post.Categories)
            //{
            //    categories.Add(new XElement("category", category));
            //}

            //XElement comments = doc.XPathSelectElement("post/comments");
            //foreach (Comment comment in post.Comments)
            //{
            //    comments.Add(
            //        new XElement("comment",
            //            new XElement("author", comment.Author),
            //            new XElement("email", comment.Email),
            //            new XElement("date", comment.PubDate.ToString("yyyy-MM-dd HH:m:ss")),
            //            new XElement("content", comment.Content),
            //            new XAttribute("isAdmin", comment.IsAdmin),
            //            new XAttribute("id", comment.ID)
            //        ));
            //}

            using (FileStream fs = new FileStream(ApplicationContext.PostsFolder + @"\" + id + ".xml", FileMode.Create, FileAccess.ReadWrite))
            {
                await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
            }

            List<BlogPost> posts = GetCachedPosts();


            BlogPost post = new BlogPost();
            post.Title = title;
            post.Slug = string.Empty;
            post.PubDate = now;
            post.LastModified = now;
            post.Excerpt = string.Empty;
            post.IsPublished = true;
            post.ID = id;

            posts.Add(post);


            Sort(ref posts);
        }



        public int GetTotalPostsCount()
        {
            List<BlogPost> posts = GetCachedPosts();
            return posts.Count;
        }

        public Task<BlogPost> GetPostById(string id)
        {
            List<BlogPost> posts = GetCachedPosts();

            return Task.FromResult(posts.FirstOrDefault(post => post.ID == id));

        }

        public Task<List<BlogPost>> GetPosts()
        {
            List<BlogPost> posts = GetCachedPosts();

            return Task.FromResult(posts);
        }

        public Task<List<BlogPost>> GetPosts(int count, int skip = 0)
        {
            List<BlogPost> posts = GetCachedPosts();

            return Task.FromResult(posts.Skip(skip).Take(count).ToList());
        }


        private List<BlogPost> GetCachedPosts()
        {
            List<BlogPost> posts;

            if (cache.TryGetValue(BLOG_POSTS, out posts))
            {
                return posts;
            }

            LoadFromDisk();

            cache.TryGetValue(BLOG_POSTS, out posts);

            return posts;
        }

        private void LoadFromDisk()
        {
            List<BlogPost> posts = new List<BlogPost>();

            foreach (string file in Directory.EnumerateFiles(ApplicationContext.PostsFolder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                XElement doc = XElement.Load(file);

                BlogPost post = new BlogPost
                {
                    ID = Path.GetFileNameWithoutExtension(file),
                    Title = ReadValue(doc, "title"),
                    Excerpt = ReadValue(doc, "excerpt"),
                    Content = ReadValue(doc, "content"),
                    Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                    PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                    LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString(CultureInfo.InvariantCulture))),
                    IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
                };

                posts.Add(post);
            }

            Sort(ref posts);

            cache.Set(BLOG_POSTS, posts);

        }


        private void Sort(ref List<BlogPost> posts)
        {
            posts = posts.OrderByDescending(post => post.PubDate).ToList();

            cache.Set(BLOG_POSTS, posts);
        }

        private string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
            {
                return doc.Element(name)?.Value;
            }

            return defaultValue;
        }

        private string ReadAttribute(XElement element, XName name, string defaultValue = "")
        {
            if (element.Attribute(name) != null)
            {
                return element.Attribute(name)?.Value;
            }

            return defaultValue;
        }
    }
}