using Microsoft.EntityFrameworkCore.Migrations;

namespace FreshBoard.Data.Migrations
{
    public partial class ProvideOrderForApplicationPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "Order",
                table: "ApplicationPeriod",
                nullable: false,
                defaultValue: 0u)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPeriod_Order",
                table: "ApplicationPeriod",
                column: "Order",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationPeriod_Order",
                table: "ApplicationPeriod");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ApplicationPeriod");
        }
    }
}
