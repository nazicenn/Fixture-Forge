using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixtureForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchemaRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaRelationships_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemaRelationships_SchemaColumns_SourceColumnId",
                        column: x => x.SourceColumnId,
                        principalTable: "SchemaColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemaRelationships_SchemaColumns_TargetColumnId",
                        column: x => x.TargetColumnId,
                        principalTable: "SchemaColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemaRelationships_SchemaTables_SourceTableId",
                        column: x => x.SourceTableId,
                        principalTable: "SchemaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemaRelationships_SchemaTables_TargetTableId",
                        column: x => x.TargetTableId,
                        principalTable: "SchemaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemaRelationships_ProjectId",
                table: "SchemaRelationships",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaRelationships_SourceColumnId_TargetColumnId",
                table: "SchemaRelationships",
                columns: new[] { "SourceColumnId", "TargetColumnId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaRelationships_SourceTableId",
                table: "SchemaRelationships",
                column: "SourceTableId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaRelationships_TargetColumnId",
                table: "SchemaRelationships",
                column: "TargetColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaRelationships_TargetTableId",
                table: "SchemaRelationships",
                column: "TargetTableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemaRelationships");
        }
    }
}
