using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZoomersClient.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: true),
                    Round = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentPlayerAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guess = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AudienceScore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    FromPlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToPlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceScore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<int>(type: "int", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sound = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    LoveScore = table.Column<int>(type: "int", nullable: false),
                    HateScore = table.Column<int>(type: "int", nullable: false),
                    LoveReactions = table.Column<int>(type: "int", nullable: false),
                    HateReactions = table.Column<int>(type: "int", nullable: false),
                    CorrectGuesses = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rounds = table.Column<int>(type: "int", nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Party = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentPlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_CurrentPlayerId",
                        column: x => x.CurrentPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriesString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_GameId",
                table: "Answers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_PlayerId",
                table: "Answers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AudienceScore_GameId",
                table: "AudienceScore",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentPlayerId",
                table: "Games",
                column: "CurrentPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameId",
                table: "Questions",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Games_GameId",
                table: "Answers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Players_PlayerId",
                table: "Answers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceScore_Games_GameId",
                table: "AudienceScore",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AudienceScore");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
