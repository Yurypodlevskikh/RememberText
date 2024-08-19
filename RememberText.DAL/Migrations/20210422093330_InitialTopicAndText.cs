using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RememberText.DAL.Migrations
{
    public partial class InitialTopicAndText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    TopicTitle = table.Column<string>(maxLength: 256, nullable: false),
                    Primary = table.Column<string>(maxLength: 50, nullable: false),
                    Secondary = table.Column<string>(maxLength: 50, nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: true),
                    BanText = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnTexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextContent = table.Column<string>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    AuthorNickname = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: true),
                    Redactor = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnTexts_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuTexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextContent = table.Column<string>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    AuthorNickname = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: true),
                    Redactor = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RuTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuTexts_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SvTexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextContent = table.Column<string>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    AuthorNickname = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: true),
                    Redactor = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SvTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SvTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SvTexts_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnTexts_LanguageId",
                table: "EnTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_EnTexts_TopicId",
                table: "EnTexts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_RuTexts_LanguageId",
                table: "RuTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_RuTexts_TopicId",
                table: "RuTexts",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SvTexts_LanguageId",
                table: "SvTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SvTexts_TopicId",
                table: "SvTexts",
                column: "TopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnTexts");

            migrationBuilder.DropTable(
                name: "RuTexts");

            migrationBuilder.DropTable(
                name: "SvTexts");

            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
