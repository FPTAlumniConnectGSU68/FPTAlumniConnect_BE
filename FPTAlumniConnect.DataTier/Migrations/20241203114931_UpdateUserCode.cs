using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTAlumniConnect.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Users__RoleID__5BE2A6F2",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Users__RoleID__5BE2A6F2",
                table: "Users",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Users__RoleID__5BE2A6F2",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "RoleID",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__RoleID__5BE2A6F2",
                table: "Users",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID");
        }
    }
}
