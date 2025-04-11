using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyGame.Migrations
{
    /// <inheritdoc />
    public partial class correctusings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Users_AuthorId",
                table: "Places");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Users_AuthorId",
                table: "Places",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Users_AuthorId",
                table: "Places");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Users_AuthorId",
                table: "Places",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
