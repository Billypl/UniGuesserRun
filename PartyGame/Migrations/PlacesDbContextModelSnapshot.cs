﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PartyGame.Entities;

#nullable disable

namespace PartyGame.Migrations
{
    [DbContext(typeof(PlacesDbContext))]
    partial class PlacesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PartyGame.Entities.GameSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("PlaceId")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("GameSessions");
                });

            modelBuilder.Entity("PartyGame.Entities.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("PartyGame.Entities.GameSession", b =>
                {
                    b.HasOne("PartyGame.Entities.Place", "Place")
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Place");
                });

            modelBuilder.Entity("PartyGame.Entities.Place", b =>
                {
                    b.OwnsOne("PartyGame.Coordinates", "Coordinates", b1 =>
                        {
                            b1.Property<int>("PlaceId")
                                .HasColumnType("integer");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.HasKey("PlaceId");

                            b1.ToTable("Places");

                            b1.WithOwner()
                                .HasForeignKey("PlaceId");
                        });

                    b.Navigation("Coordinates")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
