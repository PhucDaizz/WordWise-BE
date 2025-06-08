using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WordWise.Api.Models.Domain;

namespace WordWise.Api.Data
{
    public class WordWiseDbContext: DbContext
    {
        public WordWiseDbContext(DbContextOptions<WordWiseDbContext> options): base(options)
        {
        }

        public DbSet<ExtendedIdentityUser> ExtendedIdentityUsers { get; set; }
        public DbSet<FlashcardSet> FlashcardSets { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<FlashcardReview> FlashcardReviews { get; set; }
        public DbSet<WritingExercise> WritingExercises { get; set; }
        public DbSet<MultipleChoiceTest> MultipleChoiceTests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ContentReport> ContentReports { get; set; }
        public DbSet<UserLearningStats> UserLearningStats { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomParticipant> RoomParticipants { get; set; }
        public DbSet<StudentFlashcardAttempt> StudentFlashcardAttempts { get; set; }

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var superAdminRoleId = "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1";
            var adminRoleId = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab";
            var userRoleId = "8bc05967-a01b-424c-a760-475af79c738f";

            var roles = new List<IdentityRole>
            {
                new IdentityRole { 
                    Id = superAdminRoleId, 
                    Name = "SuperAdmin", 
                    NormalizedName = "SUPERADMIN", 
                    ConcurrencyStamp = Guid.NewGuid().ToString() 
                },
                new IdentityRole { 
                    Id = adminRoleId, 
                    Name = "Admin", 
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole { 
                    Id = userRoleId, 
                    Name = "User", 
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            var superAdminId = "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef";
            var superAdminUser = new ExtendedIdentityUser
            {
                Id = superAdminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "dai742004.dn@gmail.com",
                NormalizedEmail = "dai742004.dn@gmail.com".ToUpper(),
            };

            superAdminUser.PasswordHash = new PasswordHasher<ExtendedIdentityUser>().HashPassword(superAdminUser, "Admin@123");
            builder.Entity<ExtendedIdentityUser>().HasData(superAdminUser);

            var superAdminRole = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string> {
                    RoleId = adminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string> {
                    RoleId = userRoleId,
                    UserId = superAdminId,
                }
            };


            builder.Entity<IdentityUserRole<string>>().HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRole);



            builder.Entity<FlashcardReview>()
                .HasOne(fr => fr.User)
                .WithMany(u => u.FlashcardReviews)
                .HasForeignKey(fr => fr.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FlashcardReview>()
                .HasOne(fr => fr.FlashcardSet)
                .WithMany(fs => fs.FlashcardReviews)
                .HasForeignKey(fr => fr.FlashcardSetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FlashcardSet>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.FlashcardSets)
                .HasForeignKey(fs => fs.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Flashcard>()
                .HasOne(f => f.FlashcardSet)
                .WithMany(fs => fs.Flashcards)
                .HasForeignKey(f => f.FlashcardSetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WritingExercise>()
                .HasOne(we => we.User)
                .WithMany(u => u.WritingExercises)
                .HasForeignKey(we => we.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MultipleChoiceTest>()
                .HasOne(mt => mt.User)
                .WithMany(u => u.MultipleChoiceTests)
                .HasForeignKey(mt => mt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MultipleChoiceTest>()
                .HasMany(mt => mt.Questions)
                .WithOne(q => q.MultipleChoiceTest)
                .HasForeignKey(q => q.MultipleChoiceTestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentReport>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.ContentReports)
                .HasForeignKey(cr => cr.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserLearningStats>()
                .HasOne(uls => uls.User)
                .WithOne(u => u.UserLearningStats)
                .HasForeignKey<UserLearningStats>(uls => uls.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

           builder.Entity<Room>()
                .HasMany(r => r.RoomParticipants)
                .WithOne(rp => rp.Room)
                .HasForeignKey(rp => rp.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Room>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Room>()
                .HasOne(r => r.FlashcardSet)
                .WithMany(fs => fs.Rooms)
                .HasForeignKey(r => r.FlashcardSetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RoomParticipant>()
                .HasMany(rp => rp.StudentFlashcardAttempts)
                .WithOne(sfa => sfa.RoomParticipant)
                .HasForeignKey(sfa => sfa.RoomParticipantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoomParticipant>()
                .HasOne(rp => rp.User)
                .WithMany(u => u.RoomParticipants)
                .HasForeignKey(rp => rp.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentFlashcardAttempt>()
                .HasOne(sfa => sfa.Flashcard)
                .WithMany(f => f.StudentFlashcardAttempts)
                .HasForeignKey(sfa => sfa.FlashcardId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentFlashcardAttempt>()
                .HasOne(sfa => sfa.Room)
                .WithMany(r => r.StudentFlashcardAttempts)
                .HasForeignKey(sfa => sfa.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            
        }
    }
}
