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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE [User] DROP CONSTRAINT UQ_User_UserID");
        }
    }
}
