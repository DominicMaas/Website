﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Website.Common;

#nullable disable

namespace Website.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240103205652_AddImages")]
    partial class AddImages
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("GalleryImage", b =>
                {
                    b.Property<Guid>("GalleriesId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ImagesId")
                        .HasColumnType("TEXT");

                    b.HasKey("GalleriesId", "ImagesId");

                    b.HasIndex("ImagesId");

                    b.ToTable("GalleryImage");
                });

            modelBuilder.Entity("ImageStreamPost", b =>
                {
                    b.Property<Guid>("AttachedImagesId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StreamsId")
                        .HasColumnType("TEXT");

                    b.HasKey("AttachedImagesId", "StreamsId");

                    b.HasIndex("StreamsId");

                    b.ToTable("ImageStreamPost");
                });

            modelBuilder.Entity("Website.Models.Database.Gallery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Galleries");
                });

            modelBuilder.Entity("Website.Models.Database.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateTaken")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateUploaded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("R2Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Website.Models.Database.ShortLink", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("RedirectLink")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ShortLinks");
                });

            modelBuilder.Entity("Website.Models.Database.StreamPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Posted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Streams");
                });

            modelBuilder.Entity("GalleryImage", b =>
                {
                    b.HasOne("Website.Models.Database.Gallery", null)
                        .WithMany()
                        .HasForeignKey("GalleriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Website.Models.Database.Image", null)
                        .WithMany()
                        .HasForeignKey("ImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ImageStreamPost", b =>
                {
                    b.HasOne("Website.Models.Database.Image", null)
                        .WithMany()
                        .HasForeignKey("AttachedImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Website.Models.Database.StreamPost", null)
                        .WithMany()
                        .HasForeignKey("StreamsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
