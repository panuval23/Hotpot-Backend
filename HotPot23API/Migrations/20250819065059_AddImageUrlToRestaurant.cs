using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotPot23API.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RestaurantMasters",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RestaurantMasters");
        }
    }
}
