using Boff.Blog.Blog;
using Boff.Blog.Wallpaper;
using Boff.Blog.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Boff.Blog
{
    [ConnectionStringName("MySql")]
    public class BlogDbContext : AbpDbContext<BlogDbContext>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<FriendLink> FriendLinks { get; set; }

        public DbSet<Boff.Blog.Wallpaper.Wallpaper> Wallpapers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Console.WriteLine(modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType.Name.ToString()).JoinAsString(","));
            var AllEntites= modelBuilder.Model.GetEntityTypes().ToList();        
            foreach(var entity in AllEntites)
            {
                modelBuilder.Entity(entity.ClrType).ConfigureByConvention();
            }          
            var AllPropertys = AllEntites.SelectMany(t => t.GetProperties()).ToList();
            foreach(var property in AllPropertys)
            {
                if(property.ClrType == typeof(decimal)&& property.GetPrecision()==null)
                {
                    property.SetPrecision(10);       // set decimal precision
                    property.SetScale(2);
                }
                if (property.ClrType == typeof(string))
                {
                    var CustomAttributes= property.PropertyInfo.CustomAttributes;
                    if (!CustomAttributes.Any(x => x.AttributeType == typeof(RequiredAttribute)))
                    {
                        property.IsNullable = false;
                        property.SetDefaultValue(string.Empty);
                    }                                   
                    if (property.GetMaxLength() == null&&!CustomAttributes.Any(x=>x.AttributeType==typeof(MaxLengthAttribute)|| x.AttributeType == typeof(StringLengthAttribute)))
                    property.SetMaxLength(500);                   
                }
            }

            base.OnModelCreating(modelBuilder);
            Volo.Abp.Check.NotNull(modelBuilder, nameof(modelBuilder));
   
            /* Configure your own tables/entities inside here */

            //modelBuilder.Entity<Category>().ConfigureByConvention();
            //modelBuilder.Entity<Category>().Property(x => x.Remark).IsRequired(false);
            //modelBuilder.Entity<Category>().Property(x => x.Remark).Metadata.RemoveAnnotation("MaxLength");
        }
    }
}
