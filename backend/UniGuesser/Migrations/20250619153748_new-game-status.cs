using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyGame.Migrations
{
    /// <inheritdoc />
    public partial class newgamestatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "GameSessions");

            migrationBuilder.AddColumn<int>(
                name: "GameState",
                table: "GameSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameState",
                table: "GameSessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "GameSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
