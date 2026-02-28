using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class checkChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyOwnerships");

            migrationBuilder.DropTable(
                name: "Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Bathrooms = table.Column<int>(type: "INTEGER", nullable: true),
                    Bedrooms = table.Column<int>(type: "INTEGER", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRented = table.Column<bool>(type: "INTEGER", nullable: false),
                    LotNumber = table.Column<string>(type: "TEXT", nullable: true),
                    SquareFeet = table.Column<int>(type: "INTEGER", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyOwnerships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<string>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    OwnershipPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    OwnershipType = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId1 = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyOwnerships_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyOwnerships_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyOwnerships_Properties_PropertyId1",
                        column: x => x.PropertyId1,
                        principalTable: "Properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ClientId",
                table: "Properties",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwnerships_MemberId",
                table: "PropertyOwnerships",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwnerships_PropertyId_OwnershipType_IsCurrent",
                table: "PropertyOwnerships",
                columns: new[] { "PropertyId", "OwnershipType", "IsCurrent" },
                unique: true,
                filter: "\"IsCurrent\" = true AND \"OwnershipType\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwnerships_PropertyId1",
                table: "PropertyOwnerships",
                column: "PropertyId1");
        }
    }
}
