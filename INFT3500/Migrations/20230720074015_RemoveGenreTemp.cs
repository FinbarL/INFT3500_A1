using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGenreTemp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("GenreTemp", "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("GenreTemp", "Product", nullable: true);
            migrationBuilder.Sql(
                @"UPDATE Product SET GenreTemp = CASE Genre
                  WHEN 1 THEN 'Books'
                  WHEN 2 THEN 'Movies'
                  WHEN 3 THEN 'Games'
                  ELSE 'Unknown' 
                  END
                  ");
        }
    }
}
