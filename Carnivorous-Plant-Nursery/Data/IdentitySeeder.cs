using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Identity;

namespace Carnivorous_Plant_Nursery.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[] { AuthorizationRole.Admin, AuthorizationRole.Manager, AuthorizationRole.Customer })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = configuration["SeedAdmin:Email"];
            var adminPassword = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = configuration["SeedAdmin:DisplayName"] ?? "Nursery Admin"
                };

                var created = await userManager.CreateAsync(admin, adminPassword);
                if (!created.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AuthorizationRole.Admin))
            {
                await userManager.AddToRoleAsync(admin, AuthorizationRole.Admin);
            }
        }
    }
}
