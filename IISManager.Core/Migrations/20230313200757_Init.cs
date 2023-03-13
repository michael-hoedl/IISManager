using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISManager.Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPoolGroups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPoolGroups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AppPoolGroupAppPool",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AppPoolGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPoolGroupAppPool", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AppPoolGroupAppPool_AppPoolGroups_AppPoolGroupName",
                        column: x => x.AppPoolGroupName,
                        principalTable: "AppPoolGroups",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPoolGroupAppPool_AppPoolGroupName",
                table: "AppPoolGroupAppPool",
                column: "AppPoolGroupName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPoolGroupAppPool");

            migrationBuilder.DropTable(
                name: "AppPoolGroups");
        }
    }
}
