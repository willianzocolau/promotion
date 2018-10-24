using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PromotionApi.Migrations
{
    public partial class PromotionDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "answer",
                table: "orders",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "answer_register_date",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "orders",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "comment_register_date",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_vote_positive",
                table: "orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "answer",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "answer_register_date",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "comment_register_date",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "is_vote_positive",
                table: "orders");
        }
    }
}
