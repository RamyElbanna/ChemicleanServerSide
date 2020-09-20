using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChemicleanProject.Migrations
{
    public partial class bytesAndUpdatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Bytes",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Products",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bytes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Products");
        }
    }
}
