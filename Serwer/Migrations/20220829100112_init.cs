using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Serwer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uzytkownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "varchar(30)", nullable: false),
                    Haslo = table.Column<string>(type: "varchar(255)", nullable: false),
                    Imie = table.Column<string>(type: "varchar(30)", nullable: false),
                    Nazwisko = table.Column<string>(type: "varchar(30)", nullable: false),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    Data_ur = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Uprawnienia = table.Column<string>(type: "varchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategorie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nazwa = table.Column<string>(type: "varchar(30)", nullable: false),
                    Data_utw = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UzytkownikId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategorie_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ogloszenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tytul = table.Column<string>(type: "varchar(30)", nullable: false),
                    Data_utw = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Data_ed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tresc = table.Column<string>(type: "varchar(4096)", nullable: false),
                    UzytkownikId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogloszenia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ogloszenia_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Komentarze",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tresc = table.Column<string>(type: "varchar(2048)", nullable: false),
                    UzytkownikId = table.Column<int>(type: "integer", nullable: false),
                    OgloszenieId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komentarze", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Komentarze_Ogloszenia_OgloszenieId",
                        column: x => x.OgloszenieId,
                        principalTable: "Ogloszenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Komentarze_Uzytkownicy_UzytkownikId",
                        column: x => x.UzytkownikId,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OgloszeniaKategorie",
                columns: table => new
                {
                    OgloszenieId = table.Column<int>(type: "integer", nullable: false),
                    KategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgloszeniaKategorie", x => new { x.OgloszenieId, x.KategoriaId });
                    table.ForeignKey(
                        name: "FK_OgloszeniaKategorie_Kategorie_KategoriaId",
                        column: x => x.KategoriaId,
                        principalTable: "Kategorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OgloszeniaKategorie_Ogloszenia_OgloszenieId",
                        column: x => x.OgloszenieId,
                        principalTable: "Ogloszenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kategorie_UzytkownikId",
                table: "Kategorie",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Komentarze_OgloszenieId",
                table: "Komentarze",
                column: "OgloszenieId");

            migrationBuilder.CreateIndex(
                name: "IX_Komentarze_UzytkownikId",
                table: "Komentarze",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Ogloszenia_UzytkownikId",
                table: "Ogloszenia",
                column: "UzytkownikId");

            migrationBuilder.CreateIndex(
                name: "IX_OgloszeniaKategorie_KategoriaId",
                table: "OgloszeniaKategorie",
                column: "KategoriaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Komentarze");

            migrationBuilder.DropTable(
                name: "OgloszeniaKategorie");

            migrationBuilder.DropTable(
                name: "Kategorie");

            migrationBuilder.DropTable(
                name: "Ogloszenia");

            migrationBuilder.DropTable(
                name: "Uzytkownicy");
        }
    }
}
