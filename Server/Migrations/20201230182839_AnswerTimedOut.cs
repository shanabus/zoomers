using Microsoft.EntityFrameworkCore.Migrations;

namespace ZoomersClient.Server.Migrations
{
    public partial class AnswerTimedOut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AnswerTimedOut",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerTimedOut",
                table: "Answers");
        }
    }
}
