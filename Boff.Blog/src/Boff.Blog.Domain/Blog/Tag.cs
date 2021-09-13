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
    /// Tag
    /// </summary>
    public class Tag : Entity<int>
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string TagName { get; set; }

        /// <summary>
        /// 展示名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string DisplayName { get; set; }


        public string Remark { get; set; }
    }
}
