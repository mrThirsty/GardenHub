﻿// <auto-generated />
using System;
using GardenHub.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GardenHub.Server.Migrations
{
    [DbContext(typeof(GardenHubDbContext))]
    partial class GardenHubDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("GardenHub.Shared.Model.Plant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlantName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("RequiredSoilMoisture")
                        .HasColumnType("REAL");

                    b.Property<int>("RequiredSun")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Plant");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Pot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DatePlanted")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PlantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PotName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PlantId");

                    b.ToTable("Pot");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Sensor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("SensorName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Pot", b =>
                {
                    b.HasOne("GardenHub.Shared.Model.Plant", "Plant")
                        .WithMany("Pots")
                        .HasForeignKey("PlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plant");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Plant", b =>
                {
                    b.Navigation("Pots");
                });
#pragma warning restore 612, 618
        }
    }
}
