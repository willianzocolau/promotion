﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PromotionApi.Data;

namespace PromotionApi.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("PromotionApi.Models.ForgotPasswordRequest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .HasColumnName("code")
                        .HasMaxLength(6);

                    b.Property<string>("Ip")
                        .HasColumnName("ip")
                        .HasMaxLength(45);

                    b.Property<DateTimeOffset>("RequestDate")
                        .HasColumnName("request_date");

                    b.Property<long>("UserFK")
                        .HasColumnName("user_fk");

                    b.HasKey("Id")
                        .HasName("pk_forgot_password_requests");

                    b.HasIndex("UserFK")
                        .HasName("ix_forgot_password_requests_user_fk");

                    b.ToTable("forgot_password_requests");
                });

            modelBuilder.Entity("PromotionApi.Models.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long?>("ApprovedByUserFK")
                        .HasColumnName("approved_by_user_fk");

                    b.Property<long>("PromotionFK")
                        .HasColumnName("promotion_fk");

                    b.Property<DateTimeOffset>("RegisterDate")
                        .HasColumnName("register_date");

                    b.Property<long>("UserFK")
                        .HasColumnName("user_fk");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("ApprovedByUserFK")
                        .HasName("ix_orders_approved_by_user_fk");

                    b.HasIndex("PromotionFK")
                        .HasName("ix_orders_promotion_fk");

                    b.HasIndex("UserFK")
                        .HasName("ix_orders_user_fk");

                    b.ToTable("orders");
                });

            modelBuilder.Entity("PromotionApi.Models.Promotion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<bool>("Active")
                        .HasColumnName("active");

                    b.Property<double?>("CashbackPercentage")
                        .HasColumnName("cashback_percentage");

                    b.Property<DateTimeOffset>("ExpireDate")
                        .HasColumnName("expire_date");

                    b.Property<string>("ImageUrl")
                        .HasColumnName("image_url")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(45);

                    b.Property<double>("Price")
                        .HasColumnName("price");

                    b.Property<DateTimeOffset>("RegisterDate")
                        .HasColumnName("register_date");

                    b.Property<long>("StateFK")
                        .HasColumnName("state_fk");

                    b.Property<long>("StoreFK")
                        .HasColumnName("store_fk");

                    b.Property<long>("UserFK")
                        .HasColumnName("user_fk");

                    b.HasKey("Id")
                        .HasName("pk_promotions");

                    b.HasIndex("StateFK")
                        .HasName("ix_promotions_state_fk");

                    b.HasIndex("StoreFK")
                        .HasName("ix_promotions_store_fk");

                    b.HasIndex("UserFK")
                        .HasName("ix_promotions_user_fk");

                    b.ToTable("promotions");
                });

            modelBuilder.Entity("PromotionApi.Models.State", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(45);

                    b.HasKey("Id")
                        .HasName("pk_states");

                    b.ToTable("states");

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
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(45);

                    b.Property<DateTimeOffset>("RegisterDate")
                        .HasColumnName("register_date");

                    b.Property<string>("Token")
                        .HasColumnName("token")
                        .HasMaxLength(64);

                    b.HasKey("Id")
                        .HasName("pk_stores");

                    b.ToTable("stores");
                });

            modelBuilder.Entity("PromotionApi.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Cellphone")
                        .HasColumnName("cellphone")
                        .HasMaxLength(11);

                    b.Property<string>("Cpf")
                        .HasColumnName("cpf")
                        .HasMaxLength(11);

                    b.Property<double>("Credit")
                        .HasColumnName("credit");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasMaxLength(255);

                    b.Property<string>("ImageUrl")
                        .HasColumnName("image_url")
                        .HasMaxLength(150);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(150);

                    b.Property<string>("Nickname")
                        .HasColumnName("nickname")
                        .HasMaxLength(45);

                    b.Property<string>("Password")
                        .HasColumnName("password")
                        .HasMaxLength(64);

                    b.Property<string>("PasswordSalt")
                        .HasColumnName("password_salt")
                        .HasMaxLength(64);

                    b.Property<DateTimeOffset>("RegisterDate")
                        .HasColumnName("register_date");

                    b.Property<long?>("StateFK")
                        .HasColumnName("state_fk");

                    b.Property<string>("Telephone")
                        .HasColumnName("telephone")
                        .HasMaxLength(11);

                    b.Property<string>("Token")
                        .HasColumnName("token")
                        .HasMaxLength(64);

                    b.Property<int>("Type")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("StateFK")
                        .HasName("ix_users_state_fk");

                    b.ToTable("users");
                });

            modelBuilder.Entity("PromotionApi.Models.ForgotPasswordRequest", b =>
                {
                    b.HasOne("PromotionApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserFK")
                        .HasConstraintName("fk_forgot_password_requests_users_user_fk")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PromotionApi.Models.Order", b =>
                {
                    b.HasOne("PromotionApi.Models.User", "ApprovedByUser")
                        .WithMany()
                        .HasForeignKey("ApprovedByUserFK")
                        .HasConstraintName("fk_orders_users_approved_by_user_fk");

                    b.HasOne("PromotionApi.Models.Promotion", "Promotion")
                        .WithMany()
                        .HasForeignKey("PromotionFK")
                        .HasConstraintName("fk_orders_promotions_promotion_fk")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PromotionApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserFK")
                        .HasConstraintName("fk_orders_users_user_fk")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PromotionApi.Models.Promotion", b =>
                {
                    b.HasOne("PromotionApi.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateFK")
                        .HasConstraintName("fk_promotions_states_state_fk")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PromotionApi.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreFK")
                        .HasConstraintName("fk_promotions_stores_store_fk")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PromotionApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserFK")
                        .HasConstraintName("fk_promotions_users_user_fk")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PromotionApi.Models.User", b =>
                {
                    b.HasOne("PromotionApi.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateFK")
                        .HasConstraintName("fk_users_states_state_fk");
                });
#pragma warning restore 612, 618
        }
    }
}
