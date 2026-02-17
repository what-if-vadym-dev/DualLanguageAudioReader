using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamPrepar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferencesAndLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Transcripts",
                type: "TEXT",
                maxLength: 3,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetLanguage = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    NativeLanguage = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Level = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Transcripts");
        }
    }
}
