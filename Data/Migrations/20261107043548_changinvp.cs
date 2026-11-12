using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class changinvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReciverInfoid",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReciverInfos",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciverInfos", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ReciverInfoid",
                table: "Conversations",
                column: "ReciverInfoid");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ReciverInfos_ReciverInfoid",
                table: "Conversations",
                column: "ReciverInfoid",
                principalTable: "ReciverInfos",
                principalColumn: "id");
        }
    }
}
