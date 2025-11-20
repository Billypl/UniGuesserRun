using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniGuesser.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Rounds",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_Distance",
                table: "Rounds",
                column: "Distance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rounds_Distance",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Rounds");
        }
    }
}
