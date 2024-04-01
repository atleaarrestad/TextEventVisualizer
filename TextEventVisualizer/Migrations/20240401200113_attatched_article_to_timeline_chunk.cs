using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextEventVisualizer.Migrations
{
    /// <inheritdoc />
    public partial class attatched_article_to_timeline_chunk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TimelineChunks_ArticleId",
                table: "TimelineChunks",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineChunks_Articles_ArticleId",
                table: "TimelineChunks",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimelineChunks_Articles_ArticleId",
                table: "TimelineChunks");

            migrationBuilder.DropIndex(
                name: "IX_TimelineChunks_ArticleId",
                table: "TimelineChunks");
        }
    }
}
