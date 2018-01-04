using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tinix.Context
{
    public interface IBlog
    {

        Task SavePost(string postContent, string title);

        int GetTotalPostsCount();

        Task<List<BlogPost>> GetPosts();

        Task<List<BlogPost>> GetPosts(int count, int skip = 0);

        Task<BlogPost> GetPostById(string id);

        void Delete(string id);
    }
}