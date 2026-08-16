using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBackupDirectorySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "directory",
                table: "database_backups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "backup_directory",
                table: "database_backup_settings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "directory",
                table: "database_backups");

            migrationBuilder.DropColumn(
                name: "backup_directory",
                table: "database_backup_settings");
        }
    }
}
