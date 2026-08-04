using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeGroupTeacherIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "teacher_id",
                table: "group",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.Sql(
                "ALTER TABLE \"group\" DROP CONSTRAINT IF EXISTS group_teacher_id_not_null;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "teacher_id",
                table: "group",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.Sql(
                "ALTER TABLE \"group\" ADD CONSTRAINT group_teacher_id_not_null CHECK (teacher_id IS NOT NULL);");
        }
    }
}
