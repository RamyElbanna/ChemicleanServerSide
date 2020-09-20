using Microsoft.EntityFrameworkCore.Migrations;

namespace ChemicleanProject.Migrations
{
    public partial class addingsomecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Products");
        }
    }
}
