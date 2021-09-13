using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Boff.Blog.Wallpaper
{
    public class Wallpaper : Entity<Guid>
    {
        /// <summary>
        /// URL
        /// </summary>
        [MaxLength(200)]      
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public int Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [MaxLength]
        public string Remark { get; set; }
    }
}