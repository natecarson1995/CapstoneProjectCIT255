using CapstoneProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject.Data
{
    public class PostingContext : DbContext
    {
        public PostingContext(DbContextOptions<PostingContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model>().HasData(SeedData.Models);
            modelBuilder.Entity<PostCategory>().HasData(SeedData.PostCategories);
            modelBuilder.Entity<Post>().HasData(SeedData.Posts);
        }
        public DbSet<Model> Models { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
    }
}
