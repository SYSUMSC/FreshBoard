using Microsoft.EntityFrameworkCore.Migrations;

namespace mscfreshman.Data.Migrations
{
    public partial class AddAnswerField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "Problem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "Problem");
        }
    }
}
