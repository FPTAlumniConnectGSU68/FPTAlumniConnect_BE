using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTAlumniConnect.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột Status dạng string để lưu enum dạng chuỗi
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CV",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "Public"); // Hoặc giá trị mặc định phù hợp với enum của bạn

            // Các cột bổ sung
            migrationBuilder.AddColumn<int>(
                name: "SkillJobId",
                table: "CV",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MajorId",
                table: "CV",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "CV",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesiredJob",
                table: "CV",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalContent",
                table: "CV",
                type: "nvarchar(max)",
                nullable: true);

            // Tạo index nếu có liên kết
            migrationBuilder.CreateIndex(
                name: "IX_CV_MajorId",
                table: "CV",
                column: "MajorId");

            migrationBuilder.AddForeignKey(
                    name: "FK_CV_MajorCode",
                    table: "CV",
                    column: "MajorId",
                    principalTable: "MajorCode",
                    principalColumn: "MajorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CV_SkillJob_SkillJobId",
                table: "CV");

            migrationBuilder.DropIndex(
                name: "IX_CV_MajorId",
                table: "CV");

            migrationBuilder.DropIndex(
                name: "IX_CV_SkillJobId",
                table: "CV");

            migrationBuilder.RenameColumn(
                name: "AdditionalInfo",
                table: "CV",
                newName: "AdditionalContent");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CV",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "CV",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MajorId",
                table: "CV",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DesiredJob",
                table: "CV",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
