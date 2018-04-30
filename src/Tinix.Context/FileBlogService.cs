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
        private const string BLOG_COMMENTS = "comment_cache";

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
            List<BlogPostComment> comments = GetCachedComments();
            comments = comments.Where(c => c.BlogPostID == id).ToList();

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
            //Remove associated comments
            foreach(var comment in comments)
            {
                DeleteComment(comment.ID);
            }
        }

        public void DeleteComment(string id)
        {
            string filePath = ApplicationContext.CommentsFolder + @"\" + id + ".xml";

            try
            {
                if (File.Exists(filePath))
                {
                     File.Delete(filePath);
                     List<BlogPostComment> blogComments =  GetCachedComments();
                     BlogPostComment blogComment = blogComments.FirstOrDefault(b => b.ID == id);
                     if(blogComment != null)
                     {
                        blogComments.Remove(blogComment);
                        cache.Set(BLOG_COMMENTS, blogComments);
                     }

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

        public async Task AddComment(string BlogPostID, string comment)
        {
            XDocument doc = new XDocument(
                            new XElement("comment",
                                new XElement("BlogPostID", BlogPostID),
                                new XElement("comment", comment)
                            ));

            var id = Directory.GetFiles(ApplicationContext.CommentsFolder).Length+1;//auto increment

            using (FileStream fs = new FileStream(ApplicationContext.CommentsFolder + @"\" + id + ".xml", FileMode.Create, FileAccess.ReadWrite))
            {
                await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
            }

            BlogPostComment blogComment = new BlogPostComment
            {
                BlogPostID = BlogPostID,
                Comment = comment
            };

            List<BlogPostComment> blogComments =  GetCachedComments();
            blogComments.Add(blogComment);
            cache.Set(BLOG_COMMENTS, blogComments);
                        
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
                                new XElement("numberOfLikes", "0")
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
            post.NumberOfLikes = 0;

            posts.Add(post);


            Sort(ref posts);
        }

         public async Task EditPost(string id, string postContent, string title)
         {
            var now =  DateTime.UtcNow;
            XElement doc = XElement.Load(ApplicationContext.PostsFolder + "/" + id+ ".xml");
            doc.Element("content").Value = postContent;
            doc.Element("title").Value = title;
            doc.Element("lastModified").Value = now.ToString("yyyy-MM-dd HH:mm:ss");

            using (FileStream fs = new FileStream(ApplicationContext.PostsFolder + @"\" + id + ".xml", FileMode.Create, FileAccess.ReadWrite))
            {
                await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
            }

            //edit the post from the list and reset the cache
            List<BlogPost> posts = GetCachedPosts();
            posts.Where(p=> p.ID == id)
                 .Select(p => { p.Content = postContent; p.Title = title; p.LastModified = now; return p;} )
                 .ToList();

            Sort(ref posts);
            cache.Set(BLOG_POSTS, posts);

         }

         public async Task LikePost(string BlogPostID)
         {
                XElement doc = XElement.Load(ApplicationContext.PostsFolder + "/" +  BlogPostID+ ".xml");
                
                var likes = Convert.ToInt32(doc.Element("numberOfLikes").Value); 
                doc.Element("numberOfLikes").Value = (likes+1).ToString();

                using (FileStream fs = new FileStream(ApplicationContext.PostsFolder + @"\" + BlogPostID + ".xml", FileMode.Create, FileAccess.ReadWrite))
                {
                    await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
                }

         }


        public int GetTotalPostsCount()
        {
            List<BlogPost> posts = GetCachedPosts();
            return posts.Count;
        }

        public Task<BlogPost> GetPostById(string id)
        {
            List<BlogPost> posts = GetCachedPosts();
            List<BlogPostComment> comments= GetCachedComments();
            posts = AssignCommentsToPosts(posts, comments);

            return Task.FromResult(posts.FirstOrDefault(post => post.ID == id));

        }

        public Task<List<BlogPost>> GetPosts()
        {
            List<BlogPost> posts = GetCachedPosts();
            List<BlogPostComment> comments= GetCachedComments();
            posts = AssignCommentsToPosts(posts, comments);

            return Task.FromResult(posts);
        }

        public Task<List<BlogPost>> GetPosts(int count, int skip = 0)
        {
            List<BlogPost> posts = GetCachedPosts();
            List<BlogPostComment> comments= GetCachedComments();
            posts = AssignCommentsToPosts(posts, comments);

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

        private List<BlogPostComment> GetCachedComments()
        {
            List<BlogPostComment> comments;

            if (cache.TryGetValue(BLOG_COMMENTS, out comments))
            {
               return comments;
            }

            LoadFromDisk();
            cache.TryGetValue(BLOG_COMMENTS, out comments);

            return comments;
        }

        private List<BlogPost> AssignCommentsToPosts(List<BlogPost> posts, List<BlogPostComment> comments)      
        {
            foreach(var post in posts)
            {
                
                post.Comments = comments.Where(x => x.BlogPostID == post.ID).ToList();
            }

            return posts;
        }


        /* 
        Retrieve blogposts and comments from XML and store them separately in the cachce
        */
        private void LoadFromDisk()
        {
            List<BlogPost> posts = new List<BlogPost>();
            List<BlogPostComment> comments = new List<BlogPostComment>();

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
                    NumberOfLikes =  Convert.ToInt32(ReadValue(doc, "numberOfLikes")),
                };

                posts.Add(post);
            }

            foreach (string file in Directory.EnumerateFiles(ApplicationContext.CommentsFolder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                XElement doc = XElement.Load(file);


                BlogPostComment comment = new BlogPostComment
                {
                    ID = Path.GetFileNameWithoutExtension(file),
                    BlogPostID = ReadValue(doc, "BlogPostID"),
                    Comment = ReadValue(doc, "comment"),
                };

                comments.Add(comment);
            }

            Sort(ref posts);

            cache.Set(BLOG_POSTS, posts);
            cache.Set(BLOG_COMMENTS, comments);

            

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