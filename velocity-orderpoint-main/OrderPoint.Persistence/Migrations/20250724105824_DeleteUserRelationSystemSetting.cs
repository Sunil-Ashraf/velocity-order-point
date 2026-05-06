using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPoint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUserRelationSystemSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_tblSystemSettings_CreatedBy",
                table: "tblSystemSettings");

      

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tblSystemSettings_CreatedBy",
                table: "tblSystemSettings",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSystemSettings_AspNetUsers_CreatedBy",
                table: "tblSystemSettings",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
