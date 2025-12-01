using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class IsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LessonSlots_Start_End",
                table: "LessonSlots",
                columns: new[] { "Start", "End" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonSlots_Start_End",
                table: "LessonSlots");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Lessons");
        }
    }
}
