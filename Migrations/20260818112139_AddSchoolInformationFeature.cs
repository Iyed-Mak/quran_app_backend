using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolInformationFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "school_information",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    school_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    founded_year = table.Column<int>(type: "integer", nullable: false),
                    school_type = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    additional_phone = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    whatsapp = table.Column<string>(type: "text", nullable: true),
                    official_page = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_information", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "school_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    school_information_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_rules_school_information_school_information_id",
                        column: x => x.school_information_id,
                        principalTable: "school_information",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "school_working_hours",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    school_information_id = table.Column<int>(type: "integer", nullable: false),
                    day_of_week = table.Column<string>(type: "text", nullable: false),
                    is_open = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_working_hours", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_working_hours_school_information_school_information_",
                        column: x => x.school_information_id,
                        principalTable: "school_information",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "school_working_periods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    working_hours_id = table.Column<int>(type: "integer", nullable: false),
                    opening_time = table.Column<string>(type: "text", nullable: false),
                    closing_time = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_working_periods", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_working_periods_school_working_hours_working_hours_id",
                        column: x => x.working_hours_id,
                        principalTable: "school_working_hours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_school_rules_school_information_id_display_order",
                table: "school_rules",
                columns: new[] { "school_information_id", "display_order" });

            migrationBuilder.CreateIndex(
                name: "ix_school_working_hours_school_information_id",
                table: "school_working_hours",
                column: "school_information_id");

            migrationBuilder.CreateIndex(
                name: "ix_school_working_periods_working_hours_id",
                table: "school_working_periods",
                column: "working_hours_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "school_rules");

            migrationBuilder.DropTable(
                name: "school_working_periods");

            migrationBuilder.DropTable(
                name: "school_working_hours");

            migrationBuilder.DropTable(
                name: "school_information");
        }
    }
}
