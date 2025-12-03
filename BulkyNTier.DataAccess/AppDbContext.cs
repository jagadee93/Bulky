

using BulkyNTier.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace BulkyNTier.DataAccess
{
    public class AppDbContext:IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }


        public DbSet<Product> Products { get; set; }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //Creating Table 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder); //Identity Requirement 
            modelBuilder.Entity<Category>().HasData(
                   new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                    new Category { Id = 2, Name = "Sci-Fi", DisplayOrder = 2 },
                    new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(  
                new Product { Id = 101, Title = "Harry Potter And the Deathly Hallows", Author = "J.K Rowling", Description = "Harry Potter In a Magical world looking to find out sirius Black", ISBN = "127h32g24", ListPrice = 100, Price = 90, Price50 = 60, Price100 = 40 ,CategoryId=1,ImageURL=""},
                new Product { Id = 102, Title = "Think and Grow Rich", Author = "Napolean Hill", Description = "A Schlor trying to teach Economics", ISBN = "12843jur", ListPrice = 300, Price = 250, Price50 = 200, Price100 = 150,CategoryId=1 ,ImageURL=""}
                );



        }
    }
}

