using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bbin.ResultWebApp.Migrations
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    GameId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    ResultId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<long>(type: "bigint", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Rn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    ResultState = table.Column<int>(type: "int", nullable: false),
                    Begin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Result_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_RoomId_Index_Date",
                table: "Game",
                columns: new[] { "RoomId", "Index", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Result_GameId",
                table: "Result",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "Game");
        }
    }
}
