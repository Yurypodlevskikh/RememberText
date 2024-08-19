using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Data
{
    public class RememberTextDBInitializer
    {
        private readonly RememberTextDbContext _db;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<Role> _RoleManager;

        public RememberTextDBInitializer(RememberTextDbContext db, UserManager<User> UserManager, RoleManager<Role> RoleManager)
        {
            _db = db;
            _UserManager = UserManager;
            _RoleManager = RoleManager;
        }

        public void Initialize() => InitializeAsync().Wait();
        public async Task InitializeAsync()
        {
            var db = _db.Database;
            await db.MigrateAsync().ConfigureAwait(false);
            await InitializeIdentityAsync().ConfigureAwait(false);
        }

        public async Task InitializeIdentityAsync()
        {
            if (!await _RoleManager.RoleExistsAsync(Role.GenAdministrator))
                await _RoleManager.CreateAsync(new Role { Name = Role.GenAdministrator });

            if (!await _RoleManager.RoleExistsAsync(Role.Administrator))
                await _RoleManager.CreateAsync(new Role { Name = Role.Administrator });

            if (!await _RoleManager.RoleExistsAsync(Role.User))
                await _RoleManager.CreateAsync(new Role { Name = Role.User });

            if(await _UserManager.FindByNameAsync(User.AdminEmail) is null)
            {
                var admin = new User
                {
                    Nickname = User.RTAdministrator,
                    UserName = User.AdminEmail,
                    Email = User.AdminEmail,
                    RegistrationDate = DateTime.Now,
                    EmailConfirmed = true,
                    YearOfBirth = 1974
                };
                
                var create_result = await _UserManager.CreateAsync(admin, User.AdminDefaultPassword);
                if (create_result.Succeeded)
                    await _UserManager.AddToRoleAsync(admin, Role.GenAdministrator);
                else
                {
                    var errors = create_result.Errors.Select(error => error.Description);
                    throw new InvalidOperationException($"Error when created GenAdministrator: {string.Join(",", errors)}");
                }
            }
        }
    }
}
