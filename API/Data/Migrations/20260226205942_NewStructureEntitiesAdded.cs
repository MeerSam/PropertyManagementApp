using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewStructureEntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Users_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClientAccess_Users_UserId",
                table: "UserClientAccess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClientAccess",
                table: "UserClientAccess");

            migrationBuilder.DropIndex(
                name: "IX_UserClientAccess_UserId",
                table: "UserClientAccess");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserClientAccess");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserClientAccess",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "UserClientAccess",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RefreshTokenIsRevoked",
                table: "UserClientAccess",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Members",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Members",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClientAccess",
                table: "UserClientAccess",
                columns: new[] { "UserId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Members_AppUserId",
                table: "Members",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Users_AppUserId",
                table: "Members",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Users_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClientAccess_Users_UserId",
                table: "UserClientAccess",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Users_AppUserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Users_UserId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClientAccess_Users_UserId",
                table: "UserClientAccess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClientAccess",
                table: "UserClientAccess");

            migrationBuilder.DropIndex(
                name: "IX_Members_AppUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserClientAccess");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "UserClientAccess");

            migrationBuilder.DropColumn(
                name: "RefreshTokenIsRevoked",
                table: "UserClientAccess");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserClientAccess",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Members",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClientAccess",
                table: "UserClientAccess",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserClientAccess_UserId",
                table: "UserClientAccess",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Users_UserId",
                table: "Members",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClientAccess_Users_UserId",
                table: "UserClientAccess",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
