using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptiva.Migrations
{
    /// <inheritdoc />
    public partial class AddedHoldings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortfolioModelId",
                table: "CryptoHoldings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldings_PortfolioModelId",
                table: "CryptoHoldings",
                column: "PortfolioModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoHoldings_Portfolio_PortfolioModelId",
                table: "CryptoHoldings",
                column: "PortfolioModelId",
                principalTable: "Portfolio",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoHoldings_Portfolio_PortfolioModelId",
                table: "CryptoHoldings");

            migrationBuilder.DropIndex(
                name: "IX_CryptoHoldings_PortfolioModelId",
                table: "CryptoHoldings");

            migrationBuilder.DropColumn(
                name: "PortfolioModelId",
                table: "CryptoHoldings");
        }
    }
}
