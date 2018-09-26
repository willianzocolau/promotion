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
                name: "States",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 45, nullable: true),
                    RegisterDate = table.Column<DateTimeOffset>(nullable: false),
                    Token = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nickname = table.Column<string>(maxLength: 45, nullable: true),
                    Name = table.Column<string>(maxLength: 150, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    Password = table.Column<string>(maxLength: 64, nullable: true),
                    PasswordSalt = table.Column<string>(maxLength: 64, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    RegisterDate = table.Column<DateTimeOffset>(nullable: false),
                    Cpf = table.Column<string>(maxLength: 11, nullable: true),
                    Credit = table.Column<double>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 150, nullable: true),
                    Telephone = table.Column<string>(maxLength: 11, nullable: true),
                    Cellphone = table.Column<string>(maxLength: 11, nullable: true),
                    Token = table.Column<string>(maxLength: 64, nullable: true),
                    StateFK = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_States_StateFK",
                        column: x => x.StateFK,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForgotPasswordRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Ip = table.Column<string>(maxLength: 45, nullable: true),
                    Code = table.Column<string>(maxLength: 6, nullable: true),
                    RequestDate = table.Column<DateTimeOffset>(nullable: false),
                    UserFK = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForgotPasswordRequests_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 45, nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CashbackPercentage = table.Column<double>(nullable: true),
                    RegisterDate = table.Column<DateTimeOffset>(nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 150, nullable: true),
                    UserFK = table.Column<long>(nullable: false),
                    StateFK = table.Column<long>(nullable: false),
                    StoreFK = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_States_StateFK",
                        column: x => x.StateFK,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promotions_Stores_StoreFK",
                        column: x => x.StoreFK,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promotions_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    ApprovedByUserFK = table.Column<long>(nullable: true),
                    UserFK = table.Column<long>(nullable: false),
                    PromotionFK = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_ApprovedByUserFK",
                        column: x => x.ApprovedByUserFK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Promotions_PromotionFK",
                        column: x => x.PromotionFK,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserFK",
                        column: x => x.UserFK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "Id", "Name" },
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
                name: "IX_ForgotPasswordRequests_UserFK",
                table: "ForgotPasswordRequests",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApprovedByUserFK",
                table: "Orders",
                column: "ApprovedByUserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromotionFK",
                table: "Orders",
                column: "PromotionFK");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserFK",
                table: "Orders",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_StateFK",
                table: "Promotions",
                column: "StateFK");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_StoreFK",
                table: "Promotions",
                column: "StoreFK");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_UserFK",
                table: "Promotions",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StateFK",
                table: "Users",
                column: "StateFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForgotPasswordRequests");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
