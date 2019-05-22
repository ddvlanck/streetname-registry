﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreetNameRegistry.Projections.Legacy;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    [DbContext(typeof(LegacyContext))]
    [Migration("20190522075530_AddEventXmlToSyndication")]
    partial class AddEventXmlToSyndication
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.ProjectionStates.ProjectionStateItem", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Position");

                    b.HasKey("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("ProjectionStates","StreetNameRegistryLegacy");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Legacy.StreetNameDetail.StreetNameDetail", b =>
                {
                    b.Property<Guid>("StreetNameId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Complete");

                    b.Property<string>("HomonymAdditionDutch");

                    b.Property<string>("HomonymAdditionEnglish");

                    b.Property<string>("HomonymAdditionFrench");

                    b.Property<string>("HomonymAdditionGerman");

                    b.Property<string>("NameDutch");

                    b.Property<string>("NameEnglish");

                    b.Property<string>("NameFrench");

                    b.Property<string>("NameGerman");

                    b.Property<string>("NisCode");

                    b.Property<int?>("OsloId");

                    b.Property<bool>("Removed");

                    b.Property<int?>("Status");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnName("VersionTimestamp");

                    b.HasKey("StreetNameId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("OsloId")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Removed");

                    b.ToTable("StreetNameDetails","StreetNameRegistryLegacy");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Legacy.StreetNameList.StreetNameListItem", b =>
                {
                    b.Property<Guid>("StreetNameId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Complete");

                    b.Property<string>("HomonymAdditionDutch");

                    b.Property<string>("HomonymAdditionEnglish");

                    b.Property<string>("HomonymAdditionFrench");

                    b.Property<string>("HomonymAdditionGerman");

                    b.Property<string>("NameDutch");

                    b.Property<string>("NameEnglish");

                    b.Property<string>("NameFrench");

                    b.Property<string>("NameGerman");

                    b.Property<string>("NisCode");

                    b.Property<int?>("OsloId");

                    b.Property<int?>("PrimaryLanguage");

                    b.Property<bool>("Removed");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnName("VersionTimestamp");

                    b.HasKey("StreetNameId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("NisCode");

                    b.HasIndex("OsloId")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Removed");

                    b.ToTable("StreetNameList","StreetNameRegistryLegacy");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Legacy.StreetNameName.StreetNameName", b =>
                {
                    b.Property<Guid>("StreetNameId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Complete");

                    b.Property<bool>("IsFlemishRegion");

                    b.Property<string>("NameDutch");

                    b.Property<string>("NameDutchSearch");

                    b.Property<string>("NameEnglish");

                    b.Property<string>("NameEnglishSearch");

                    b.Property<string>("NameFrench");

                    b.Property<string>("NameFrenchSearch");

                    b.Property<string>("NameGerman");

                    b.Property<string>("NameGermanSearch");

                    b.Property<string>("NisCode");

                    b.Property<int>("OsloId");

                    b.Property<bool>("Removed");

                    b.Property<int?>("Status");

                    b.Property<DateTimeOffset>("VersionTimestampAsDateTimeOffset")
                        .HasColumnName("VersionTimestamp");

                    b.HasKey("StreetNameId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Complete");

                    b.HasIndex("IsFlemishRegion");

                    b.HasIndex("NameDutchSearch");

                    b.HasIndex("NameEnglishSearch");

                    b.HasIndex("NameFrenchSearch");

                    b.HasIndex("NameGermanSearch");

                    b.HasIndex("NisCode");

                    b.HasIndex("OsloId");

                    b.HasIndex("Removed");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestampAsDateTimeOffset");

                    b.ToTable("StreetNameName","StreetNameRegistryLegacy");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Legacy.StreetNameSyndication.StreetNameSyndicationItem", b =>
                {
                    b.Property<long>("Position");

                    b.Property<int?>("Application");

                    b.Property<string>("ChangeType");

                    b.Property<string>("EventDataAsXml");

                    b.Property<string>("HomonymAdditionDutch");

                    b.Property<string>("HomonymAdditionEnglish");

                    b.Property<string>("HomonymAdditionFrench");

                    b.Property<string>("HomonymAdditionGerman");

                    b.Property<bool>("IsComplete");

                    b.Property<DateTimeOffset>("LastChangedOnAsDateTimeOffset")
                        .HasColumnName("LastChangedOn");

                    b.Property<int?>("Modification");

                    b.Property<string>("NameDutch");

                    b.Property<string>("NameEnglish");

                    b.Property<string>("NameFrench");

                    b.Property<string>("NameGerman");

                    b.Property<string>("NisCode");

                    b.Property<string>("Operator");

                    b.Property<int?>("Organisation");

                    b.Property<int?>("OsloId");

                    b.Property<int?>("Plan");

                    b.Property<DateTimeOffset>("RecordCreatedAtAsDateTimeOffset")
                        .HasColumnName("RecordCreatedAt");

                    b.Property<int?>("Status");

                    b.Property<Guid>("StreetNameId");

                    b.HasKey("Position")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("StreetNameId");

                    b.ToTable("StreetNameSyndication","StreetNameRegistryLegacy");
                });

            modelBuilder.Entity("StreetNameRegistry.Projections.Legacy.StreetNameVersion.StreetNameVersion", b =>
                {
                    b.Property<Guid>("StreetNameId");

                    b.Property<long>("Position");

                    b.Property<int?>("Application");

                    b.Property<bool>("Complete");

                    b.Property<string>("HomonymAdditionDutch");

                    b.Property<string>("HomonymAdditionEnglish");

                    b.Property<string>("HomonymAdditionFrench");

                    b.Property<string>("HomonymAdditionGerman");

                    b.Property<int?>("Modification");

                    b.Property<string>("NameDutch");

                    b.Property<string>("NameEnglish");

                    b.Property<string>("NameFrench");

                    b.Property<string>("NameGerman");

                    b.Property<string>("NisCode");

                    b.Property<string>("Operator");

                    b.Property<int?>("Organisation");

                    b.Property<int?>("OsloId");

                    b.Property<int?>("Plan");

                    b.Property<bool>("Removed");

                    b.Property<int?>("Status");

                    b.Property<DateTimeOffset?>("VersionTimestampAsDateTimeOffset")
                        .HasColumnName("VersionTimestamp");

                    b.HasKey("StreetNameId", "Position")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("OsloId")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Removed");

                    b.ToTable("StreetNameVersions","StreetNameRegistryLegacy");
                });
#pragma warning restore 612, 618
        }
    }
}
