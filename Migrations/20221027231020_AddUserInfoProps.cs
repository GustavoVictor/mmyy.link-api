using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_tink_link.Migrations
{
    public partial class AddUserInfoProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "USERS",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "USERS",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "USERS");
        }
    }
}
