using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextEventVisualizer.Migrations
{
    /// <inheritdoc />
    public partial class timeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimelineRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticleClusterSearch = table.Column<string>(type: "TEXT", nullable: false),
                    MaxArticleClusterSearchDistance = table.Column<float>(type: "REAL", nullable: false),
                    MaxDistanceDeltaForArticles = table.Column<float>(type: "REAL", nullable: false),
                    ArticleClusterSearchPositiveBias_Concepts = table.Column<string>(type: "TEXT", nullable: true),
                    PositiveBiasForce = table.Column<float>(type: "REAL", nullable: true),
                    ArticleClusterSearchNegativeBias_Concepts = table.Column<string>(type: "TEXT", nullable: true),
                    NegativeBiasForce = table.Column<float>(type: "REAL", nullable: true),
                    MaxEventCountForEachArticle = table.Column<int>(type: "INTEGER", nullable: false),
                    DesiredEventCountForEachArticle = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxArticleCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timelines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimelineRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timelines_TimelineRequests_TimelineRequestId",
                        column: x => x.TimelineRequestId,
                        principalTable: "TimelineRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimelineChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimelineId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimelineChunks_Timelines_TimelineId",
                        column: x => x.TimelineId,
                        principalTable: "Timelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<string>(type: "TEXT", nullable: false),
                    TimelineChunkId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_TimelineChunks_TimelineChunkId",
                        column: x => x.TimelineChunkId,
                        principalTable: "TimelineChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_TimelineChunkId",
                table: "Events",
                column: "TimelineChunkId");

            migrationBuilder.CreateIndex(
                name: "IX_TimelineChunks_TimelineId",
                table: "TimelineChunks",
                column: "TimelineId");

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_TimelineRequestId",
                table: "Timelines",
                column: "TimelineRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "TimelineChunks");

            migrationBuilder.DropTable(
                name: "Timelines");

            migrationBuilder.DropTable(
                name: "TimelineRequests");
        }
    }
}
