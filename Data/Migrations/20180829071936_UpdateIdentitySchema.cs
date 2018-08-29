using Microsoft.EntityFrameworkCore.Migrations;

namespace mscfreshman.Data.Migrations
{
    public partial class UpdateIdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WeChat",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeChat",
                table: "AspNetUsers");
        }
    }
}
