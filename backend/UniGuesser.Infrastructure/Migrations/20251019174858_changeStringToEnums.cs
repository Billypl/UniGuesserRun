using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniGuesser.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeStringToEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GameState",
                table: "GameSessions",
                type: "text",
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "GameMode",
                table: "GameSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GameState",
                table: "GameSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "InProgress");

            migrationBuilder.AlterColumn<int>(
                name: "GameMode",
                table: "GameSessions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
