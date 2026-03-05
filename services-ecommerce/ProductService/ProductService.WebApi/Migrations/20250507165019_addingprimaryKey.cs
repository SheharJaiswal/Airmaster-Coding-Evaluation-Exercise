using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addingprimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "0a34fc8e-c4f0-49ae-9a22-f3cbffcb4a49");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "7cee35fc-cd8b-4e76-b35e-19cf8ef03928");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "7ecbd585-22b5-4db5-8047-d97b6a5a330f");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "Name", "Price", "StockQuantity", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "40defbf7-2710-457e-a356-69b3a77c9a68", "Electronics", new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5927), "Seed", "16GB RAM, 1TB SSD, Intel i7", "Premium Laptop", 1299.99m, 50, new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5928), "Seed" },
                    { "a3683810-021c-4110-86db-651268830538", "Accessories", new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5948), "Seed", "RGB backlight, Cherry MX switches", "Mechanical Keyboard", 89.99m, 75, new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5948), "Seed" },
                    { "f482fe18-dad7-4fd9-ae1e-a49b7be4b4d8", "Accessories", new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5935), "Seed", "Ergonomic design, 2.4GHz wireless", "Wireless Mouse", 29.99m, 200, new DateTime(2025, 5, 7, 16, 50, 19, 149, DateTimeKind.Utc).AddTicks(5935), "Seed" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "40defbf7-2710-457e-a356-69b3a77c9a68");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "a3683810-021c-4110-86db-651268830538");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "f482fe18-dad7-4fd9-ae1e-a49b7be4b4d8");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "Name", "Price", "StockQuantity", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "0a34fc8e-c4f0-49ae-9a22-f3cbffcb4a49", "Electronics", new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1660), "Seed", "16GB RAM, 1TB SSD, Intel i7", "Premium Laptop", 1299.99m, 50, new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1661), "Seed" },
                    { "7cee35fc-cd8b-4e76-b35e-19cf8ef03928", "Accessories", new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1672), "Seed", "RGB backlight, Cherry MX switches", "Mechanical Keyboard", 89.99m, 75, new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1673), "Seed" },
                    { "7ecbd585-22b5-4db5-8047-d97b6a5a330f", "Accessories", new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1669), "Seed", "Ergonomic design, 2.4GHz wireless", "Wireless Mouse", 29.99m, 200, new DateTime(2025, 5, 7, 16, 45, 15, 151, DateTimeKind.Utc).AddTicks(1669), "Seed" }
                });
        }
    }
}
