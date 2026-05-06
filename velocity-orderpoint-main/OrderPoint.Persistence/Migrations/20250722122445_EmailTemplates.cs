using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPoint.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BccOrderConfirmation",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblListTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblListTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentID = table.Column<long>(type: "bigint", nullable: true),
                    HierLevel = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ListTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblLists_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblLists_tblListTypes_ListTypeId",
                        column: x => x.ListTypeId,
                        principalTable: "tblListTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblEmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblEmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblEmailTemplates_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblEmailTemplates_AspNetUsers_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tblEmailTemplates_tblLists_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "tblLists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblEmailTemplates_CreatedBy",
                table: "tblEmailTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_tblEmailTemplates_EmailTypeId",
                table: "tblEmailTemplates",
                column: "EmailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tblEmailTemplates_UpdatedBy",
                table: "tblEmailTemplates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_tblLists_CreatedBy",
                table: "tblLists",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_tblLists_ListTypeId",
                table: "tblLists",
                column: "ListTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblEmailTemplates");

            migrationBuilder.DropTable(
                name: "tblLists");

            migrationBuilder.DropTable(
                name: "tblListTypes");

            migrationBuilder.DropColumn(
                name: "BccOrderConfirmation",
                table: "AspNetUsers");
        }
    }
}
