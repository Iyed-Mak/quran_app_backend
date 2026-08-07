using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseBackupFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "backup_audit_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    backup_id = table.Column<int>(type: "integer", nullable: true),
                    backup_file_name = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "text", nullable: false),
                    performed_by_id = table.Column<int>(type: "integer", nullable: true),
                    performed_by_name = table.Column<string>(type: "text", nullable: false),
                    performed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_backup_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "database_backup_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    frequency = table.Column<string>(type: "text", nullable: false),
                    backup_time = table.Column<string>(type: "text", nullable: false),
                    max_backups_to_keep = table.Column<int>(type: "integer", nullable: false),
                    last_run_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    next_run_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_database_backup_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "database_backups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    backup_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_by_name = table.Column<string>(type: "text", nullable: false),
                    restore_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    restored_by = table.Column<int>(type: "integer", nullable: true),
                    restore_status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_database_backups", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "backup_audit_logs");

            migrationBuilder.DropTable(
                name: "database_backup_settings");

            migrationBuilder.DropTable(
                name: "database_backups");
        }
    }
}
