using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OhMyNails.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagenReferencia",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "ImagenReferencia",
                table: "Citas");
        }
    }
}
