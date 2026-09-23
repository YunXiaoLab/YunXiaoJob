using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YunXiaoJob.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandCandidateProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "CandidateProfiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "CandidateProfiles",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedMaxSalary",
                table: "CandidateProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedMinSalary",
                table: "CandidateProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpectedSalaryCurrency",
                table: "CandidateProfiles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "CandidateProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "CandidateProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "CandidateProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioUrl",
                table: "CandidateProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CandidateEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Major = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    Gpa = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    GpaScale = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEducations_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateExperiences_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Proficiency = table.Column<int>(type: "int", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    LastUsedYear = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateSkills_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_IsSearchable_YearsOfExperience",
                table: "CandidateProfiles",
                columns: new[] { "IsSearchable", "YearsOfExperience" });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_CandidateProfileId_Level",
                table: "CandidateEducations",
                columns: new[] { "CandidateProfileId", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_Major",
                table: "CandidateEducations",
                column: "Major");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateExperiences_CandidateProfileId_StartDate",
                table: "CandidateExperiences",
                columns: new[] { "CandidateProfileId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateExperiences_JobTitle",
                table: "CandidateExperiences",
                column: "JobTitle");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkills_CandidateProfileId_Name",
                table: "CandidateSkills",
                columns: new[] { "CandidateProfileId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkills_Name_Proficiency",
                table: "CandidateSkills",
                columns: new[] { "Name", "Proficiency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateEducations");

            migrationBuilder.DropTable(
                name: "CandidateExperiences");

            migrationBuilder.DropTable(
                name: "CandidateSkills");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_IsSearchable_YearsOfExperience",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ExpectedMaxSalary",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ExpectedMinSalary",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ExpectedSalaryCurrency",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PortfolioUrl",
                table: "CandidateProfiles");
        }
    }
}
