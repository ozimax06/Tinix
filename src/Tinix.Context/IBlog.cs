using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tinix.Context
{
    public interface IBlog
    {

        Task SavePost(string postContent, string title, bool publish);
        
        Task EditPost(string id, string postContent, string title, bool publish);

        int GetTotalPostsCount();

        Task<List<BlogPost>> GetPosts();

        Task<List<BlogPost>> GetPosts(int count, int skip = 0);

        Task<BlogPost> GetPostById(string id);

        Task Delete(string id);
        
        Task DeleteComment(string id);

        Task AddComment(string BlogPostID, string comment);
        
        Task LikePost(string BlogPost);
    }
}