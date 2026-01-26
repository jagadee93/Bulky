using BulkyNTier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using BulkyNTier.Utilities;
using System.Security.Cryptography.Xml;


namespace BulkyNTier.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;


        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = appDbContext;
        }

        public async Task InitializeAsync(string email, string userName, string password)
        {

           

            // Apply migrations
            if (_db.Database.GetPendingMigrations().Any())
            {
                await _db.Database.MigrateAsync();
            }

            // Create roles
            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
            }


            if (string.IsNullOrWhiteSpace(email)) throw new InvalidOperationException("Facebook AppId is not configured.");
            if (string.IsNullOrWhiteSpace(userName)) throw new InvalidOperationException("Admin details are not configured.");
            if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("Admin Details are not configured");


            // Create admin user
            var adminUser = await _userManager.FindByEmailAsync(email);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Admin);
                }
            }


        }


    }
}