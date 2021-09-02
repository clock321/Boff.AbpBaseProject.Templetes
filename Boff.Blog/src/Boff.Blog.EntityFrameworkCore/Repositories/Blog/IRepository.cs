using Boff.Blog.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Boff.Blog.Repositories
{
    /// <summary>
    /// IPostRepository
    /// </summary>
    public interface IPostRepository : IRepository<Post, int>
    {
    }

    /// <summary>
    /// ICategoryRepository
    /// </summary>
    public interface ICategoryRepository : IRepository<Category, int>
    {
    }

    /// <summary>
    /// ITagRepository
    /// </summary>
    public interface ITagRepository : IRepository<Tag, int>
    {
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        Task BulkInsertAsync(IEnumerable<Tag> tags);
    }

    /// <summary>
    /// IPostTagRepository
    /// </summary>
    public interface IPostTagRepository : IRepository<PostTag, int>
    {
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="postTags"></param>
        /// <returns></returns>
        Task BulkInsertAsync(IEnumerable<PostTag> postTags);
    }

     /// <summary>
    /// IFriendLinkRepository
    /// </summary>
    public interface IFriendLinkRepository : IRepository<FriendLink, int>
    {
    }
}
