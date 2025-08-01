﻿// <auto-generated />
using System;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameStore.Migrations
{
    [DbContext(typeof(GameStoreContext))]
    [Migration("20250729103411_usersRoleString")]
    partial class usersRoleString
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.7");

            modelBuilder.Entity("GameStore.Shared.Models.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GenreId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.HasKey("GameId");

                    b.HasIndex("GenreId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GameStore.Shared.Models.GameRating", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "GameId");

                    b.HasIndex("GameId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("GameStore.Shared.Models.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GenreId");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            GenreId = 1,
                            Name = "Fighting"
                        },
                        new
                        {
                            GenreId = 2,
                            Name = "Adventure"
                        },
                        new
                        {
                            GenreId = 3,
                            Name = "Platformer"
                        },
                        new
                        {
                            GenreId = 4,
                            Name = "Shooter"
                        },
                        new
                        {
                            GenreId = 5,
                            Name = "RPG"
                        },
                        new
                        {
                            GenreId = 6,
                            Name = "Sandbox"
                        },
                        new
                        {
                            GenreId = 7,
                            Name = "Action"
                        },
                        new
                        {
                            GenreId = 8,
                            Name = "Action RPG"
                        },
                        new
                        {
                            GenreId = 9,
                            Name = "Puzzle"
                        },
                        new
                        {
                            GenreId = 10,
                            Name = "Strategy"
                        });
                });

            modelBuilder.Entity("GameStore.Shared.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GameStore.Shared.Models.Game", b =>
                {
                    b.HasOne("GameStore.Shared.Models.Genre", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("GameStore.Shared.Models.GameRating", b =>
                {
                    b.HasOne("GameStore.Shared.Models.Game", "GameDetails")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameStore.Shared.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameDetails");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
