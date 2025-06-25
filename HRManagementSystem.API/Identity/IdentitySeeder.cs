using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace HRManagementSystem.API.Identity
{
    public static class IdentitySeeder
    {
        public static readonly string[] Roles = new[] { "Admin", "HR", "DepartmentManager", "Auditor", "Employee" };

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

           
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null) 
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Admin" 
                };

                var result = await userManager.CreateAsync(user, "Admin@123"); 

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

                    var employee = new Employee
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        ApplicationUserId = user.Id,
                        IsActive = true
                    };

                    dbContext.Employees.Add(employee);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Failed to create default admin user:\n" +
                        string.Join("\n", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
