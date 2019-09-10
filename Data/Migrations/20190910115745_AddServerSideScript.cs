using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshBoard.Data.Migrations
{
    public partial class AddServerSideScript : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServerSideScript",
                table: "Problem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerSideScript",
                table: "Problem");
        }
    }
}
