using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteToExerciseSeriesLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ExerciseSeriesLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ExerciseSeriesLogs");
        }
    }
}
