﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WordWise.Api.Data;

#nullable disable

namespace WordWise.Api.Migrations
{
    [DbContext(typeof(WordWiseDbContext))]
    [Migration("20250405020837_AddTableContentReportAndUserLearningStats")]
    partial class AddTableContentReportAndUserLearningStats
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IdentityRole");

                    b.HasData(
                        new
                        {
                            Id = "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1",
                            ConcurrencyStamp = "7eaf4091-b3f3-46e6-b3af-11d708f2193d",
                            Name = "SuperAdmin",
                            NormalizedName = "SUPERADMIN"
                        },
                        new
                        {
                            Id = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab",
                            ConcurrencyStamp = "742366ea-d53c-4d50-8644-fff0de647afd",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "8bc05967-a01b-424c-a760-475af79c738f",
                            ConcurrencyStamp = "e544e937-29dc-4781-9268-44e1baffb45b",
                            Name = "User",
                            NormalizedName = "USER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("IdentityUserRole<string>");

                    b.HasData(
                        new
                        {
                            UserId = "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                            RoleId = "e95e8a62-fb9b-4b1d-9b64-b36e5805c4f1"
                        },
                        new
                        {
                            UserId = "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                            RoleId = "477d3788-e4b3-4f3d-8dbd-aaead19b78ab"
                        },
                        new
                        {
                            UserId = "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                            RoleId = "8bc05967-a01b-424c-a760-475af79c738f"
                        });
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.ContentReport", b =>
                {
                    b.Property<Guid>("ContentReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ContentReportId");

                    b.HasIndex("UserId");

                    b.ToTable("ContentReport");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.ExtendedIdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiry")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ExtendedIdentityUsers");

                    b.HasData(
                        new
                        {
                            Id = "6ebdbaaf-706e-4d35-9e26-e8ce70a866ef",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "828cf79c-3fc9-40f7-8510-08779f8aaff2",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "dai742004.dn@gmail.com",
                            EmailConfirmed = false,
                            Gender = false,
                            Level = 0,
                            LockoutEnabled = false,
                            NormalizedEmail = "DAI742004.DN@GMAIL.COM",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAEHp3s9IHqY0XvXaHz8ynXcxOz7IKZPNbuX0c5SRBhhqk6bVMk5X91b2TgXrdjBaqjw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "4ed42858-1214-4afa-8a6f-e9aef9e7ee44",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.Flashcard", b =>
                {
                    b.Property<int>("FlashcardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlashcardId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Definition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Example")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("FlashcardSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Term")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlashcardId");

                    b.HasIndex("FlashcardSetId");

                    b.ToTable("Flashcards");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.FlashcardReview", b =>
                {
                    b.Property<Guid>("FlashcardReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FlashcardSetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FlashcardReviewId");

                    b.HasIndex("FlashcardSetId");

                    b.HasIndex("UserId");

                    b.ToTable("FlashcardReviews");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.FlashcardSet", b =>
                {
                    b.Property<Guid>("FlashcardSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<int?>("LearnerCount")
                        .HasColumnType("int");

                    b.Property<string>("LearningLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("Level")
                        .HasColumnType("int");

                    b.Property<string>("NativeLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FlashcardSetId");

                    b.HasIndex("UserId");

                    b.ToTable("FlashcardSets");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.MultipleChoiceTest", b =>
                {
                    b.Property<Guid>("MultipleChoiceTestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<int?>("LearnerCount")
                        .HasColumnType("int");

                    b.Property<string>("LearningLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("NativeLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Title")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MultipleChoiceTestId");

                    b.HasIndex("UserId");

                    b.ToTable("MultipleChoiceTests");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.Question", b =>
                {
                    b.Property<Guid>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Answer_a")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Answer_b")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Answer_c")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Answer_d")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("CorrectAnswer")
                        .HasColumnType("int");

                    b.Property<string>("Explanation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MultipleChoiceTestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QuestionId");

                    b.HasIndex("MultipleChoiceTestId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.UserLearningStats", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CurrentStreak")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastLearningDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LongestStreak")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalLearningHours")
                        .HasColumnType("decimal(5, 1)");

                    b.HasKey("UserId");

                    b.ToTable("UserLearningStats");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.WritingExercise", b =>
                {
                    b.Property<Guid>("WritingExerciseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AIFeedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("LearningLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NativeLanguage")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("WritingExerciseId");

                    b.HasIndex("UserId");

                    b.ToTable("WritingExercises");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.ContentReport", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithMany("ContentReports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.Flashcard", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.FlashcardSet", "FlashcardSet")
                        .WithMany("Flashcards")
                        .HasForeignKey("FlashcardSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FlashcardSet");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.FlashcardReview", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.FlashcardSet", "FlashcardSet")
                        .WithMany("FlashcardReviews")
                        .HasForeignKey("FlashcardSetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithMany("FlashcardReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FlashcardSet");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.FlashcardSet", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithMany("FlashcardSets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.MultipleChoiceTest", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithMany("MultipleChoiceTests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.Question", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.MultipleChoiceTest", "MultipleChoiceTest")
                        .WithMany("Questions")
                        .HasForeignKey("MultipleChoiceTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MultipleChoiceTest");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.UserLearningStats", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithOne("UserLearningStats")
                        .HasForeignKey("WordWise.Api.Models.Domain.UserLearningStats", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.WritingExercise", b =>
                {
                    b.HasOne("WordWise.Api.Models.Domain.ExtendedIdentityUser", "User")
                        .WithMany("WritingExercises")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.ExtendedIdentityUser", b =>
                {
                    b.Navigation("ContentReports");

                    b.Navigation("FlashcardReviews");

                    b.Navigation("FlashcardSets");

                    b.Navigation("MultipleChoiceTests");

                    b.Navigation("UserLearningStats")
                        .IsRequired();

                    b.Navigation("WritingExercises");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.FlashcardSet", b =>
                {
                    b.Navigation("FlashcardReviews");

                    b.Navigation("Flashcards");
                });

            modelBuilder.Entity("WordWise.Api.Models.Domain.MultipleChoiceTest", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
