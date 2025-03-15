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

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab";
            var userRoleId = "8bc05967-a01b-424c-a760-475af79c738f";

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            };
            builder.Entity<IdentityRole>().HasData(roles);


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
                .OnDelete(DeleteBehavior.Restrict);

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
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MultipleChoiceTest>()
                .HasMany(mt => mt.Questions)
                .WithOne(q => q.MultipleChoiceTest)
                .HasForeignKey(q => q.MultipleChoiceTestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
