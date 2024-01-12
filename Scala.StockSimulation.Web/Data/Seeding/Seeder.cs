using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Core.Entities;

namespace Scala.StockSimulation.Web.Data.Seeding
{
    public static class Seeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var supplier1 = new Supplier { Id = Guid.NewGuid(), Name = "Panos", Created = System.DateTime.Now };
            var supplier2 = new Supplier { Id = Guid.NewGuid(), Name = "Pizza Hut", Created = System.DateTime.Now };
            modelBuilder.Entity<Supplier>().HasData(supplier1, supplier2);

            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "admin@howest.be",
                NormalizedEmail = "ADMIN@HOWEST.BE",
                UserName = "admin@howest.be",
                NormalizedUserName = "ADMIN@HOWEST.BE",
                EmailConfirmed = true,
                Firstname = "Admin",
                Lastname = "Admin",
                SecurityStamp = Guid.NewGuid().ToString(),
                Created = System.DateTime.Now
            };

            adminUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminUser, "Welkom123");

            var seedingStudents = new List<ApplicationUser>
            {
                new() { Id = Guid.NewGuid(), Email = "diegoetha@howest.be", NormalizedEmail = "DIEGOGOETHA@HOWEST.BE", Firstname = "Diego", Lastname = "Goethals", Created = System.DateTime.Now, SecurityStamp = Guid.NewGuid().ToString() },
                new() { Id = Guid.NewGuid(), Email = "quinaspe@howest.be", NormalizedEmail = "QUINASPE@HOWEST.BE", Firstname = "Quinten", Lastname = "Aspeslagh", Created = System.DateTime.Now, SecurityStamp = Guid.NewGuid().ToString() },
                new() { Id = Guid.NewGuid(), Email = "bluevos@howest.be", NormalizedEmail = "BLUEVOS@HOWEST.BE", Firstname = "Blue", Lastname = "Vosselman", Created = System.DateTime.Now, SecurityStamp = Guid.NewGuid().ToString() },
                new() { Id = Guid.NewGuid(), Email = "alexver@howest.be", NormalizedEmail = "ALEXVER@HOWEST.BE", Firstname = "Alekxander", Lastname = "Verhaeghe", Created = System.DateTime.Now, SecurityStamp = Guid.NewGuid().ToString() }
            };

            seedingStudents.ForEach(s =>
            {
                s.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(s, "Welkom123");
                s.UserName = s.Email;
                s.NormalizedUserName = s.NormalizedEmail;
            });

            modelBuilder.Entity<ApplicationUser>().HasData(
                seedingStudents.Concat(new[] { adminUser })
            );

            modelBuilder.Entity<OrderType>().HasData(
                new OrderType { Id = Guid.NewGuid(), Name = "Leverancier", Created = System.DateTime.Now },
                new OrderType { Id = Guid.NewGuid(), Name = "Klant", Created = System.DateTime.Now }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = Guid.NewGuid(), Name = "Broodje Kip Curry", Description = "Broodje met kip curry beleg", Price = 1.20m, ArticleNumber = "bkc1", InitialStock = 150, InitialMinimumStock = 100, InitialMaximumStock = 500, SupplierId = supplier1.Id, Created = System.DateTime.Now },
                new Product { Id = Guid.NewGuid(), Name = "Broodje Smos", Description = "Broodje met kaas en ham", Price = 7.20m, ArticleNumber = "brs1", InitialStock = 180, InitialMinimumStock = 100, InitialMaximumStock = 500, SupplierId = supplier1.Id, Created = System.DateTime.Now },
                new Product { Id = Guid.NewGuid(), Name = "Pizza Pepperoni", Description = "Pizza met pepperoni", Price = 6.70m, ArticleNumber = "pp1", InitialStock = 150, InitialMinimumStock = 100, InitialMaximumStock = 500, SupplierId = supplier2.Id, Created = System.DateTime.Now },
                new Product { Id = Guid.NewGuid(), Name = "Pizza Margherita", Description = "Standaard pizza", Price = 6.80m, ArticleNumber = "pm1", InitialStock = 160, InitialMinimumStock = 100, InitialMaximumStock = 500, SupplierId = supplier2.Id, Created = System.DateTime.Now }
            );

            var adminRole = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" };
            var studentRole = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Student", NormalizedName = "STUDENT" };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(adminRole, studentRole);

            var studentUserRoles = seedingStudents.Select(s => new IdentityUserRole<Guid>
            {
                RoleId = studentRole.Id,
                UserId = s.Id
            }).ToList();

            var adminUserRole = new IdentityUserRole<Guid> { RoleId = adminRole.Id, UserId = adminUser.Id };

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(studentUserRoles.Concat(new[] { adminUserRole })
            );
        }
    }
}