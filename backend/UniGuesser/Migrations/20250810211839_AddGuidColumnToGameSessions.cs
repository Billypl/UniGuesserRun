using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyGame.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidColumnToGameSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "GameSessions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "GameSessions");
        }

    }
}
