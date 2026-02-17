using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exe_Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaUrlToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Vouchers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Suppliers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Shifts",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Settings",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Reviews",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AddColumn<string>(
                name: "MediaUrl",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "PurchaseOrders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "PurchaseOrders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Products",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpVerifications",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Orders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LoyaltyPointsHistory",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "InventoryTransactions",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Expenses",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employees",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Customers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ContactMessages",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Categories",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Cart",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Blogs",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "BlogComments",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.CreateTable(
                name: "ChatHistories",
                columns: table => new
                {
                    ChatHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOrderRelated = table.Column<bool>(type: "bit", nullable: false),
                    OrderData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.ChatHistoryId);
                    table.ForeignKey(
                        name: "FK_ChatHistories_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_CustomerId",
                table: "ChatHistories",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "MediaUrl",
                table: "Reviews");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Vouchers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Suppliers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Shifts",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Settings",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Reviews",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "PurchaseOrders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "PurchaseOrders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Products",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpVerifications",
                type: "datetime",
                nullable: false,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Orders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LoyaltyPointsHistory",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "InventoryTransactions",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Expenses",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Employees",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Customers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ContactMessages",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Categories",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Cart",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Blogs",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "BlogComments",
                type: "datetime",
                nullable: true,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
