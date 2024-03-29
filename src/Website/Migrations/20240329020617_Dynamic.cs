using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Website.Migrations
{
    /// <inheritdoc />
    public partial class Dynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTaken = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortLinkHits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShortLinkId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortLinkHits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortLinkHits_ShortLinks_ShortLinkId",
                        column: x => x.ShortLinkId,
                        principalTable: "ShortLinks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Posted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageStreamPost",
                columns: table => new
                {
                    AttachedImagesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StreamsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageStreamPost", x => new { x.AttachedImagesId, x.StreamsId });
                    table.ForeignKey(
                        name: "FK_ImageStreamPost_Images_AttachedImagesId",
                        column: x => x.AttachedImagesId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageStreamPost_Streams_StreamsId",
                        column: x => x.StreamsId,
                        principalTable: "Streams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageStreamPost_StreamsId",
                table: "ImageStreamPost",
                column: "StreamsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortLinkHits_ShortLinkId",
                table: "ShortLinkHits",
                column: "ShortLinkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageStreamPost");

            migrationBuilder.DropTable(
                name: "ShortLinkHits");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Streams");
        }
    }
}
