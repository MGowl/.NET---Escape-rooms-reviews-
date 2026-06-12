using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscapeRoomReviews.Migrations
{
    /// <inheritdoc />
    public partial class ExtendPhotoWithMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Photos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Photos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Photos",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Photos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Photos");
        }
    }
}
