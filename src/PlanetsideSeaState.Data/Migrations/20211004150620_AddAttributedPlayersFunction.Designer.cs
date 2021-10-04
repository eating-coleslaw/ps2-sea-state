﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PlanetsideSeaState.Data;

namespace PlanetsideSeaState.Data.Migrations
{
    [DbContext(typeof(PlanetmansDbContext))]
    [Migration("20211004150620_AddAttributedPlayersFunction")]
    partial class AddAttributedPlayersFunction
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Census.Character", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<short>("BattleRank")
                        .HasColumnType("smallint");

                    b.Property<short>("FactionId")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OutfitAlias")
                        .HasColumnType("text");

                    b.Property<string>("OutfitAliasLower")
                        .HasColumnType("text");

                    b.Property<string>("OutfitId")
                        .HasColumnType("text");

                    b.Property<short>("PrestigeLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)0);

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Census.Experience", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<float>("Xp")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Experience");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Census.FacilityLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Desription")
                        .HasColumnType("text");

                    b.Property<int>("FacilityIdA")
                        .HasColumnType("integer");

                    b.Property<int>("FacilityIdB")
                        .HasColumnType("integer");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("FacilityLink");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Census.FacilityType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FacilityType");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Census.MapRegion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<int>("FacilityId")
                        .HasColumnType("integer");

                    b.Property<string>("FacilityName")
                        .HasColumnType("text");

                    b.Property<string>("FacilityType")
                        .HasColumnType("text");

                    b.Property<int>("FacilityTypeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeprecated")
                        .HasColumnType("boolean");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id", "FacilityId");

                    b.ToTable("MapRegion");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.Death", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("AttackerCharacterId")
                        .HasColumnType("text");

                    b.Property<string>("VictimCharacterId")
                        .HasColumnType("text");

                    b.Property<short?>("AttackerFactionId")
                        .HasColumnType("smallint");

                    b.Property<short?>("AttackerLoadoutId")
                        .HasColumnType("smallint");

                    b.Property<int?>("AttackerVehicleId")
                        .HasColumnType("integer");

                    b.Property<int>("DeathType")
                        .HasColumnType("integer");

                    b.Property<bool>("IsHeadshot")
                        .HasColumnType("boolean");

                    b.Property<short?>("VictimFactionId")
                        .HasColumnType("smallint");

                    b.Property<short?>("VictimLoadoutId")
                        .HasColumnType("smallint");

                    b.Property<int?>("WeaponId")
                        .HasColumnType("integer");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.Property<long?>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Timestamp", "AttackerCharacterId", "VictimCharacterId");

                    b.ToTable("Death");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.ExperienceGain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<string>("CharacterId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ExperienceId")
                        .HasColumnType("integer");

                    b.Property<short?>("LoadoutId")
                        .HasColumnType("smallint");

                    b.Property<string>("OtherId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp", "WorldId", "ZoneId");

                    b.ToTable("ExperienceGain");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.FacilityControl", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("FacilityId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsCapture")
                        .HasColumnType("boolean");

                    b.Property<short>("NewFactionId")
                        .HasColumnType("smallint");

                    b.Property<short>("OldFactionId")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp", "WorldId", "FacilityId");

                    b.ToTable("FacilityControl");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.PlayerFacilityControl", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("FacilityId")
                        .HasColumnType("integer");

                    b.Property<string>("CharacterId")
                        .HasColumnType("text");

                    b.Property<Guid>("FacilityControlId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCapture")
                        .HasColumnType("boolean");

                    b.Property<string>("OutfitId")
                        .HasColumnType("text");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Timestamp", "FacilityId", "CharacterId");

                    b.HasIndex("FacilityControlId");

                    b.ToTable("PlayerFacilityControl");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.PlayerLogin", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CharacterId")
                        .HasColumnType("text");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.HasKey("Timestamp", "CharacterId");

                    b.ToTable("PlayerLogin");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.PlayerLogout", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CharacterId")
                        .HasColumnType("text");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.HasKey("Timestamp", "CharacterId");

                    b.ToTable("PlayerLogout");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.VehicleDestruction", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("AttackerCharacterId")
                        .HasColumnType("text");

                    b.Property<string>("VictimCharacterId")
                        .HasColumnType("text");

                    b.Property<int>("VictimVehicleId")
                        .HasColumnType("integer");

                    b.Property<int?>("AttackerVehicleId")
                        .HasColumnType("integer");

                    b.Property<int>("DeathType")
                        .HasColumnType("integer");

                    b.Property<int?>("FacilityId")
                        .HasColumnType("integer");

                    b.Property<bool?>("IsVehicleWeapon")
                        .HasColumnType("boolean");

                    b.Property<int?>("WeaponId")
                        .HasColumnType("integer");

                    b.Property<short>("WorldId")
                        .HasColumnType("smallint");

                    b.Property<long?>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Timestamp", "AttackerCharacterId", "VictimCharacterId", "VictimVehicleId");

                    b.ToTable("VehicleDestruction");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.UpdaterScheduler", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("UpdaterScheduler");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.PlayerFacilityControl", b =>
                {
                    b.HasOne("PlanetsideSeaState.Data.Models.Events.FacilityControl", "FacilityControl")
                        .WithMany("PlayerControls")
                        .HasForeignKey("FacilityControlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FacilityControl");
                });

            modelBuilder.Entity("PlanetsideSeaState.Data.Models.Events.FacilityControl", b =>
                {
                    b.Navigation("PlayerControls");
                });
#pragma warning restore 612, 618
        }
    }
}
