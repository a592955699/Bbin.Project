﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Bbin.ResultConsoleApp.Migrations
{
    public partial class Db_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoomId = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    Date = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "RecommendItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RecommendTemplateId = table.Column<int>(nullable: false),
                    ResultState = table.Column<int>(nullable: false),
                    Times = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Describe = table.Column<string>(nullable: true),
                    Publish = table.Column<bool>(nullable: false),
                    Sort = table.Column<int>(nullable: false),
                    RecommendType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    Rs = table.Column<string>(nullable: false),
                    GameId = table.Column<long>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    Rn = table.Column<string>(nullable: true),
                    Card1 = table.Column<string>(nullable: true),
                    Card2 = table.Column<string>(nullable: true),
                    Card3 = table.Column<string>(nullable: true),
                    Card4 = table.Column<string>(nullable: true),
                    Card5 = table.Column<string>(nullable: true),
                    Card6 = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    ResultState = table.Column<int>(nullable: false),
                    Begin = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.Rs);
                    table.ForeignKey(
                        name: "FK_Result_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Result_GameId",
                table: "Result",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Result_Index",
                table: "Result",
                column: "Index");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendItem");

            migrationBuilder.DropTable(
                name: "RecommendTemplate");

            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "Game");
        }
    }
}
