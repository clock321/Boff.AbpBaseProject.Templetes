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
    /// Category
    /// </summary>
    public class Category : Entity<int>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [MaxLength(50)]
        public string CategoryName { get; set; }

        /// <summary>
        /// 展示名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string DisplayName { get; set; }

        [Required(AllowEmptyStrings =true)]
        public string Remark { get; set; }
    }
}
