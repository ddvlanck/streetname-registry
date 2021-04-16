using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class AddLinkedDataEventStream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "StreetNameRegistryLdes");

            migrationBuilder.CreateTable(
                name: "StreetName",
                schema: "StreetNameRegistryLdes",
                columns: table => new
                {
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    StreetNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersistentLocalId = table.Column<int>(type: "int", nullable: true),
                    NisCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomonymAdditionGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    EventGeneratedAtTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ObjectIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordCanBePublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetName", x => x.Position)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "CI_StreetName_Position",
                schema: "StreetNameRegistryLdes",
                table: "StreetName",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_StreetName_StreetNameId",
                schema: "StreetNameRegistryLdes",
                table: "StreetName",
                column: "StreetNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetName",
                schema: "StreetNameRegistryLdes");
        }
    }
}
