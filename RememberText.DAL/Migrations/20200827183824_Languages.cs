using Microsoft.EntityFrameworkCore.Migrations;

namespace RememberText.DAL.Migrations
{
    public partial class Languages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangCode = table.Column<string>(maxLength: 50, nullable: false),
                    LangName = table.Column<string>(maxLength: 256, nullable: false),
                    PrimaryLang = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
