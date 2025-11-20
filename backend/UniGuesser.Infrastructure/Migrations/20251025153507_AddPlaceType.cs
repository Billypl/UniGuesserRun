using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniGuesser.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageType",
                table: "Places",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "Places");
        }
    }
}
