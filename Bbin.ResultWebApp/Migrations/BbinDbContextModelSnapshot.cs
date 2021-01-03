﻿// <auto-generated />
using System;
using Bbin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bbin.ResultWebApp.Migrations
{
    [DbContext(typeof(BbinDbContext))]
    partial class BbinDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Bbin.Api.Baccarat.Entitys.GameEntity", b =>
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

            modelBuilder.Entity("Bbin.Api.Baccarat.Entitys.ResultEntity", b =>
                {
                    b.Property<long>("ResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

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

                    b.Property<string>("Rs")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ResultId");

                    b.HasIndex("GameId");

                    b.ToTable("Result");
                });

            modelBuilder.Entity("Bbin.Api.Baccarat.Entitys.ResultEntity", b =>
                {
                    b.HasOne("Bbin.Api.Baccarat.Entitys.GameEntity", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.Navigation("Game");
                });
#pragma warning restore 612, 618
        }
    }
}
