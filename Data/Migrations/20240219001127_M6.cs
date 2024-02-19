using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksOnLoan.Data.Migrations
{
    /// <inheritdoc />
    public partial class M6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Transaction",
                newName: "ReturnDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Transaction",
                newName: "LoanStartDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "HoldDate",
                table: "Transaction",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "LoanDueDate",
                table: "Transaction",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoldDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "LoanDueDate",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "Transaction",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "LoanStartDate",
                table: "Transaction",
                newName: "EndDate");
        }
    }
}
