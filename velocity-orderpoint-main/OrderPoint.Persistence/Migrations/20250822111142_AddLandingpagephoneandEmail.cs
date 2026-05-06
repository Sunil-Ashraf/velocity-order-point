using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPoint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLandingpagephoneandEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LandingPageEmail",
                table: "tbWholesaler",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandingPageTelephone",
                table: "tbWholesaler",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandingPageEmail",
                table: "tbWholesaler");

            migrationBuilder.DropColumn(
                name: "LandingPageTelephone",
                table: "tbWholesaler");
        }
    }
}
