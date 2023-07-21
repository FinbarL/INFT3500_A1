using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class AddTempGenreSubGenre_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
            migrationBuilder.AddColumn<string>("SubGenreTemp", "Product", nullable: true);
            migrationBuilder.Sql(
                @"UPDATE Product SET SubGenreTemp = CASE
    WHEN (Genre = 1 AND SubGenre < 10) THEN (SELECT Name FROM Book_genre WHERE SubGenreId = SubGenre)
    WHEN (Genre = 1 AND SubGenre > 11) THEN (SELECT Name FROM [Book_genre NEW] WHERE SubGenreId = SubGenre)
    WHEN (Genre = 2) THEN (SELECT Name FROM Movie_genre WHERE SubGenreId = SubGenre)
    WHEN (Genre = 3) THEN (SELECT Name FROM Game_genre WHERE SubGenreId = SubGenre)
    ELSE 'Unknown'
    END
                  ");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("GenreTemp", "Product");
            migrationBuilder.DropColumn("SubGenreTemp", "Product");
        }
    }
}
