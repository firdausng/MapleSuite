using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace members.api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Biographical_DateOfBirth = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Biographical_FirstName = table.Column<string>(type: "text", nullable: false),
                    Biographical_LastName = table.Column<string>(type: "text", nullable: false),
                    Contact_Address = table.Column<string>(type: "text", nullable: false),
                    Contact_Email = table.Column<string>(type: "text", nullable: false),
                    Contact_PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Contact_WorkEmail = table.Column<string>(type: "text", nullable: false),
                    Demographic_Country = table.Column<string>(type: "text", nullable: false),
                    Demographic_Enthic = table.Column<string>(type: "text", nullable: true),
                    Demographic_Gender = table.Column<string>(type: "text", nullable: false),
                    Demographic_MaritalStatus = table.Column<string>(type: "text", nullable: false),
                    Demographic_Religion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
