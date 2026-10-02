using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AllowDeletedInactiveUserIdentityReuse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_admin_users_email",
                table: "admin_users");

            migrationBuilder.DropIndex(
                name: "ix_admin_users_user_name",
                table: "admin_users");

            migrationBuilder.AddColumn<string>(
                name: "unique_email",
                table: "admin_users",
                type: "varchar(254)",
                maxLength: 254,
                nullable: true,
                computedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE email END",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "unique_user_name",
                table: "admin_users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                computedColumnSql: "CASE WHEN is_deleted = 1 AND is_active = 0 THEN NULL ELSE user_name END",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_unique_email",
                table: "admin_users",
                column: "unique_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_unique_user_name",
                table: "admin_users",
                column: "unique_user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_admin_users_unique_email",
                table: "admin_users");

            migrationBuilder.DropIndex(
                name: "ix_admin_users_unique_user_name",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "unique_email",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "unique_user_name",
                table: "admin_users");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_email",
                table: "admin_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_user_name",
                table: "admin_users",
                column: "user_name",
                unique: true);
        }
    }
}
