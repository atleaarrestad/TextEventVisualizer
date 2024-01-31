using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextEventVisualizer.Migrations
{
    /// <inheritdoc />
    public partial class adjuster_article_model_to_fit_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Articles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
