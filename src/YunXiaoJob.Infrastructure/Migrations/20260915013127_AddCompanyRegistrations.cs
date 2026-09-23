using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YunXiaoJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    ContactFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactPhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrations_Companies_CreatedCompanyId",
                        column: x => x.CreatedCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrations_Users_CreatedOwnerUserId",
                        column: x => x.CreatedOwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrations_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_ContactEmail",
                table: "CompanyRegistrations",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_CreatedCompanyId",
                table: "CompanyRegistrations",
                column: "CreatedCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_CreatedOwnerUserId",
                table: "CompanyRegistrations",
                column: "CreatedOwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_ReviewedByUserId",
                table: "CompanyRegistrations",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_Status_CreatedAtUtc",
                table: "CompanyRegistrations",
                columns: new[] { "Status", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyRegistrations");
        }
    }
}
