using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClientSelectionTokenChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedFromIpAddress",
                table: "ClientSelectionTokens",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenIdentifier",
                table: "ClientSelectionTokens",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedFromIpAddress",
                table: "ClientSelectionTokens");

            migrationBuilder.DropColumn(
                name: "TokenIdentifier",
                table: "ClientSelectionTokens");
        }
    }
}
