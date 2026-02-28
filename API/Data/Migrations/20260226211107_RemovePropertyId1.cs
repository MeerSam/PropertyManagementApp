using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePropertyId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyOwnerships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<string>(type: "TEXT", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OwnershipType = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnershipPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false) 
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
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyOwnerships");
        }
    }
}
