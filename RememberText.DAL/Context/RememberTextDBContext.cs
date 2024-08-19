using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;

namespace RememberText.DAL.Context
{
    public class RememberTextDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SvText> SvTexts { get; set; }
        public DbSet<RuText> RuTexts { get; set; }
        public DbSet<EnText> EnTexts { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }
        public DbSet<Language> Languages { get; set; }
        public RememberTextDbContext(DbContextOptions<RememberTextDbContext> Options) : base(Options) { }
    }
}
