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

        public void DodajStartoweOgloszenia()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Ogloszenia.Any())
                {
                    var ogloszenia = PrzykladoweOgloszenia();
                    _dbContext.Ogloszenia.AddRange(ogloszenia);
                    _dbContext.SaveChanges();
                }
                else
                {
                    //MessageBox.Show("Przykladowe OGLOSZENIA juz znajduja sie w bazie!");
                }
            }
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
                }
            };
            return ogloszenia;
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
                        string[] Nazwisko = new string[] { "Kowalski", "Nowak", "Madry", "Pracowity", "Lukasiewicz" };
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
                    else
                    {
                        MessageBox.Show("Przykladowi UZYTKOWNICY juz znajduja sie w bazie!");
                    }
                }
            }
        }

    }
}
