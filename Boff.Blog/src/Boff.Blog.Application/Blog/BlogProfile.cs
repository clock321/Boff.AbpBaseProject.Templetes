using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boff.Blog.Blog
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<Post, PostDto>();

            CreateMap<PostDto, Post>();
        }
    }
}
