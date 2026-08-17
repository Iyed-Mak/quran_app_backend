using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "separation_reason",
                table: "student",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "student",
                type: "text",
                nullable: false,
                defaultValue: "active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "separation_reason",
                table: "student");

            migrationBuilder.DropColumn(
                name: "status",
                table: "student");
        }
    }
}
