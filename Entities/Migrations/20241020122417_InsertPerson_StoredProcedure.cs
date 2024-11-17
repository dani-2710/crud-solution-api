using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            CREATE PROCEDURE [dbo].[InsertPersons]
            (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40), @DateOfBirth datetime2(7), @Gender nvarchar(10), @Address nvarchar(200), @CountryID uniqueidentifier, @ReceiveNewsLetters bit)
            AS BEGIN
                INSERT INTO [dbo].[Persons](PersonID, PersonName, Email, DateOfBirth, Gender, Address, CountryID, ReceiveNewsLetters) VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @Address, @CountryID, @ReceiveNewsLetters)
            END
            ";

            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string dp_InsertPerson = @"
            DROP PROCEDURE [dbo].[InsertPersons]
            ";

            migrationBuilder.Sql(dp_InsertPerson);
        }
    }
}
