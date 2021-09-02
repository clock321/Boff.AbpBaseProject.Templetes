using Boff.Blog.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Boff.Blog.Blog
{
    public interface IBlogService
    {
        Task<bool> InsertPostAsync(PostDto dto);

        Task<bool> DeletePostAsync(int id);

        Task<bool> UpdatePostAsync(PostDto dto);

        Task<PostDto> GetPostByIdAsync(int id);
    }
    public class BlogService : BlogAdminAppService, IBlogService
    {
        private readonly IPostRepository _postRepository;

        public BlogService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }


        public async Task<bool> InsertPostAsync(PostDto dto)
        {
            var entity = ObjectMapper.Map<PostDto, Post>(dto);
            //var entity = new Post
            //{
            //    Title = dto.Title,
            //    Author = dto.Author,
            //    Url = dto.Url,
            //    Html = dto.Html,
            //    Markdown = dto.Markdown,
            //    CategoryId = dto.CategoryId,
            //    CreationTime = dto.CreationTime
            //};

            var post = await _postRepository.InsertAsync(entity);
            return post != null;
        }


        public async Task<bool> DeletePostAsync(int id)
        {
            await _postRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> UpdatePostAsync(PostDto dto)
        {
            var post = await _postRepository.GetAsync(dto.Id);

            post.Title = dto.Title;
            post.Author = dto.Author;
            post.Url = dto.Url;
            post.Html = dto.Html;
            post.Markdown = dto.Markdown;
            post.CategoryId = dto.CategoryId;
            post.CreationTime = dto.CreationTime;

            await _postRepository.UpdateAsync(post);

            return true;
        }

        [AllowAnonymous]
        public async Task<PostDto> GetPostByIdAsync(int id)
        {
            var post = await _postRepository.GetAsync(id);
            if (post == null)
            {
                throw new UserFriendlyException("文章不存在");
            }
            return ObjectMapper.Map<Post, PostDto>(post);
 
        }
    }
}
