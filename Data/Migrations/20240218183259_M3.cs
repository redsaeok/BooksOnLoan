using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksOnLoan.Data.Migrations
{
    /// <inheritdoc />
    public partial class M3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BookCodeNumber",
                table: "Transaction",
                column: "BookCodeNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Book_BookCodeNumber",
                table: "Transaction",
                column: "BookCodeNumber",
                principalTable: "Book",
                principalColumn: "CodeNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Book_BookCodeNumber",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_BookCodeNumber",
                table: "Transaction");
        }
    }
}
