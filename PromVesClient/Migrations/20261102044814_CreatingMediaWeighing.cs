using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromVesClient.Migrations
{
    /// <inheritdoc />
    public partial class CreatingMediaWeighing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaWeighings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeighingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileNamePhoto = table.Column<string>(type: "text", nullable: false),
                    FilePathPhoto = table.Column<string>(type: "text", nullable: false),
                    FileNameVideo = table.Column<string>(type: "text", nullable: false),
                    FilePathVideo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaWeighings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaWeighings_Weighings_WeighingId",
                        column: x => x.WeighingId,
                        principalTable: "Weighings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaWeighings_WeighingId",
                table: "MediaWeighings",
                column: "WeighingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaWeighings");
        }
    }
}
