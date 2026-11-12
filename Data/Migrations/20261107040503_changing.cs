using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class changing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ReciverInfos_ReciverId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_CoversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ReciverInfos_ReciverId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_ReciverId",
                table: "Conversations",
                column: "ReciverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReciverId",
                table: "Messages",
                column: "ReciverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_CoversationId",
                table: "Messages",
                column: "CoversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_ReciverId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ReciverInfos_ReciverInfoid",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReciverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_CoversationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ReciverInfoid",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ReciverInfoid",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ReciverInfos_ReciverId",
                table: "Conversations",
                column: "ReciverId",
                principalTable: "ReciverInfos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_CoversationId",
                table: "Messages",
                column: "CoversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ReciverInfos_ReciverId",
                table: "Messages",
                column: "ReciverId",
                principalTable: "ReciverInfos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
