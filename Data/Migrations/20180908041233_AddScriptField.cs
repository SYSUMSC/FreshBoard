using Microsoft.EntityFrameworkCore.Migrations;

namespace mscfreshman.Data.Migrations
{
    public partial class AddScriptField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Script",
                table: "Problem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Script",
                table: "Problem");
        }
    }
}
