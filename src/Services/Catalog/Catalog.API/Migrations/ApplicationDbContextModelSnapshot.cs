﻿// <auto-generated />
using System;
using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Catalog.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Catalog.Domain.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("PlateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Registration")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AuditLogs", (string)null);
                });

            modelBuilder.Entity("Catalog.Domain.Plate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Letters")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Numbers")
                        .HasColumnType("int");

                    b.Property<int>("PurchasePrice")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Registration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SalePrice")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Plates", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}