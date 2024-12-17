using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersApi.Migrations
{
    /// <inheritdoc />
    public partial class av : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                schema: "Db",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarId",
                schema: "Db",
                table: "Users");
        }
    }
}
