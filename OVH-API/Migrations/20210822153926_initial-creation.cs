using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OVHAPI.Migrations
{
    public partial class initialcreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attacks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Server = table.Column<string>(type: "TEXT", nullable: true),
                    IPAttacked = table.Column<string>(type: "TEXT", nullable: true),
                    Duration = table.Column<string>(type: "TEXT", nullable: true),
                    DetectionTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndingTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attacks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Application = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IPs",
                columns: table => new
                {
                    IP = table.Column<string>(type: "TEXT", nullable: false),
                    Geolocation = table.Column<string>(type: "TEXT", nullable: true),
                    Server = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    EdgeRules = table.Column<string>(type: "TEXT", nullable: true),
                    FlagLink = table.Column<string>(type: "TEXT", nullable: true),
                    UnderAttack = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPs", x => x.IP);
                });

            migrationBuilder.CreateTable(
                name: "Setttings",
                columns: table => new
                {
                    ProfileName = table.Column<string>(type: "TEXT", nullable: false),
                    ApplicationKey = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationSecret = table.Column<string>(type: "TEXT", nullable: true),
                    ConsumerKey = table.Column<string>(type: "TEXT", nullable: true),
                    EndPoint = table.Column<string>(type: "TEXT", nullable: true),
                    DiscordWebHookUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setttings", x => x.ProfileName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attacks");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "IPs");

            migrationBuilder.DropTable(
                name: "Setttings");
        }
    }
}
