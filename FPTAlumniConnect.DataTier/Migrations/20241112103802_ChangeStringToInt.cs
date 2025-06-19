using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTAlumniConnect.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStringToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MinSalary",
                table: "CV",
                type: "int",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "MaxSalary",
                table: "CV",
                type: "int",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MinSalary",
                table: "CV",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<string>(
                name: "MaxSalary",
                table: "CV",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "((0))");
        }
    }
}
