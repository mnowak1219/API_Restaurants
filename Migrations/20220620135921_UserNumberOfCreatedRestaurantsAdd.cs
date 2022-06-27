using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Restaurants.Migrations
{
    public partial class UserNumberOfCreatedRestaurantsAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfCreatedRestaurants",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCreatedRestaurants",
                table: "Users");
        }
    }
}
