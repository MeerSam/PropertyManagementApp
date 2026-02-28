using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAndAddPropertyOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK if it exists
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=off;

                CREATE TABLE IF NOT EXISTS __tmp_PropertyOwnerships AS 
                SELECT * FROM PropertyOwnerships;

                PRAGMA foreign_keys=on;
            ");

            // Drop the FK constraint (SQLite requires table rebuild)
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=off;

                CREATE TABLE PropertyOwnerships_new AS
                SELECT Id, PropertyId, MemberId, StartDate, EndDate, OwnershipType, OwnershipPercentage, IsCurrent
                FROM PropertyOwnerships;

                DROP TABLE PropertyOwnerships;

                ALTER TABLE PropertyOwnerships_new RENAME TO PropertyOwnerships;

                PRAGMA foreign_keys=on;
            ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
