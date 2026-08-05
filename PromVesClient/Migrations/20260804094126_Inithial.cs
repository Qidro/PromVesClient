using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromVesClient.Migrations
{
    /// <inheritdoc />
    public partial class Inithial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeWeighng = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weighings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VagonNumber = table.Column<string>(type: "text", nullable: false),
                    TareWeight = table.Column<double>(type: "double precision", nullable: false),
                    GrossWeight = table.Column<double>(type: "double precision", nullable: false),
                    NetWeight = table.Column<double>(type: "double precision", nullable: false),
                    LoadCapacity = table.Column<double>(type: "double precision", nullable: false),
                    LoadDeviation = table.Column<double>(type: "double precision", nullable: false),
                    FirstCart = table.Column<double>(type: "double precision", nullable: false),
                    SecondCart = table.Column<double>(type: "double precision", nullable: false),
                    DifferenceCarts = table.Column<double>(type: "double precision", nullable: false),
                    LeftSide = table.Column<double>(type: "double precision", nullable: false),
                    RightSide = table.Column<double>(type: "double precision", nullable: false),
                    DifferenceSides = table.Column<double>(type: "double precision", nullable: false),
                    TypeWeighing = table.Column<string>(type: "text", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weighings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weighings_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Weighings_ReceiptId",
                table: "Weighings",
                column: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Weighings");

            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
