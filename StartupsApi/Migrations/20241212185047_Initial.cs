using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StartupsApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Db");

            migrationBuilder.CreateTable(
                name: "Startups",
                schema: "Db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DonationTarget = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Startups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donaters",
                schema: "Db",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Donated = table.Column<int>(type: "integer", nullable: false),
                    StartupId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donaters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donaters_Startups_StartupId",
                        column: x => x.StartupId,
                        principalSchema: "Db",
                        principalTable: "Startups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donaters_StartupId",
                schema: "Db",
                table: "Donaters",
                column: "StartupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donaters",
                schema: "Db");

            migrationBuilder.DropTable(
                name: "Startups",
                schema: "Db");
        }
    }
}
