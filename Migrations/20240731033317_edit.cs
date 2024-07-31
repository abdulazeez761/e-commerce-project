using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceWebsite.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Code_Products_ProductID",
                table: "Code");

            migrationBuilder.DropIndex(
                name: "IX_Code_ProductID",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "Code",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Code_ProductID",
                table: "Code",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_Code_Products_ProductID",
                table: "Code",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
