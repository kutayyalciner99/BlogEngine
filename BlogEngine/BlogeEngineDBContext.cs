using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogEngine
{
    public class BlogeEngineDbContext : IdentityDbContext<User, Role, int> //1.param user,2.role 3. aralarındaki bağlantı tipi
    {
        public BlogeEngineDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>(entity => {
                entity
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
                entity
                .HasIndex(p => p.Name)
                .IsUnique();//aynı isimli kategori kaydedilemiyor (Default olarak true kabul edilir)
                entity
                .HasMany(p => p.Posts)
                .WithOne(p => p.Category)//relation'ı tanımladığımız kısım
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);//cascade:Silerken, bağlı veriler de silinir; restrict:Silerken, Eğer bağlı veriler varsa hata döner.
            });
            builder.Entity<Post>(entity =>
            {
                entity
                .Property(p => p.Headline)
                .IsRequired()
                .HasMaxLength(450);
                entity
                .HasIndex(p => p.Headline)
                .IsUnique(false);
                entity
                .Property(p => p.Content)
                .IsRequired();
                entity
                .HasMany(p => p.Comments)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);//post silindiğinde yorumların da silinmesi için

            });
            base.OnModelCreating(builder);
        }
        public virtual DbSet<Category> Categories { get; set; } //Lazy Load yapabilmesi için Virtual , Dbset ise veri tabanından gelen veriler bu listeye doldurulur.(Entity Framework tarafından)
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
    public class Role : IdentityRole<int>//key tipi int,(Herhangi bir tipte Key olabilir)
    {

    }
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
    public class Post
    {
        public int Id { get; set; }
        public string Headline { get; set; } //Builder'la mecburi olduğunu belirttik.
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int Views { get; set; } = 0;
        public int CategoryId { get; set; } // Foreign Key
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
        public bool Enabled { get; set; } = false;
        public virtual Post Post { get; set; }

    }

}

