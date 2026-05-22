using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class PropertyAddressFieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSameMailddress",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MailAddress",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailCity",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailCountry",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailState",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailUnit",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailZipCode",
                table: "Properties",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsSameMailddress",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailAddress",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailCity",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailCountry",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailState",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailUnit",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MailZipCode",
                table: "Properties");
        }
    }
}
