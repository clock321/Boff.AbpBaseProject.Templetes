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
    /// FriendLink
    /// </summary>
    public class FriendLink : Entity<int>
    {
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(20)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string LinkUrl { get; set; }
    }
}
