using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTAlumniConnect.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobPostID",
                table: "SkillJob",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "JobPost",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MajorId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillJob_JobPostID",
                table: "SkillJob",
                column: "JobPostID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_MajorId",
                table: "Events",
                column: "MajorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_MajorCode_MajorId",
                table: "Events",
                column: "MajorId",
                principalTable: "MajorCode",
                principalColumn: "MajorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillJob_JobPost_JobPostID",
                table: "SkillJob",
                column: "JobPostID",
                principalTable: "JobPost",
                principalColumn: "JobPostID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_MajorCode_MajorId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillJob_JobPost_JobPostID",
                table: "SkillJob");

            migrationBuilder.DropIndex(
                name: "IX_SkillJob_JobPostID",
                table: "SkillJob");

            migrationBuilder.DropIndex(
                name: "IX_Events_MajorId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "JobPostID",
                table: "SkillJob");

            migrationBuilder.DropColumn(
                name: "City",
                table: "JobPost");

            migrationBuilder.DropColumn(
                name: "MajorId",
                table: "Events");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Events",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
