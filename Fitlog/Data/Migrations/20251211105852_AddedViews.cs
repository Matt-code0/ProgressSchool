using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkoutTemplates",
                newName: "Split");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Split",
                table: "WorkoutTemplates",
                newName: "Description");
        }
    }
}
