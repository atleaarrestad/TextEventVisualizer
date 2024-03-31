﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TextEventVisualizer.Data;

#nullable disable

namespace TextEventVisualizer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240331222827_timeline_name")]
    partial class timeline_name
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("TextEventVisualizer.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Authors")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("HasBeenScraped")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<bool>("UrlDoesntExistAnymore")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WebUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Category");

                    b.HasIndex("Date");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TimelineChunkId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Timestamp")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TimelineChunkId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Request.TimelineRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArticleClusterSearch")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DesiredEventCountForEachArticle")
                        .HasColumnType("INTEGER");

                    b.Property<float>("MaxArticleClusterSearchDistance")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxArticleCount")
                        .HasColumnType("INTEGER");

                    b.Property<float>("MaxDistanceDeltaForArticles")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxEventCountForEachArticle")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TimelineRequests");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Timeline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TimelineRequestId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TimelineRequestId")
                        .IsUnique();

                    b.ToTable("Timelines");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.TimelineChunk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ArticleId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimelineId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TimelineId");

                    b.ToTable("TimelineChunks");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Event", b =>
                {
                    b.HasOne("TextEventVisualizer.Models.TimelineChunk", "TimelineChunk")
                        .WithMany("Events")
                        .HasForeignKey("TimelineChunkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TimelineChunk");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Request.TimelineRequest", b =>
                {
                    b.OwnsOne("TextEventVisualizer.Models.Bias", "ArticleClusterSearchNegativeBias", b1 =>
                        {
                            b1.Property<int>("TimelineRequestId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Concepts")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<float>("Force")
                                .HasColumnType("REAL")
                                .HasColumnName("NegativeBiasForce");

                            b1.HasKey("TimelineRequestId");

                            b1.ToTable("TimelineRequests");

                            b1.WithOwner()
                                .HasForeignKey("TimelineRequestId");
                        });

                    b.OwnsOne("TextEventVisualizer.Models.Bias", "ArticleClusterSearchPositiveBias", b1 =>
                        {
                            b1.Property<int>("TimelineRequestId")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Concepts")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<float>("Force")
                                .HasColumnType("REAL")
                                .HasColumnName("PositiveBiasForce");

                            b1.HasKey("TimelineRequestId");

                            b1.ToTable("TimelineRequests");

                            b1.WithOwner()
                                .HasForeignKey("TimelineRequestId");
                        });

                    b.Navigation("ArticleClusterSearchNegativeBias")
                        .IsRequired();

                    b.Navigation("ArticleClusterSearchPositiveBias")
                        .IsRequired();
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Timeline", b =>
                {
                    b.HasOne("TextEventVisualizer.Models.Request.TimelineRequest", "TimelineRequest")
                        .WithOne("Timeline")
                        .HasForeignKey("TextEventVisualizer.Models.Timeline", "TimelineRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TimelineRequest");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.TimelineChunk", b =>
                {
                    b.HasOne("TextEventVisualizer.Models.Timeline", "Timeline")
                        .WithMany("TimelineChunks")
                        .HasForeignKey("TimelineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Timeline");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Request.TimelineRequest", b =>
                {
                    b.Navigation("Timeline")
                        .IsRequired();
                });

            modelBuilder.Entity("TextEventVisualizer.Models.Timeline", b =>
                {
                    b.Navigation("TimelineChunks");
                });

            modelBuilder.Entity("TextEventVisualizer.Models.TimelineChunk", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
