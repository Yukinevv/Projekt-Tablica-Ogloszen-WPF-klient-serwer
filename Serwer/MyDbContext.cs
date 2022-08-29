using Microsoft.EntityFrameworkCore;
using BibliotekaEncje.Encje;

namespace Serwer
{
    /// <summary>
    /// Klasa odpowiadajaca za konfiguracje Entity Frameworka (bazy danych)
    /// </summary>
    public class MyDbContext : DbContext
    {
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }

        public DbSet<Ogloszenie> Ogloszenia { get; set; }

        public DbSet<Kategoria> Kategorie { get; set; }

        public DbSet<OgloszenieKategoria> OgloszeniaKategorie { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Uzytkownik>(u =>
            {
                u.Property(u => u.Login).IsRequired().HasColumnType("varchar(30)");
                u.Property(u => u.Haslo).IsRequired().HasColumnType("varchar(255)");
                u.Property(u => u.Imie).HasColumnType("varchar(30)");
                u.Property(u => u.Nazwisko).HasColumnType("varchar(30)");
                u.Property(u => u.Email).IsRequired().HasColumnType("varchar(30)");
                u.Property(u => u.Uprawnienia).IsRequired().HasColumnType("varchar(10)");

                // relacje jeden do wielu
                u.HasMany(u => u.Ogloszenia).WithOne(o => o.Uzytkownik).HasForeignKey(o => o.UzytkownikId);
                u.HasMany(u => u.Kategorie).WithOne(k => k.Uzytkownik).HasForeignKey(k => k.UzytkownikId);
            });

            modelBuilder.Entity<Ogloszenie>(o =>
            {
                o.Property(o => o.Tytul).IsRequired().HasColumnType("varchar(30)");
                o.Property(o => o.Tresc).IsRequired().HasColumnType("varchar(4096)");
                o.Property(o => o.Data_ed).ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<Kategoria>(k =>
            {
                k.Property(k => k.Nazwa).IsRequired().HasColumnType("varchar(30)");
            });

            // relacja wiele do wielu
            modelBuilder.Entity<OgloszenieKategoria>(ok =>
            {
                ok.HasKey(x => new { x.OgloszenieId, x.KategoriaId });
                ok.HasOne(ok => ok.Ogloszenie).WithMany(o => o.Kategorie).HasForeignKey(ok => ok.OgloszenieId).OnDelete(DeleteBehavior.Restrict);
                ok.HasOne(ok => ok.Kategoria).WithMany(k => k.Ogloszenia).HasForeignKey(ok => ok.KategoriaId).OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=sxterm;Database=adrianrodzic;Username=adrianrodzic;Password=ADrian8151!@#");
    }
}
