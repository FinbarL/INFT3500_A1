using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class DropPatronsMergeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "subGenre",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>("isStaff", "User", type: "bit", nullable: true);
            migrationBuilder.AddColumn<string>("patronID", "User", type: "int", nullable: true);
            migrationBuilder.Sql(
                @"UPDATE [User] SET isStaff = CASE isAdmin
                  WHEN 1 THEN 0
                  WHEN 0 THEN 1
                  ELSE 0
                  END
                  ");

            migrationBuilder.Sql(
                @"INSERT INTO [User] (UserName, Email, Name, isStaff, isAdmin, Salt, HashPW, patronId)
                  SELECT
                  SUBSTRING(Email, 1, CHARINDEX('@', Email) - 1) AS UserName,
                  Email,
                  Name,
                  0 AS isStaff, 
                  0 AS isAdmin,
                  Salt,
                  HashPW,
                  UserID as patronId
                  FROM Patrons");
            migrationBuilder.DropForeignKey("FK_TO_Patrons", "TO");
            migrationBuilder.RenameColumn("PatronId", "TO", "UserID");
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "TO",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.Sql(@"UPDATE [TO] SET UserID = (SELECT UserName FROM [User] WHERE patronId = [TO].UserID)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "subGenre",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
