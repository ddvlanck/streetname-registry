using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StreetNameRegistry.Projections.Legacy.Migrations
{
    public partial class AddTableForLinkedDataEventStream : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StreetNameLinkedDataEventStream",
                schema: "StreetNameRegistryLegacy",
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
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    RecordCreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNameLinkedDataEventStream", x => x.Position)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "CI_StreetNameLinkedDataEventStream_Position",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameLinkedDataEventStream",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNameLinkedDataEventStream_StreetNameId",
                schema: "StreetNameRegistryLegacy",
                table: "StreetNameLinkedDataEventStream",
                column: "StreetNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreetNameLinkedDataEventStream",
                schema: "StreetNameRegistryLegacy");
        }
    }
}
