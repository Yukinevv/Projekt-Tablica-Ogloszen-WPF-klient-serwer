﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Serwer;

#nullable disable

namespace Serwer.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20220715073634_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BibliotekaEncje.Encje.Kategoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data_utw")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<int>("UzytkownikId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UzytkownikId");

                    b.ToTable("Kategorie");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Ogloszenie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data_ed")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Data_utw")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Tresc")
                        .IsRequired()
                        .HasColumnType("varchar(4096)");

                    b.Property<string>("Tytul")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<int>("UzytkownikId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UzytkownikId");

                    b.ToTable("Ogloszenia");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.OgloszenieKategoria", b =>
                {
                    b.Property<int>("OgloszenieId")
                        .HasColumnType("integer");

                    b.Property<int>("KategoriaId")
                        .HasColumnType("integer");

                    b.HasKey("OgloszenieId", "KategoriaId");

                    b.HasIndex("KategoriaId");

                    b.ToTable("OgloszeniaKategorie");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Uzytkownik", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data_ur")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Haslo")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Imie")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Nazwisko")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Uprawnienia")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Uzytkownicy");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Kategoria", b =>
                {
                    b.HasOne("BibliotekaEncje.Encje.Uzytkownik", "Uzytkownik")
                        .WithMany("Kategorie")
                        .HasForeignKey("UzytkownikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uzytkownik");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Ogloszenie", b =>
                {
                    b.HasOne("BibliotekaEncje.Encje.Uzytkownik", "Uzytkownik")
                        .WithMany("Ogloszenia")
                        .HasForeignKey("UzytkownikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uzytkownik");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.OgloszenieKategoria", b =>
                {
                    b.HasOne("BibliotekaEncje.Encje.Kategoria", "Kategoria")
                        .WithMany("Ogloszenia")
                        .HasForeignKey("KategoriaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BibliotekaEncje.Encje.Ogloszenie", "Ogloszenie")
                        .WithMany("Kategorie")
                        .HasForeignKey("OgloszenieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Kategoria");

                    b.Navigation("Ogloszenie");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Kategoria", b =>
                {
                    b.Navigation("Ogloszenia");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Ogloszenie", b =>
                {
                    b.Navigation("Kategorie");
                });

            modelBuilder.Entity("BibliotekaEncje.Encje.Uzytkownik", b =>
                {
                    b.Navigation("Kategorie");

                    b.Navigation("Ogloszenia");
                });
#pragma warning restore 612, 618
        }
    }
}
