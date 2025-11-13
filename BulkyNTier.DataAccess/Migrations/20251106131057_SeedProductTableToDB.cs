using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyNTier.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductTableToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 101, "J.K Rowling", "Harry Potter In a Magical world looking to find out sirius Black", "127h32g24", 100.0, 90.0, 40.0, 60.0, "Harry Potter And the Deathly Hallows" },
                    { 102, "Napolean Hill", "A Schlor trying to teach Economics", "12843jur", 300.0, 250.0, 150.0, 200.0, "Think and Grow Rich" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 102);
        }
    }
}
