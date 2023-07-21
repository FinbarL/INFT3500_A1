﻿// <auto-generated />
using System;
using INFT3500.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace INFT3500.Migrations
{
    [DbContext(typeof(StoreDbContext))]
    [Migration("20230720071601_AddTempGenreSubGenre_Product")]
    partial class AddTempGenreSubGenre_Product
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.6.23329.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("INFT3500.Models.BookGenre", b =>
                {
                    b.Property<int>("SubGenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("subGenreID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubGenreId"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SubGenreId");

                    b.ToTable("Book_genre", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.BookGenreNew", b =>
                {
                    b.Property<int>("SubGenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("subGenreID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubGenreId"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SubGenreId");

                    b.ToTable("Book_genre NEW", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.GameGenre", b =>
                {
                    b.Property<int>("SubGenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("subGenreID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubGenreId"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SubGenreId");

                    b.ToTable("Game_genre", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .HasColumnType("int")
                        .HasColumnName("genreID");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("GenreId");

                    b.ToTable("Genre", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.MovieGenre", b =>
                {
                    b.Property<int>("SubGenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("subGenreID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubGenreId"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SubGenreId");

                    b.ToTable("Movie_genre", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OrderID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int?>("Customer")
                        .HasColumnType("int")
                        .HasColumnName("customer");

                    b.Property<int?>("PostCode")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("StreetAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Suburb")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("OrderId");

                    b.HasIndex("Customer");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("INFT3500.Models.Patron", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("HashPw")
                        .HasMaxLength(64)
                        .IsUnicode(false)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("HashPW");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Salt")
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.HasKey("UserId");

                    b.ToTable("Patrons");
                });

            modelBuilder.Entity("INFT3500.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Genre")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime");

                    b.Property<string>("LastUpdatedBy")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateOnly?>("Published")
                        .HasColumnType("date");

                    b.Property<int?>("SubGenre")
                        .HasColumnType("int")
                        .HasColumnName("subGenre");

                    b.HasKey("Id");

                    b.HasIndex("Genre");

                    b.HasIndex("LastUpdatedBy");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.ProductsInOrder", b =>
                {
                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ProduktId")
                        .HasColumnType("int")
                        .HasColumnName("produktId");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProduktId");

                    b.ToTable("ProductsInOrders");
                });

            modelBuilder.Entity("INFT3500.Models.Source", b =>
                {
                    b.Property<int>("Sourceid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("sourceid");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Sourceid"));

                    b.Property<string>("ExternalLink")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("externalLink");

                    b.Property<int?>("Genre")
                        .HasColumnType("int");

                    b.Property<string>("SourceName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Source_name");

                    b.HasKey("Sourceid");

                    b.HasIndex("Genre");

                    b.ToTable("Source", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.Stocktake", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemId"));

                    b.Property<double?>("Price")
                        .HasColumnType("float");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("SourceId")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SourceId");

                    b.ToTable("Stocktake", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.To", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("customerID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("CardNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CardOwner")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("Cvv")
                        .HasColumnType("int")
                        .HasColumnName("CVV");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Expiry")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)");

                    b.Property<int?>("PatronId")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("PostCode")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("StreetAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Suburb")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CustomerId");

                    b.HasIndex("PatronId");

                    b.ToTable("TO", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("HashPw")
                        .HasMaxLength(64)
                        .IsUnicode(false)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("HashPW");

                    b.Property<bool?>("IsAdmin")
                        .HasColumnType("bit")
                        .HasColumnName("isAdmin");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Salt")
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.HasKey("UserName")
                        .HasName("PK_Users");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("INFT3500.Models.Order", b =>
                {
                    b.HasOne("INFT3500.Models.To", "CustomerNavigation")
                        .WithMany("Orders")
                        .HasForeignKey("Customer")
                        .HasConstraintName("FK_Orders_TO");

                    b.Navigation("CustomerNavigation");
                });

            modelBuilder.Entity("INFT3500.Models.Product", b =>
                {
                    b.HasOne("INFT3500.Models.Genre", "GenreNavigation")
                        .WithMany("Products")
                        .HasForeignKey("Genre")
                        .HasConstraintName("FK_Product_Genre");

                    b.HasOne("INFT3500.Models.User", "LastUpdatedByNavigation")
                        .WithMany("Products")
                        .HasForeignKey("LastUpdatedBy")
                        .HasConstraintName("FK_Product_Users");

                    b.Navigation("GenreNavigation");

                    b.Navigation("LastUpdatedByNavigation");
                });

            modelBuilder.Entity("INFT3500.Models.ProductsInOrder", b =>
                {
                    b.HasOne("INFT3500.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_ProductsInOrders_Orders");

                    b.HasOne("INFT3500.Models.Stocktake", "Produkt")
                        .WithMany()
                        .HasForeignKey("ProduktId")
                        .HasConstraintName("FK_ProductsInOrders_Stocktake");

                    b.Navigation("Order");

                    b.Navigation("Produkt");
                });

            modelBuilder.Entity("INFT3500.Models.Source", b =>
                {
                    b.HasOne("INFT3500.Models.Genre", "GenreNavigation")
                        .WithMany("Sources")
                        .HasForeignKey("Genre")
                        .HasConstraintName("FK_Source_Genre");

                    b.Navigation("GenreNavigation");
                });

            modelBuilder.Entity("INFT3500.Models.Stocktake", b =>
                {
                    b.HasOne("INFT3500.Models.Product", "Product")
                        .WithMany("Stocktakes")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_Stocktake_Product");

                    b.HasOne("INFT3500.Models.Source", "Source")
                        .WithMany("Stocktakes")
                        .HasForeignKey("SourceId")
                        .HasConstraintName("FK_Stocktake_Source");

                    b.Navigation("Product");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("INFT3500.Models.To", b =>
                {
                    b.HasOne("INFT3500.Models.Patron", "Patron")
                        .WithMany("Tos")
                        .HasForeignKey("PatronId")
                        .HasConstraintName("FK_TO_Patrons");

                    b.Navigation("Patron");
                });

            modelBuilder.Entity("INFT3500.Models.Genre", b =>
                {
                    b.Navigation("Products");

                    b.Navigation("Sources");
                });

            modelBuilder.Entity("INFT3500.Models.Patron", b =>
                {
                    b.Navigation("Tos");
                });

            modelBuilder.Entity("INFT3500.Models.Product", b =>
                {
                    b.Navigation("Stocktakes");
                });

            modelBuilder.Entity("INFT3500.Models.Source", b =>
                {
                    b.Navigation("Stocktakes");
                });

            modelBuilder.Entity("INFT3500.Models.To", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("INFT3500.Models.User", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
