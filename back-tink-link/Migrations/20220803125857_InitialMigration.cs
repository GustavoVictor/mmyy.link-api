using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_tink_link.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NickName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EmailValidationCode = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidatedEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    Roles = table.Column<string>(type: "TEXT", maxLength: 1200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()"),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CARDS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", maxLength: 3, nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 70, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 70, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    URL = table.Column<string>(type: "TEXT", maxLength: 800, nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETDATE()"),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CARDS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CARDS_UserId",
                table: "CARDS",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CARDS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
