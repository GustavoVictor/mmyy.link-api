using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_tink_link.Migrations
{
    public partial class Addusersummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "USERS",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "USERS");
        }
    }
}
