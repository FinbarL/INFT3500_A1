using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class Add_FK_TO_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE [User] ADD CONSTRAINT UQ_User_UserID UNIQUE (UserID)");
            
            migrationBuilder.CreateIndex(
                name: "IX_TO_UserID",
                table: "TO",
                column: "UserID");
            migrationBuilder.AddForeignKey(
                name: "FK_TO_User_UserID",
                table: "TO",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TO_User_UserID",
                table: "TO");

            migrationBuilder.DropIndex(
                name: "IX_TO_UserID",
                table: "TO");
            migrationBuilder.Sql(@"ALTER TABLE [User] DROP CONSTRAINT UQ_User_UserID");
        }
    }
}
