using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_tink_link.Migrations
{
    public partial class add_social_prop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Social",
                table: "CARDS",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Social",
                table: "CARDS");
        }
    }
}
