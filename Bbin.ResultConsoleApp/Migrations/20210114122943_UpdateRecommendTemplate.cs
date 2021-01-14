using Microsoft.EntityFrameworkCore.Migrations;

namespace Bbin.ResultConsoleApp.Migrations
{
    public partial class UpdateRecommendTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "RecommendTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                table: "RecommendTemplate");
        }
    }
}
