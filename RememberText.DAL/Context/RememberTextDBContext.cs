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
        public DbSet<FiText> FiTexts { get; set; }
        public DbSet<UkText> UkTexts { get; set; }
        public DbSet<EsText> EsTexts { get; set; }
        public DbSet<HrText> HrTexts { get; set; }
        public DbSet<DeText> DeTexts { get; set; }
        public DbSet<SkText> SkTexts { get; set; }
        public DbSet<SlText> SlTexts { get; set; }

        public DbSet<NormalizedTag> NormalizedTags { get; set; }
        public DbSet<SvTag> SvTags { get; set; }
        public DbSet<RuTag> RuTags { get; set; }
        public DbSet<EnTag> EnTags { get; set; }
        public DbSet<FiTag> FiTags { get; set; }
        public DbSet<UkTag> UkTags { get; set; }
        public DbSet<EsTag> EsTags { get; set; }
        public DbSet<HrTag> HrTags { get; set; }
        public DbSet<DeTag> DeTags { get; set; }
        public DbSet<SkTag> SkTags { get; set; }
        public DbSet<SlTag> SlTags { get; set; }
        public DbSet<SvTagAssignment> SvTagAssignments { get; set; }
        public DbSet<RuTagAssignment> RuTagAssignments { get; set; }
        public DbSet<EnTagAssignment> EnTagAssignments { get; set; }
        public DbSet<FiTagAssignment> FiTagAssignments { get; set; }
        public DbSet<UkTagAssignment> UkTagAssignments { get; set; }
        public DbSet<EsTagAssignment> EsTagAssignments { get; set; }
        public DbSet<HrTagAssignment> HrTagAssignments { get; set; }
        public DbSet<DeTagAssignment> DeTagAssignments { get; set; }
        public DbSet<SkTagAssignment> SkTagAssignments { get; set; }
        public DbSet<SlTagAssignment> SlTagAssignments { get; set; }

        public DbSet<TextCopyrightModel> TextCopyrights { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<GuestBookEntry> GuestBookEntries { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }
        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Visit>().HasKey(x => new { x.Visited, x.IpId });
            // Configure the TagAssignment entity's composite primary key
            builder.Entity<SvTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<RuTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<EnTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<FiTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<UkTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<EsTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<HrTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<DeTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<SkTagAssignment>().HasKey(x => new { x.TextId, x.TagId });
            builder.Entity<SlTagAssignment>().HasKey(x => new { x.TextId, x.TagId });

            base.OnModelCreating(builder);
        }
        public RememberTextDbContext(DbContextOptions<RememberTextDbContext> Options) : base(Options) { }
    }
}
