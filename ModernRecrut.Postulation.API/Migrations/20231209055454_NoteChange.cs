using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModernRecrut.Postulation.API.Migrations
{
    public partial class NoteChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCandidat",
                table: "NoteDetail");

            migrationBuilder.AddColumn<int>(
                name: "IdPostulation",
                table: "NoteDetail",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPostulation",
                table: "NoteDetail");

            migrationBuilder.AddColumn<string>(
                name: "IdCandidat",
                table: "NoteDetail",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
