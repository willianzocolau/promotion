﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PromotionApi.Data;

namespace PromotionApi.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20180911194013_PromotionDb")]
    partial class PromotionDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("PromotionApi.Models.Promotion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("ExpireDate");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .HasMaxLength(45);

                    b.Property<double>("Price");

                    b.Property<DateTimeOffset>("RegisterDate");

                    b.Property<long>("StateFK");

                    b.Property<long>("StoreFK");

                    b.Property<long>("UserFK");

                    b.HasKey("Id");

                    b.HasIndex("StateFK");

                    b.HasIndex("StoreFK");

                    b.HasIndex("UserFK");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("PromotionApi.Models.State", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("Name")
                        .HasMaxLength(45);

                    b.HasKey("Id");

                    b.ToTable("States");

                    b.HasData(
                        new { Id = 0L, Name = "Acre" },
                        new { Id = 1L, Name = "Alagoas" },
                        new { Id = 2L, Name = "Amapá" },
                        new { Id = 3L, Name = "Amazonas" },
                        new { Id = 4L, Name = "Bahia" },
                        new { Id = 5L, Name = "Ceará" },
                        new { Id = 6L, Name = "Distrito Federal" },
                        new { Id = 7L, Name = "Espírito Santo" },
                        new { Id = 8L, Name = "Goiás" },
                        new { Id = 9L, Name = "Maranhão" },
                        new { Id = 10L, Name = "Mato Grosso" },
                        new { Id = 11L, Name = "Mato Grosso do Sul" },
                        new { Id = 12L, Name = "Minas Gerais" },
                        new { Id = 13L, Name = "Paraná" },
                        new { Id = 14L, Name = "Paraíba" },
                        new { Id = 15L, Name = "Pará" },
                        new { Id = 16L, Name = "Pernambuco" },
                        new { Id = 17L, Name = "Piauí" },
                        new { Id = 18L, Name = "Rio Grande do Norte" },
                        new { Id = 19L, Name = "Rio Grande do Sul" },
                        new { Id = 20L, Name = "Rio de Janeiro" },
                        new { Id = 21L, Name = "Rondônia" },
                        new { Id = 22L, Name = "Roraima" },
                        new { Id = 23L, Name = "Santa Catarina" },
                        new { Id = 24L, Name = "Sergipe" },
                        new { Id = 25L, Name = "São Paulo" },
                        new { Id = 26L, Name = "Tocantins" }
                    );
                });

            modelBuilder.Entity("PromotionApi.Models.Store", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(45);

                    b.Property<DateTimeOffset>("RegisterDate");

                    b.HasKey("Id");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("PromotionApi.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Cellphone")
                        .HasMaxLength(11);

                    b.Property<string>("Cpf")
                        .HasMaxLength(11);

                    b.Property<double>("Credit");

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .HasMaxLength(150);

                    b.Property<string>("Nickname")
                        .HasMaxLength(45);

                    b.Property<string>("Password")
                        .HasMaxLength(64);

                    b.Property<DateTimeOffset>("RegisterDate");

                    b.Property<long?>("StateFK");

                    b.Property<string>("Telephone")
                        .HasMaxLength(11);

                    b.Property<string>("Token")
                        .HasMaxLength(64);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("StateFK");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PromotionApi.Models.Promotion", b =>
                {
                    b.HasOne("PromotionApi.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PromotionApi.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PromotionApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PromotionApi.Models.User", b =>
                {
                    b.HasOne("PromotionApi.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateFK");
                });
#pragma warning restore 612, 618
        }
    }
}
