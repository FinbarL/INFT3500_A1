/*
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class change_userid_to_username : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropColumn(
                name: "PatronId",
                table: "TO");

            migrationBuilder.AddColumn<bool>(
                name: "isStaff",
                table: "User",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "TO",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TO_UserName",
                table: "TO",
                column: "UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_TO_Users",
                table: "TO",
                column: "UserName",
                principalTable: "User",
                principalColumn: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TO_Users",
                table: "TO");

            migrationBuilder.DropIndex(
                name: "IX_TO_UserName",
                table: "TO");

            migrationBuilder.DropColumn(
                name: "isStaff",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "TO");

            migrationBuilder.AddColumn<int>(
                name: "PatronId",
                table: "TO",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HashPW = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Salt = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.UserID);
                });
        }
    }
}
*/
