using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Serwer
{
    public class PoleceniaOdKlienta
    {
        public static void Rejestracja()
        {
            string zserializowanyObiekt = OperacjeSerwer.Odbierz();
            var obiekt = JsonConvert.DeserializeObject<Uzytkownik>(zserializowanyObiekt);

            bool czyMogeDodac = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == obiekt.Login);

            if (!czyMogeDodac)
            {
                DataBaseLocator.Context.Uzytkownicy.Add(obiekt);
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("zarejestrowano");
            }
            else
            {
                string komunikat = "Uzytkownik o podanym loginie juz istnieje!";
                OperacjeSerwer.Wyslij(komunikat);
            }
        }

        public static void Logowanie()
        {
            string login = OperacjeSerwer.Odbierz();
            OperacjeSerwer.Wyslij("OK");
            string haslo = OperacjeSerwer.Odbierz();

            bool czyZalogowac = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == login && u.Haslo == haslo);
            if (czyZalogowac)
            {
                OperacjeSerwer.Wyslij("true");
            }
            else
            {
                OperacjeSerwer.Wyslij("false");
            }
        }

        public static void CzyAdmin()
        {
            string login = OperacjeSerwer.Odbierz();

            bool czyAdmin = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == login && u.Uprawnienia == "admin");
            if (czyAdmin)
            {
                OperacjeSerwer.Wyslij("admin");
            }
            else
            {
                OperacjeSerwer.Wyslij("nie admin");
            }
        }

        public static void Kategorie()
        {
            var kategorie = DataBaseLocator.Context.Kategorie.ToList();
            string katSerialized = JsonConvert.SerializeObject(kategorie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(katSerialized);
        }

        public static void WybraneNazwyKategorii()
        {
            string wiadomosc = OperacjeSerwer.Odbierz();
            int idOgloszenia = int.Parse(wiadomosc);
            var idWybranychKategorii = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia)
                .Select(ok => ok.KategoriaId).ToList();

            var nazwyKategorii = new List<string>();
            foreach (var idWybranejKategorii in idWybranychKategorii)
            {
                string? nazwaKategorii = DataBaseLocator.Context.Kategorie.Where(k => k.Id == idWybranejKategorii)
                    .Select(k => k.Nazwa).FirstOrDefault();
                nazwyKategorii.Add(nazwaKategorii);
            }
            string nazwyKategoriiSerialized = JsonConvert.SerializeObject(nazwyKategorii);
            OperacjeSerwer.Wyslij(nazwyKategoriiSerialized);
        }

        public static void DodanieKategorii()
        {
            string kategoriaSerialized = OperacjeSerwer.Odbierz();
            var kategoriaOdKlienta = JsonConvert.DeserializeObject<Kategoria>(kategoriaSerialized);
            string login = OperacjeSerwer.Odbierz();

            bool czyDodac = DataBaseLocator.Context.Kategorie.Any(k => k.Nazwa == kategoriaOdKlienta.Nazwa);
            if (!czyDodac)
            {
                int idUzytkownika = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
                var nowaKategoria = new Kategoria()
                {
                    Nazwa = kategoriaOdKlienta.Nazwa,
                    Data_utw = DateTime.Now,
                    UzytkownikId = idUzytkownika
                };
                DataBaseLocator.Context.Kategorie.Add(nowaKategoria);
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("Dodano");
            }
            else
            {
                string komunikat = "Kategoria o podanej nazwie juz istnieje! Prosze podac inna nazwe.";
                OperacjeSerwer.Wyslij(komunikat);
            }
        }

        public static void UsuniecieKategorii()
        {
            string nazwaKategorii = OperacjeSerwer.Odbierz();

            // sprawdzam czy kategoria o podanej przez klienta nazwie znajduje sie w bazie
            bool czyKategoriaIstnieje = DataBaseLocator.Context.Kategorie.Any(k => k.Nazwa == nazwaKategorii);

            if (czyKategoriaIstnieje)
            {
                int idKategorii = DataBaseLocator.Context.Kategorie.Where(k => k.Nazwa == nazwaKategorii).Select(k => k.Id).FirstOrDefault();
                // sprawdzam czy w usuwanej kategorii znajduja sie jakies ogloszenia, jezeli nie to usuwam kategorie
                bool czyMogeUsunac = DataBaseLocator.Context.OgloszeniaKategorie.Any(ok => ok.KategoriaId == idKategorii);

                if (!czyMogeUsunac)
                {
                    var kategoria = DataBaseLocator.Context.Kategorie.Where(k => k.Nazwa == nazwaKategorii).FirstOrDefault();
                    DataBaseLocator.Context.Kategorie.Remove(kategoria);
                    DataBaseLocator.Context.SaveChanges();

                    OperacjeSerwer.Wyslij("usunieto");
                }
                else
                {
                    OperacjeSerwer.Wyslij("nie usunieto");
                }
            }
            else
            {
                OperacjeSerwer.Wyslij("nie ma takiej kategorii");
            }
        }

        public static void Ogloszenia()
        {
            string wiadomosc = OperacjeSerwer.Odbierz();
            int idKategorii = int.Parse(wiadomosc);

            var idOgloszen = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.KategoriaId == idKategorii)
                .Select(ok => ok.OgloszenieId).ToList();

            var ogloszenia = new List<Ogloszenie>();
            foreach (var idOgloszenia in idOgloszen)
            {
                var ogloszenie = DataBaseLocator.Context.Ogloszenia.FirstOrDefault(o => o.Id == idOgloszenia);
                ogloszenia.Add(ogloszenie);
            }
            string oglSerialized = JsonConvert.SerializeObject(ogloszenia, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(oglSerialized);
        }

        public static void MojeOgloszenia()
        {
            string login = OperacjeSerwer.Odbierz();

            int idUzytkownika = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
            var ogloszenia = DataBaseLocator.Context.Ogloszenia.Where(o => o.UzytkownikId == idUzytkownika).ToList();
            string oglSerialized = JsonConvert.SerializeObject(ogloszenia, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(oglSerialized);
        }

        public static void DodanieOgloszenia()
        {
            string login = OperacjeSerwer.Odbierz();

            int idUzytkownika = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();

            // dodanie samego ogloszenia
            string zserializowanyObiekt = OperacjeSerwer.Odbierz();
            var ogloszenie = JsonConvert.DeserializeObject<Ogloszenie>(zserializowanyObiekt);
            ogloszenie.UzytkownikId = idUzytkownika;

            DataBaseLocator.Context.Ogloszenia.Add(ogloszenie);
            DataBaseLocator.Context.SaveChanges();

            OperacjeSerwer.Wyslij("dodano ogloszenie");

            // dodanie relacji do tabeli OgloszeniaKategorie
            int idOgloszenia = DataBaseLocator.Context.Ogloszenia.OrderBy(o => o.Id).Select(o => o.Id).LastOrDefault();

            string nazwyKategoriiSerialized = OperacjeSerwer.Odbierz();
            var nazwyKategorii = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);
            var idKategorii = new List<int>();

            foreach (var nazwa in nazwyKategorii)
            {
                idKategorii.Add(DataBaseLocator.Context.Kategorie.Where(k => k.Nazwa == nazwa).Select(k => k.Id).FirstOrDefault());
            }
            foreach (var id in idKategorii)
            {
                DataBaseLocator.Context.OgloszeniaKategorie.Add(new OgloszenieKategoria() { OgloszenieId = idOgloszenia, KategoriaId = id });
            }
            DataBaseLocator.Context.SaveChanges();
            OperacjeSerwer.Wyslij("zakonczono dodawanie");
        }

        public static void UsuniecieOgloszenia()
        {
            string wiadomosc = OperacjeSerwer.Odbierz();
            int idOgloszenia = int.Parse(wiadomosc);

            // usuniecie samego ogloszenia
            var ogloszenie = DataBaseLocator.Context.Ogloszenia.Where(o => o.Id == idOgloszenia).FirstOrDefault();
            DataBaseLocator.Context.Ogloszenia.Remove(ogloszenie);

            // usuniecie relacji pomiedzy Ogloszeniami a Kategoriami
            var relacje = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia);
            DataBaseLocator.Context.OgloszeniaKategorie.RemoveRange(relacje);
            DataBaseLocator.Context.SaveChanges();
            OperacjeSerwer.Wyslij("Usunieto");
        }

        public static void EdycjaOgloszenia()
        {
            string oglSeralized = OperacjeSerwer.Odbierz();
            var ogloszenieOdKlienta = JsonConvert.DeserializeObject<Ogloszenie>(oglSeralized);

            var ogloszenie = DataBaseLocator.Context.Ogloszenia.Where(o => o.Id == ogloszenieOdKlienta.Id).FirstOrDefault();
            ogloszenie.Tytul = ogloszenieOdKlienta.Tytul;
            ogloszenie.Data_ed = DateTime.Now;
            ogloszenie.Tresc = ogloszenieOdKlienta.Tresc;
            DataBaseLocator.Context.SaveChanges();

            OperacjeSerwer.Wyslij("zedytowano ogloszenie");

            // edycja relacji w tabeli OgloszeniaKategorie
            // usuniecie wczesniejszych relacji
            int idOgloszenia = ogloszenie.Id;
            var relacje = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia).ToList();
            DataBaseLocator.Context.OgloszeniaKategorie.RemoveRange(relacje);
            DataBaseLocator.Context.SaveChanges();

            // dodanie nowych relacji
            string nazwyKategoriiSerialized = OperacjeSerwer.Odbierz();
            var nazwyKategorii = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);
            var idKategorii = new List<int>();

            foreach (var nazwa in nazwyKategorii)
            {
                idKategorii.Add(DataBaseLocator.Context.Kategorie.Where(k => k.Nazwa == nazwa).Select(k => k.Id).FirstOrDefault());
            }
            foreach (var id in idKategorii)
            {
                DataBaseLocator.Context.OgloszeniaKategorie.Add(new OgloszenieKategoria() { OgloszenieId = idOgloszenia, KategoriaId = id });
            }
            DataBaseLocator.Context.SaveChanges();
            OperacjeSerwer.Wyslij("zakonczono edycje");
        }

        public static void CzyMozeEdytowac()
        {
            string login = OperacjeSerwer.Odbierz();
            OperacjeSerwer.Wyslij("OK");
            string wiadomosc = OperacjeSerwer.Odbierz();
            Debug.WriteLine("Id uzytkownika wybranego ogloszenia = " + wiadomosc);
            int idUzytkownikaWybranegoOgloszenia = int.Parse(wiadomosc);

            int idUzytkownikZalogowanego = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
            bool czyAdmin = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Id == idUzytkownikZalogowanego && u.Uprawnienia == "admin").Any();
            if (idUzytkownikZalogowanego == idUzytkownikaWybranegoOgloszenia || czyAdmin == true)
            {
                OperacjeSerwer.Wyslij("TAK");
            }
            else
            {
                OperacjeSerwer.Wyslij("NIE");
            }
        }

        public static void MojeDane()
        {
            string login = OperacjeSerwer.Odbierz();

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(uzytkownikSerialized);
        }

        public static void EdycjaDanychUzytkownika()
        {
            string uzytkownikOdKlientaSerialized = OperacjeSerwer.Odbierz();
            var uzytkownikOdKlienta = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikOdKlientaSerialized);
            string login = OperacjeSerwer.Odbierz(); // aktualny login uzytkownika

            // sprawdzam czy uzytkownik o nowo podanym loginie juz istnieje (roznym niz aktualny login)
            bool czyMogeEdytowac = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == uzytkownikOdKlienta.Login && u.Login != login);

            if (!czyMogeEdytowac)
            {
                var uzytkownikZBazy = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
                uzytkownikZBazy.Imie = uzytkownikOdKlienta.Imie;
                uzytkownikZBazy.Nazwisko = uzytkownikOdKlienta.Nazwisko;
                uzytkownikZBazy.Login = uzytkownikOdKlienta.Login;
                uzytkownikZBazy.Email = uzytkownikOdKlienta.Email;
                uzytkownikZBazy.Haslo = uzytkownikOdKlienta.Haslo;
                uzytkownikZBazy.Data_ur = uzytkownikOdKlienta.Data_ur;
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("edytowano");
            }
            else
            {
                string komunikat = "Uzytkownik o podanym loginie juz istnieje! Prosze podac inny login.";
                OperacjeSerwer.Wyslij(komunikat);
            }
        }

        public static void Uzytkownicy()
        {
            string login = OperacjeSerwer.Odbierz();

            var uzytkownicy = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login != login).ToList();
            string uzytkownicySerialized = JsonConvert.SerializeObject(uzytkownicy, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(uzytkownicySerialized);
        }

        public static void AwansUzytkownika()
        {
            string login = OperacjeSerwer.Odbierz();

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);

            if (uzytkownik.Uprawnienia == "uzytkownik")
            {
                uzytkownik.Uprawnienia = "admin";
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("awansowano");
            }
            else
            {
                string komunikat = "Uzytkownik juz jest administratorem!";
                OperacjeSerwer.Wyslij(komunikat);
            }
        }

        public static void ZdegradowanieUzytkownika()
        {
            string login = OperacjeSerwer.Odbierz();

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);

            if (uzytkownik.Uprawnienia == "admin")
            {
                uzytkownik.Uprawnienia = "uzytkownik";
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("zdegradowano");
            }
            else
            {
                string komunikat = "Uzytkownik jest juz zwyklym uzytkownikiem!";
                OperacjeSerwer.Wyslij(komunikat);
            }
        }
    }
}
