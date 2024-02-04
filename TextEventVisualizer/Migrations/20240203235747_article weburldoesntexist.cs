using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextEventVisualizer.Migrations
{
    /// <inheritdoc />
    public partial class articleweburldoesntexist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UrlDoesntExistAnymore",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlDoesntExistAnymore",
                table: "Articles");
        }
    }
}
