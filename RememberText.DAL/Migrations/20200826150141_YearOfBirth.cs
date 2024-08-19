using Microsoft.EntityFrameworkCore.Migrations;

namespace RememberText.DAL.Migrations
{
    public partial class YearOfBirth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfBirth",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 1974);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfBirth",
                table: "AspNetUsers");
        }
    }
}
