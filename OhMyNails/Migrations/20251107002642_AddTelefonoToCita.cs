using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OhMyNails.Migrations
{
    /// <inheritdoc />
    public partial class AddTelefonoToCita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Citas");
        }
    }
}
