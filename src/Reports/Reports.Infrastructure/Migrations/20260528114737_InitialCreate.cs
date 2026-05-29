using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reports.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reports");

            migrationBuilder.CreateTable(
                name: "ReportJobs",
                schema: "reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FileKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RequestedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ParametersJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportJobs_RequestedBy",
                schema: "reports",
                table: "ReportJobs",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportJobs_Status",
                schema: "reports",
                table: "ReportJobs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportJobs",
                schema: "reports");
        }
    }
}
