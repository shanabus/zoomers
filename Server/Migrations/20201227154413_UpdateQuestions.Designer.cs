﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZoomersClient.Shared.Data;

namespace ZoomersClient.Server.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20201227154413_UpdateQuestions")]
    partial class UpdateQuestions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("ZoomersClient.Shared.Models.AnsweredQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Answer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentPlayerAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Guess")
                        .HasColumnType("int");

                    b.Property<Guid?>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("QuestionId")
                        .HasColumnType("int");

                    b.Property<int>("Round")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.AudienceScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<Guid>("FromPlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Round")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<Guid>("ToPlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("AudienceScore");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CurrentRound")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Party")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rounds")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Voice")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.GameQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CategoriesString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionBaseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BackgroundColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CorrectGuesses")
                        .HasColumnType("int");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("HateReactions")
                        .HasColumnType("int");

                    b.Property<int>("HateScore")
                        .HasColumnType("int");

                    b.Property<int>("Icon")
                        .HasColumnType("int");

                    b.Property<int>("LoveReactions")
                        .HasColumnType("int");

                    b.Property<int>("LoveScore")
                        .HasColumnType("int");

                    b.Property<bool>("OnDeck")
                        .HasColumnType("bit");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<string>("Sound")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.QuestionBase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CategoriesString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AllQuestions");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.AnsweredQuestion", b =>
                {
                    b.HasOne("ZoomersClient.Shared.Models.Game", null)
                        .WithMany("AnsweredQuestions")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZoomersClient.Shared.Models.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");

                    b.HasOne("ZoomersClient.Shared.Models.GameQuestion", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId");

                    b.Navigation("Player");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.AudienceScore", b =>
                {
                    b.HasOne("ZoomersClient.Shared.Models.Game", null)
                        .WithMany("AudienceScore")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.GameQuestion", b =>
                {
                    b.HasOne("ZoomersClient.Shared.Models.Game", null)
                        .WithMany("Questions")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.Player", b =>
                {
                    b.HasOne("ZoomersClient.Shared.Models.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ZoomersClient.Shared.Models.Game", b =>
                {
                    b.Navigation("AnsweredQuestions");

                    b.Navigation("AudienceScore");

                    b.Navigation("Players");

                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
