using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class declaringKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "8efbc85d-ebef-44f3-93da-164ea8031564");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "e2e57c1f-ab39-45bc-9dd6-b3c1f326a107");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "edb0ef59-73aa-40a4-a1cd-20db1c053aad");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "8efbc85d-ebef-44f3-93da-164ea8031564", "Accessories", new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4711), "Seed", "Ergonomic design, 2.4GHz wireless", "Wireless Mouse", 29.99m, 200, new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4712), "Seed" },
                    { "e2e57c1f-ab39-45bc-9dd6-b3c1f326a107", "Electronics", new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4703), "Seed", "16GB RAM, 1TB SSD, Intel i7", "Premium Laptop", 1299.99m, 50, new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4705), "Seed" },
                    { "edb0ef59-73aa-40a4-a1cd-20db1c053aad", "Accessories", new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4714), "Seed", "RGB backlight, Cherry MX switches", "Mechanical Keyboard", 89.99m, 75, new DateTime(2025, 5, 6, 18, 50, 57, 846, DateTimeKind.Utc).AddTicks(4715), "Seed" }
                });
        }
    }
}
