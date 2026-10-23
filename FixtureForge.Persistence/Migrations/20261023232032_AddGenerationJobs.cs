using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixtureForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenerationJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeneratedRowCount = table.Column<int>(type: "int", nullable: false),
                    ErrorSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RandomSeed = table.Column<int>(type: "int", nullable: false),
                    SchemaVersionTag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RuleVersionTag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GeneratorVersionTag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationJobs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenerationJobTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenerationJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetRowCount = table.Column<int>(type: "int", nullable: false),
                    GeneratedRowCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationJobTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationJobTables_GenerationJobs_GenerationJobId",
                        column: x => x.GenerationJobId,
                        principalTable: "GenerationJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenerationJobTables_SchemaTables_SchemaTableId",
                        column: x => x.SchemaTableId,
                        principalTable: "SchemaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenerationJobs_ProjectId_CreatedAt",
                table: "GenerationJobs",
                columns: new[] { "ProjectId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GenerationJobTables_GenerationJobId_SchemaTableId",
                table: "GenerationJobTables",
                columns: new[] { "GenerationJobId", "SchemaTableId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenerationJobTables_SchemaTableId",
                table: "GenerationJobTables",
                column: "SchemaTableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenerationJobTables");

            migrationBuilder.DropTable(
                name: "GenerationJobs");
        }
    }
}
