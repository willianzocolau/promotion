using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PromotionApi.Migrations
{
    public partial class PromotionDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matchs",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    user_fk = table.Column<long>(nullable: false),
                    promotion_fk = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matchs", x => x.id);
                    table.ForeignKey(
                        name: "fk_matchs_promotions_promotion_fk",
                        column: x => x.promotion_fk,
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_matchs_users_user_fk",
                        column: x => x.user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wishlist",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(maxLength: 45, nullable: true),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    user_fk = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishlist", x => x.id);
                    table.ForeignKey(
                        name: "fk_wishlist_users_user_fk",
                        column: x => x.user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_matchs_promotion_fk",
                table: "matchs",
                column: "promotion_fk");

            migrationBuilder.CreateIndex(
                name: "ix_matchs_user_fk",
                table: "matchs",
                column: "user_fk");

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_user_fk",
                table: "wishlist",
                column: "user_fk");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matchs");

            migrationBuilder.DropTable(
                name: "wishlist");
        }
    }
}
