using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using InternDiary.Models.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InternDiary.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Entry> Entries { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<EntrySkill> EntrySkills { get; set; }

        public ApplicationDbContext()
            : base("InternDiaryDb", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EntrySkill>()
                .HasRequired(e => e.Entry)
                .WithMany()
                .HasForeignKey(e => e.EntryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EntrySkill>()
                .HasRequired(e => e.Skill)
                .WithMany()
                .HasForeignKey(e => e.SkillId)
                .WillCascadeOnDelete(false);
        }
    }
}