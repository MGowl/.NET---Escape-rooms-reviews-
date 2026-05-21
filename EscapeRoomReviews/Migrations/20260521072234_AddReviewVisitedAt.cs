using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscapeRoomReviews.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewVisitedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VisitedAt",
                table: "Reviews",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitedAt",
                table: "Reviews");
        }
    }
}
