using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Category = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
