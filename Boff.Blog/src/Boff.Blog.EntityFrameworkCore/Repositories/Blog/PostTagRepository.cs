using Boff.Blog.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Boff.Blog.Repositories.Blog
{
    /// <summary>
    /// PostTagRepository
    /// </summary>
    public class PostTagRepository : EfCoreRepository<BlogDbContext, PostTag, int>, IPostTagRepository
    {
        public PostTagRepository(IDbContextProvider<BlogDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="postTags"></param>
        /// <returns></returns>
        public async Task BulkInsertAsync(IEnumerable<PostTag> postTags)
        {
            await GetDbSetAsync().Result.AddRangeAsync(postTags);
            await SaveChangesAsync(GetCancellationToken());
        }
    }
}
