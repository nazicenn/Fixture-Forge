using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixtureForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQueryExecutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueryExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QueryFingerprint = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FingerprintPolicyVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QueryText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ParameterSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ServerDurationMs = table.Column<long>(type: "bigint", nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    RowsAffected = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryExecutions_DatabaseConnections_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "DatabaseConnections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueryExecutions_ConnectionId_CreatedAt",
                table: "QueryExecutions",
                columns: new[] { "ConnectionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QueryExecutions_QueryFingerprint",
                table: "QueryExecutions",
                column: "QueryFingerprint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryExecutions");
        }
    }
}
