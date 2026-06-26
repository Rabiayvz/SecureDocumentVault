using FirstApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Data
{
    public static class SeedData
    {
        public static void Seed(AppDbContext context)
        {
            // Zaten veri varsa tekrar ekleme
            if (context.Users.Any(u => u.RoleId == 2))
            {
                Console.WriteLine("Seed data already exists, skipping.");
                return;
            }

            var managers = new List<User>();

            // 5 Manager oluştur
            for (int i = 1; i <= 5; i++)
            {
                var manager = new User
                {
                    Email = $"manager{i}@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                    RoleId = 2, // Manager
                    CreatedAt = DateTime.UtcNow
                };
                managers.Add(manager);
            }

            context.Users.AddRange(managers);
            context.SaveChanges(); // Manager ID'leri oluşsun

            // Her manager'a 10 User ekle
            var users = new List<User>();

            for (int i = 0; i < managers.Count; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    var user = new User
                    {
                        Email = $"user{i + 1}_{j}@test.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                        RoleId = 3, // User
                        ManagerId = managers[i].Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    users.Add(user);
                }
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            Console.WriteLine("Seed data created:");
            Console.WriteLine("  5 Managers (manager1@test.com ... manager5@test.com)");
            Console.WriteLine("  50 Users (user1_1@test.com ... user5_10@test.com)");
            Console.WriteLine("  Password for all: Manager123! / User123!");
        }
    }
}