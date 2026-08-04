using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranSchool.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherRegistrationNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "registration_number",
                table: "teacher",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // ترقيم المعلمين النشطين الحاليين بالتسلسل (1..N) حسب المعرّف،
            // ليبدأ كل معلم جديد برقم تالي تلقائيًا.
            migrationBuilder.Sql("""
                WITH numbered AS (
                    SELECT id, ROW_NUMBER() OVER (ORDER BY id) AS rn
                    FROM teacher
                    WHERE is_active = true
                )
                UPDATE teacher t
                SET registration_number = numbered.rn
                FROM numbered
                WHERE t.id = numbered.id;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "registration_number",
                table: "teacher");
        }
    }
}
