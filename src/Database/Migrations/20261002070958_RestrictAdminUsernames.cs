using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RestrictAdminUsernames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "admin_users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "unique_user_name",
                table: "admin_users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                computedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE user_name END",
                stored: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComputedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE user_name END",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "admin_users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "unique_user_name",
                table: "admin_users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                computedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE user_name END",
                stored: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComputedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE user_name END",
                oldStored: true);
        }
    }
}
