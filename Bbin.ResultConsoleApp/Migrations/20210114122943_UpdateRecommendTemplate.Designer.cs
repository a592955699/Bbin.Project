﻿// <auto-generated />
using System;
using Bbin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bbin.ResultConsoleApp.Migrations
{
    [DbContext(typeof(BbinDbContext))]
    [Migration("20210114122943_UpdateRecommendTemplate")]
    partial class UpdateRecommendTemplate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Bbin.Core.Entitys.GameEntity", b =>
                {
                    b.Property<long>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("Date")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<string>("RoomId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GameId");

                    b.HasIndex("RoomId", "Index", "Date");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("Bbin.Core.Entitys.RecommendItemEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("RecommendTemplateId")
                        .HasColumnType("int");

                    b.Property<int>("ResultState")
                        .HasColumnType("int");

                    b.Property<int>("Times")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RecommendItem");
                });

            modelBuilder.Entity("Bbin.Core.Entitys.RecommendTemplateEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Describe")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Publish")
                        .HasColumnType("bit");

                    b.Property<int>("RecommendType")
                        .HasColumnType("int");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RecommendTemplate");
                });

            modelBuilder.Entity("Bbin.Core.Entitys.ResultEntity", b =>
                {
                    b.Property<string>("Rs")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Begin")
                        .HasColumnType("datetime2");

                    b.Property<string>("Card1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Card6")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime");

                    b.Property<long?>("GameId")
                        .HasColumnType("bigint");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<int>("ResultState")
                        .HasColumnType("int");

                    b.Property<string>("Rn")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Rs");

                    b.HasIndex("GameId");

                    b.ToTable("Result");
                });

            modelBuilder.Entity("Bbin.Core.Entitys.ResultEntity", b =>
                {
                    b.HasOne("Bbin.Core.Entitys.GameEntity", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.Navigation("Game");
                });
#pragma warning restore 612, 618
        }
    }
}
