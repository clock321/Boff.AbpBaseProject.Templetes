using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Boff.Blog.Blog
{
    /// <summary>
    /// PostTag
    /// </summary>
    public class PostTag : Entity<int>
    {
        /// <summary>
        /// 文章Id
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        [Required]
        public int TagId { get; set; }
    }
}
