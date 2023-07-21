using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFT3500.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSubGenreWithTemp_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("SubGenre", "Product", "SubGenreOld");
            migrationBuilder.RenameColumn("SubGenreTemp", "Product", "SubGenre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("SubGenre", "Product", "SubGenreTemp");
            migrationBuilder.RenameColumn("SubGenreOld", "Product", "SubGenre");
        }
    }
}
