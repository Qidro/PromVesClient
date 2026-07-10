using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PromVesClient.Migrations
{
    /// <inheritdoc />
    public partial class CreateReceiptAndWeighing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeWeighng = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
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
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weighings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weighings_Receipts_ReceiptId1",
                        column: x => x.ReceiptId1,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Weighings_ReceiptId1",
                table: "Weighings",
                column: "ReceiptId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weighings");

            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
