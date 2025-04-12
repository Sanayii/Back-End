using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanayii.Migrations
{
    /// <inheritdoc />
    public partial class CustomerDiscountt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDiscount_Customer_CustomerId",
                table: "CustomerDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDiscount_Discount_DiscountId",
                table: "CustomerDiscount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerDiscount",
                table: "CustomerDiscount");

            migrationBuilder.RenameTable(
                name: "CustomerDiscount",
                newName: "CustomerDiscounts");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerDiscount_DiscountId",
                table: "CustomerDiscounts",
                newName: "IX_CustomerDiscounts_DiscountId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateGiven",
                table: "CustomerDiscounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerDiscounts",
                table: "CustomerDiscounts",
                columns: new[] { "CustomerId", "DiscountId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDiscounts_Customer_CustomerId",
                table: "CustomerDiscounts",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDiscounts_Discount_DiscountId",
                table: "CustomerDiscounts",
                column: "DiscountId",
                principalTable: "Discount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDiscounts_Customer_CustomerId",
                table: "CustomerDiscounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDiscounts_Discount_DiscountId",
                table: "CustomerDiscounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerDiscounts",
                table: "CustomerDiscounts");

            migrationBuilder.DropColumn(
                name: "DateGiven",
                table: "CustomerDiscounts");

            migrationBuilder.RenameTable(
                name: "CustomerDiscounts",
                newName: "CustomerDiscount");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerDiscounts_DiscountId",
                table: "CustomerDiscount",
                newName: "IX_CustomerDiscount_DiscountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerDiscount",
                table: "CustomerDiscount",
                columns: new[] { "CustomerId", "DiscountId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDiscount_Customer_CustomerId",
                table: "CustomerDiscount",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDiscount_Discount_DiscountId",
                table: "CustomerDiscount",
                column: "DiscountId",
                principalTable: "Discount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
