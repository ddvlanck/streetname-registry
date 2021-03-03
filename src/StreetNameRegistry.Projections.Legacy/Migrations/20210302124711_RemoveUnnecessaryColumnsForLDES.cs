using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class RemoveUnnecessaryColumnsForLDES : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "EventDataAsXml",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "LastChangedOn",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "Modification",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "Operator",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "Organisation",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");

            migrationBuilder.DropColumn(
                name: "SyndicationItemCreatedAt",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Application",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventDataAsXml",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastChangedOn",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Modification",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Operator",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Organisation",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SyndicationItemCreatedAt",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameSyndication",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
