﻿// <auto-generated />
using System;
using GardenHub.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GardenHub.Server.Migrations
{
    [DbContext(typeof(GardenHubDbContext))]
    [Migration("20220704154102_Controllers")]
    partial class Controllers
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<DateTime?>("DatePlanted")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PlantId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PotName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("SensorId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PlantId");

                    b.HasIndex("SensorId");

                    b.ToTable("Pot");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Reading", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PotId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PotId");

                    b.ToTable("Reading");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Sensor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SensorControllerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SensorName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SensorControllerId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.SensorController", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ControllerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SensorController");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Pot", b =>
                {
                    b.HasOne("GardenHub.Shared.Model.Plant", "Plant")
                        .WithMany("Pots")
                        .HasForeignKey("PlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GardenHub.Shared.Model.Sensor", "Sensor")
                        .WithMany()
                        .HasForeignKey("SensorId");

                    b.Navigation("Plant");

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Reading", b =>
                {
                    b.HasOne("GardenHub.Shared.Model.Pot", "Pot")
                        .WithMany()
                        .HasForeignKey("PotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pot");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Sensor", b =>
                {
                    b.HasOne("GardenHub.Shared.Model.SensorController", "SensorController")
                        .WithMany("Sensors")
                        .HasForeignKey("SensorControllerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SensorController");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.Plant", b =>
                {
                    b.Navigation("Pots");
                });

            modelBuilder.Entity("GardenHub.Shared.Model.SensorController", b =>
                {
                    b.Navigation("Sensors");
                });
#pragma warning restore 612, 618
        }
    }
}
