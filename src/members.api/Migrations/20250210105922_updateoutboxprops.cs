using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace members.api.Migrations
{
    /// <inheritdoc />
    public partial class updateoutboxprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OutboxMessages",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "OutboxMessages",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "OutboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payload",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "OutboxMessages",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "OutboxMessages",
                newName: "Content");
        }
    }
}
