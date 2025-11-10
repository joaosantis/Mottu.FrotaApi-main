using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mottu.FrotaApi.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoCamposVisao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PosicaoX",
                table: "Motos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PosicaoY",
                table: "Motos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisaoId",
                table: "Motos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosicaoX",
                table: "Motos");

            migrationBuilder.DropColumn(
                name: "PosicaoY",
                table: "Motos");

            migrationBuilder.DropColumn(
                name: "VisaoId",
                table: "Motos");
        }
    }
}
