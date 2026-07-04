using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falck.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Employees",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Employees");
        }
    }
}
