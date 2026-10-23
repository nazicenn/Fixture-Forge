using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixtureForge.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaTablesAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchemaVersion",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SchemaTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaTables_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsNullable = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    IsUnique = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaxLength = table.Column<int>(type: "int", nullable: true),
                    Precision = table.Column<int>(type: "int", nullable: true),
                    Scale = table.Column<int>(type: "int", nullable: true),
                    OrdinalPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaColumns_SchemaTables_SchemaTableId",
                        column: x => x.SchemaTableId,
                        principalTable: "SchemaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemaColumns_SchemaTableId_Name",
                table: "SchemaColumns",
                columns: new[] { "SchemaTableId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaColumns_SchemaTableId_OrdinalPosition",
                table: "SchemaColumns",
                columns: new[] { "SchemaTableId", "OrdinalPosition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaTables_ProjectId_Name",
                table: "SchemaTables",
                columns: new[] { "ProjectId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemaColumns");

            migrationBuilder.DropTable(
                name: "SchemaTables");

            migrationBuilder.DropColumn(
                name: "SchemaVersion",
                table: "Projects");
        }
    }
}
