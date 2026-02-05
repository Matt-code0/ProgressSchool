using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progress.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToWorkoutTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkoutTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkoutTemplates");
        }
    }
}
