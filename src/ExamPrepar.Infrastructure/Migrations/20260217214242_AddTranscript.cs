using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamPrepar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TranscriptId",
                table: "Segments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Transcripts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetLanguage = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    NativeLanguage = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transcripts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Segments_TranscriptId",
                table: "Segments",
                column: "TranscriptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Segments_Transcripts_TranscriptId",
                table: "Segments",
                column: "TranscriptId",
                principalTable: "Transcripts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Segments_Transcripts_TranscriptId",
                table: "Segments");

            migrationBuilder.DropTable(
                name: "Transcripts");

            migrationBuilder.DropIndex(
                name: "IX_Segments_TranscriptId",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "TranscriptId",
                table: "Segments");
        }
    }
}
