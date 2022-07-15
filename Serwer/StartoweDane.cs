using BibliotekaEncje.Encje;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Serwer
{
    public class StartoweDane
    {
        private readonly MyDbContext _dbContext;

        public StartoweDane(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DodajStartoweDane()
        {
            if (_dbContext.Database.CanConnect())
            {
                DodajStartowychUzytkownikow(); // doda jezeli nie ma zadnych w bazie
                if (!_dbContext.Kategorie.Any())
                {
                    var kategorie = PrzykladoweKategorie();
                    _dbContext.Kategorie.AddRange(kategorie);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Ogloszenia.Any())
                {
                    var ogloszenia = PrzykladoweOgloszenia();
                    _dbContext.Ogloszenia.AddRange(ogloszenia);
                    _dbContext.SaveChanges();           
                }
                if (!_dbContext.OgloszeniaKategorie.Any())
                {
                    var relacje = RelacjeOgloszeniaKategorie();
                    _dbContext.OgloszeniaKategorie.AddRange(relacje);
                    _dbContext.SaveChanges();
                }
            }
        }

        public static void DodajStartowychUzytkownikow()
        {
            using (var dbContext = new MyDbContext())
            {
                if (dbContext.Database.CanConnect())
                {
                    if (!dbContext.Uzytkownicy.Any())
                    {
                        string[] Imie = new string[] { "Adrian", "Pawel", "Bartek", "Lukasz", "Czesio" };
                        string[] Nazwisko = new string[] { "Kowalski", "Nowak", "Lukasiewicz", "Nowy", "Stary" };
                        Random r = new Random();

                        for (int i = 1; i <= 5; i++)
                        {
                            string wybraneImie = Imie[r.Next(0, Imie.Length)];
                            string wybraneNazwisko = Nazwisko[r.Next(0, Nazwisko.Length)];

                            Uzytkownik uzytkownik = new Uzytkownik()
                            {
                                Login = wybraneImie + wybraneNazwisko,
                                Haslo = "qwerty",
                                Imie = wybraneImie,
                                Nazwisko = wybraneNazwisko,
                                Email = wybraneImie + "@gmail.com",
                                Data_ur = DateTime.Now,
                                Uprawnienia = "uzytkownik"
                            };

                            dbContext.Uzytkownicy.Add(uzytkownik);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private static IEnumerable<Kategoria> PrzykladoweKategorie()
        {
            var ogloszenia = new List<Kategoria>()
            {
                new Kategoria()
                {
                    Nazwa = "Pilka nozna",
                    UzytkownikId = 1,
                    Data_utw = DateTime.Now
                },

                new Kategoria()
                {
                    Nazwa = "Gry komputerowe",
                    UzytkownikId = 2,
                    Data_utw = DateTime.Now
                },

                new Kategoria()
                {
                    Nazwa = "Zaginione zwierzeta",
                    UzytkownikId = 4,
                    Data_utw = DateTime.Now
                }
            };
            return ogloszenia;
        }

        private static IEnumerable<Ogloszenie> PrzykladoweOgloszenia()
        {
            var ogloszenia = new List<Ogloszenie>()
            {
                new Ogloszenie()
                {
                    Tytul = "Bramka Ronaldo",
                    Data_utw = DateTime.Now,
                    Data_ed = DateTime.Now,
                    Tresc = "brawo mamy bramke!!!",
                    UzytkownikId = 1
                },

                new Ogloszenie()
                {
                    Tytul = "Nowy Wiedzmin na szlaku",
                    Data_utw = DateTime.Now,
                    Data_ed = DateTime.Now,
                    Tresc = "zabojca potworow ma rece pelne roboty",
                    UzytkownikId = 2
                },

                new Ogloszenie()
                {
                    Tytul = "Bieg na 1000m w Solankach",
                    Data_utw = DateTime.Now,
                    Data_ed = DateTime.Now,
                    Tresc = "zapraszamy do udzialu",
                    UzytkownikId = 3
                }
            };
            return ogloszenia;
        }

        private static IEnumerable<OgloszenieKategoria> RelacjeOgloszeniaKategorie()
        {
            var relacje = new List<OgloszenieKategoria>()
            {
                new OgloszenieKategoria()
                {
                    OgloszenieId = 1,
                    KategoriaId = 1
                },

                new OgloszenieKategoria()
                {
                    OgloszenieId = 1,
                    KategoriaId = 2
                },

                new OgloszenieKategoria()
                {
                    OgloszenieId = 2,
                    KategoriaId = 3
                },

                new OgloszenieKategoria()
                {
                    OgloszenieId = 3,
                    KategoriaId = 3
                }
            };
            return relacje;
        }
    }
}
