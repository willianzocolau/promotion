using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PromotionApi.Migrations
{
    public partial class PromotionDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false),
                    name = table.Column<string>(maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(maxLength: 45, nullable: true),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    token = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    nickname = table.Column<string>(maxLength: 45, nullable: true),
                    name = table.Column<string>(maxLength: 150, nullable: true),
                    email = table.Column<string>(maxLength: 255, nullable: true),
                    password = table.Column<string>(maxLength: 64, nullable: true),
                    password_salt = table.Column<string>(maxLength: 64, nullable: true),
                    type = table.Column<int>(nullable: false),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    cpf = table.Column<string>(maxLength: 11, nullable: true),
                    credit = table.Column<double>(nullable: false),
                    image_url = table.Column<string>(maxLength: 150, nullable: true),
                    telephone = table.Column<string>(maxLength: 11, nullable: true),
                    cellphone = table.Column<string>(maxLength: 11, nullable: true),
                    token = table.Column<string>(maxLength: 64, nullable: true),
                    state_fk = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_states_state_fk",
                        column: x => x.state_fk,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "forgot_password_requests",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ip = table.Column<string>(maxLength: 45, nullable: true),
                    code = table.Column<string>(maxLength: 6, nullable: true),
                    request_date = table.Column<DateTimeOffset>(nullable: false),
                    user_fk = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forgot_password_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_forgot_password_requests_users_user_fk",
                        column: x => x.user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(maxLength: 45, nullable: true),
                    price = table.Column<double>(nullable: false),
                    active = table.Column<bool>(nullable: false),
                    cashback_percentage = table.Column<double>(nullable: true),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    expire_date = table.Column<DateTimeOffset>(nullable: false),
                    image_url = table.Column<string>(maxLength: 150, nullable: true),
                    user_fk = table.Column<long>(nullable: false),
                    state_fk = table.Column<long>(nullable: false),
                    store_fk = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotions", x => x.id);
                    table.ForeignKey(
                        name: "fk_promotions_states_state_fk",
                        column: x => x.state_fk,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_promotions_stores_store_fk",
                        column: x => x.store_fk,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_promotions_users_user_fk",
                        column: x => x.user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    register_date = table.Column<DateTimeOffset>(nullable: false),
                    approved_by_user_fk = table.Column<long>(nullable: true),
                    user_fk = table.Column<long>(nullable: false),
                    promotion_fk = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_users_approved_by_user_fk",
                        column: x => x.approved_by_user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_promotions_promotion_fk",
                        column: x => x.promotion_fk,
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_users_user_fk",
                        column: x => x.user_fk,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "states",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 0L, "Acre" },
                    { 24L, "Sergipe" },
                    { 23L, "Santa Catarina" },
                    { 22L, "Roraima" },
                    { 21L, "Rondônia" },
                    { 20L, "Rio de Janeiro" },
                    { 19L, "Rio Grande do Sul" },
                    { 18L, "Rio Grande do Norte" },
                    { 17L, "Piauí" },
                    { 16L, "Pernambuco" },
                    { 15L, "Pará" },
                    { 14L, "Paraíba" },
                    { 25L, "São Paulo" },
                    { 13L, "Paraná" },
                    { 11L, "Mato Grosso do Sul" },
                    { 10L, "Mato Grosso" },
                    { 9L, "Maranhão" },
                    { 8L, "Goiás" },
                    { 7L, "Espírito Santo" },
                    { 6L, "Distrito Federal" },
                    { 5L, "Ceará" },
                    { 4L, "Bahia" },
                    { 3L, "Amazonas" },
                    { 2L, "Amapá" },
                    { 1L, "Alagoas" },
                    { 12L, "Minas Gerais" },
                    { 26L, "Tocantins" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_forgot_password_requests_user_fk",
                table: "forgot_password_requests",
                column: "user_fk");

            migrationBuilder.CreateIndex(
                name: "ix_orders_approved_by_user_fk",
                table: "orders",
                column: "approved_by_user_fk");

            migrationBuilder.CreateIndex(
                name: "ix_orders_promotion_fk",
                table: "orders",
                column: "promotion_fk");

            migrationBuilder.CreateIndex(
                name: "ix_orders_user_fk",
                table: "orders",
                column: "user_fk");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_state_fk",
                table: "promotions",
                column: "state_fk");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_store_fk",
                table: "promotions",
                column: "store_fk");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_user_fk",
                table: "promotions",
                column: "user_fk");

            migrationBuilder.CreateIndex(
                name: "ix_users_state_fk",
                table: "users",
                column: "state_fk");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forgot_password_requests");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "states");
        }
    }
}
