using dating_course_api.Src.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data
{
    public class DataContext(DbContextOptions options)
        : IdentityDbContext<
            User,
            Role,
            int,
            IdentityUserClaim<int>,
            UserRole,
            IdentityUserLogin<int>,
            IdentityRoleClaim<int>,
            IdentityUserToken<int>
        >(options)
    {
        public required DbSet<Like> Likes { get; set; }
        public required DbSet<Message> Messages { get; set; }
        public required DbSet<Group> Groups { get; set; }
        public required DbSet<Connection> Connections { get; set; }
        public required DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User configuration
            builder.Entity<User>(b =>
            {
                // Email y Username configuration
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();

                b.HasIndex(u => u.NormalizedUserName)
                    .HasDatabaseName("UserNameIndex")
                    .IsUnique(false);

                // User roles configuration
                b.HasMany(ur => ur.UserRoles)
                    .WithOne(u => u.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            // Role configuration
            builder.Entity<Role>(b =>
            {
                b.HasMany(ur => ur.UserRoles)
                    .WithOne(u => u.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Seed roles
                b.HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Moderator",
                        NormalizedName = "MODERATOR"
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Member",
                        NormalizedName = "MEMBER"
                    }
                );
            });

            // Like configuration
            builder.Entity<Like>(b =>
            {
                b.HasKey(k => new { k.SourceUserId, k.TargetUserId });

                b.HasOne(s => s.SourceUser)
                    .WithMany(l => l.LikedUsers)
                    .HasForeignKey(s => s.SourceUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(s => s.TargetUser)
                    .WithMany(l => l.LikedByUsers)
                    .HasForeignKey(s => s.TargetUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Message configuration
            builder.Entity<Message>(b =>
            {
                b.HasOne(x => x.Recipient)
                    .WithMany(x => x.MessagesReceived)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Sender)
                    .WithMany(x => x.MessagesSent)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Photo configuration
            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
        }
    }
}
