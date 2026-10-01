using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "admin_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    public_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_admin_users_admin_users_created_by",
                        column: x => x.created_by,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_admin_users_admin_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_created_by",
                table: "admin_users",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_email",
                table: "admin_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_public_id",
                table: "admin_users",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_updated_by",
                table: "admin_users",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_user_name",
                table: "admin_users",
                column: "user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_users");
        }
    }
}
