using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPoint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDescritionandimageforWholesaler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImage",
                table: "tbWholesaler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "tbWholesaler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeMessage",
                table: "tbWholesaler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImage",
                table: "tbWholesaler");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "tbWholesaler");

            migrationBuilder.DropColumn(
                name: "WelcomeMessage",
                table: "tbWholesaler");
        }
    }
}
