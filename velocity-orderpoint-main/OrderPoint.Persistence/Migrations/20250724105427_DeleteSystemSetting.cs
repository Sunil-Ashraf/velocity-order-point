using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPoint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSystemSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings");

            migrationBuilder.DropColumn(
                name: "IsSensitive",
                table: "tblSystemSettings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "tblSystemSettings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                table: "tblSystemSettings",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "tblSystemSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedBy",
                table: "tblSystemSettings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSensitive",
                table: "tblSystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
