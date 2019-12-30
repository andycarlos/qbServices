using Microsoft.EntityFrameworkCore.Migrations;

namespace qbService.Migrations
{
    public partial class saleOrder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderConfigModel_AspNetUsers_ApplicationUserId",
                table: "SaleOrderConfigModel");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderConfigModel_ApplicationUserId",
                table: "SaleOrderConfigModel");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "SaleOrderConfigModel",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderConfigModel_ApplicationUserId",
                table: "SaleOrderConfigModel",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderConfigModel_AspNetUsers_ApplicationUserId",
                table: "SaleOrderConfigModel",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderConfigModel_AspNetUsers_ApplicationUserId",
                table: "SaleOrderConfigModel");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderConfigModel_ApplicationUserId",
                table: "SaleOrderConfigModel");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "SaleOrderConfigModel",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderConfigModel_ApplicationUserId",
                table: "SaleOrderConfigModel",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderConfigModel_AspNetUsers_ApplicationUserId",
                table: "SaleOrderConfigModel",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
