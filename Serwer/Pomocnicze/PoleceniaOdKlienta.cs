using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace Serwer
{
    public class PoleceniaOdKlienta
    {
        public static void Rejestracja(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);

            string zserializowanyObiekt = OperacjeSerwer.odKlienta;
            var obiekt = JsonConvert.DeserializeObject<Uzytkownik>(zserializowanyObiekt);

            bool czyMogeDodac = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == obiekt.Login);

            if (!czyMogeDodac)
            {
                DataBaseLocator.Context.Uzytkownicy.Add(obiekt);
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("zarejestrowano", current);
            }
            else
            {
                string komunikat = "Uzytkownik o podanym loginie juz istnieje!";
                OperacjeSerwer.Wyslij(komunikat, current);
            }
        }

        public static void Logowanie(byte[] buffer, int BUFFER_SIZE, Socket current, List<string> zalogowaniKlienciLoginy)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;
            OperacjeSerwer.Wyslij("OK", current);

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string haslo = OperacjeSerwer.odKlienta;

            bool czyJestJuzZalogowany = zalogowaniKlienciLoginy.Contains(login);

            bool czyJestWBazie = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == login && u.Haslo == haslo);
            if (czyJestWBazie && !czyJestJuzZalogowany)
            {
                OperacjeSerwer.Wyslij("zaloguj", current);
            }
            else if (czyJestWBazie && czyJestJuzZalogowany)
            {
                OperacjeSerwer.Wyslij("jest juz zalogowany", current);
            }
            else
            {
                OperacjeSerwer.Wyslij("nie ma w bazie", current);
            }
        }

        public static void DodajLoginDoListyZalogowanych(byte[] buffer, int BUFFER_SIZE, Socket current, List<string> zalogowaniKlienciLoginy)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;
            zalogowaniKlienciLoginy.Add(login);
            OperacjeSerwer.Wyslij("dodano", current);
        }

        public static void Wyloguj(byte[] buffer, int BUFFER_SIZE, Socket current, List<string> zalogowaniKlienciLoginy)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;
            zalogowaniKlienciLoginy.Remove(login);
        }

        public static void CzyAdmin(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            bool czyAdmin = DataBaseLocator.Context.Uzytkownicy.Any(u => u.Login == login && u.Uprawnienia == "admin");
            if (czyAdmin)
            {
                OperacjeSerwer.Wyslij("admin", current);
            }
            else
            {
                OperacjeSerwer.Wyslij("nie admin", current);
            }
        }

        public static void Kategorie(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            var kategorie = DataBaseLocator.Context.Kategorie.ToList();
            string katSerialized = JsonConvert.SerializeObject(kategorie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(katSerialized, current);
        }

        public static void WybraneNazwyKategorii(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string wiadomosc = OperacjeSerwer.odKlienta;
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
            string nazwyKategoriiSerialized = JsonConvert.SerializeObject(nazwyKategorii, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(nazwyKategoriiSerialized, current);
        }

        public static void DodanieKategorii(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string kategoriaSerialized = OperacjeSerwer.odKlienta;
            var kategoriaOdKlienta = JsonConvert.DeserializeObject<Kategoria>(kategoriaSerialized);

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string login = OperacjeSerwer.odKlienta;

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

                OperacjeSerwer.Wyslij("Dodano", current);
            }
            else
            {
                string komunikat = "Kategoria o podanej nazwie juz istnieje! Prosze podac inna nazwe.";
                OperacjeSerwer.Wyslij(komunikat, current);
            }
        }

        public static void UsuniecieKategorii(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string nazwaKategorii = OperacjeSerwer.odKlienta;

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

                    OperacjeSerwer.Wyslij("usunieto", current);
                }
                else
                {
                    OperacjeSerwer.Wyslij("nie usunieto", current);
                }
            }
            else
            {
                OperacjeSerwer.Wyslij("nie ma takiej kategorii", current);
            }
        }

        public static void Ogloszenia(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string wiadomosc = OperacjeSerwer.odKlienta;
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
            OperacjeSerwer.Wyslij(oglSerialized, current);
        }

        public static void MojeOgloszenia(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            int idUzytkownika = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
            var ogloszenia = DataBaseLocator.Context.Ogloszenia.Where(o => o.UzytkownikId == idUzytkownika).ToList();
            string oglSerialized = JsonConvert.SerializeObject(ogloszenia, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(oglSerialized, current);
        }

        public static void DodanieOgloszenia(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            int idUzytkownika = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();

            // dodanie samego ogloszenia
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string zserializowanyObiekt = OperacjeSerwer.odKlienta;
            var ogloszenie = JsonConvert.DeserializeObject<Ogloszenie>(zserializowanyObiekt);
            ogloszenie.UzytkownikId = idUzytkownika;

            DataBaseLocator.Context.Ogloszenia.Add(ogloszenie);
            DataBaseLocator.Context.SaveChanges();

            OperacjeSerwer.Wyslij("dodano ogloszenie", current);

            // dodanie relacji do tabeli OgloszeniaKategorie
            int idOgloszenia = DataBaseLocator.Context.Ogloszenia.OrderBy(o => o.Id).Select(o => o.Id).LastOrDefault();

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string nazwyKategoriiSerialized = OperacjeSerwer.odKlienta;
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

            OperacjeSerwer.Wyslij("zakonczono dodawanie", current);
        }

        public static void UsuniecieOgloszenia(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string wiadomosc = OperacjeSerwer.odKlienta;
            int idOgloszenia = int.Parse(wiadomosc);

            // usuniecie samego ogloszenia
            var ogloszenie = DataBaseLocator.Context.Ogloszenia.Where(o => o.Id == idOgloszenia).FirstOrDefault();
            DataBaseLocator.Context.Ogloszenia.Remove(ogloszenie);

            // usuniecie relacji pomiedzy Ogloszeniami a Kategoriami
            var relacje = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia);
            DataBaseLocator.Context.OgloszeniaKategorie.RemoveRange(relacje);
            DataBaseLocator.Context.SaveChanges();

            OperacjeSerwer.Wyslij("Usunieto", current);
        }

        public static void EdycjaOgloszenia(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string oglSeralized = OperacjeSerwer.odKlienta;
            var ogloszenieOdKlienta = JsonConvert.DeserializeObject<Ogloszenie>(oglSeralized);

            var ogloszenie = DataBaseLocator.Context.Ogloszenia.Where(o => o.Id == ogloszenieOdKlienta.Id).FirstOrDefault();
            ogloszenie.Tytul = ogloszenieOdKlienta.Tytul;
            ogloszenie.Data_ed = DateTime.Now;
            ogloszenie.Tresc = ogloszenieOdKlienta.Tresc;
            DataBaseLocator.Context.SaveChanges();

            OperacjeSerwer.Wyslij("zedytowano ogloszenie", current);

            // edycja relacji w tabeli OgloszeniaKategorie
            // usuniecie wczesniejszych relacji
            int idOgloszenia = ogloszenie.Id;
            var relacje = DataBaseLocator.Context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia).ToList();
            DataBaseLocator.Context.OgloszeniaKategorie.RemoveRange(relacje);
            DataBaseLocator.Context.SaveChanges();

            // dodanie nowych relacji
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string nazwyKategoriiSerialized = OperacjeSerwer.odKlienta;
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

            OperacjeSerwer.Wyslij("zakonczono edycje", current);
        }

        public static void CzyMozeEdytowac(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;
            OperacjeSerwer.Wyslij("OK", current);

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string wiadomosc = OperacjeSerwer.odKlienta;
            Debug.WriteLine("Id uzytkownika wybranego ogloszenia = " + wiadomosc);
            int idUzytkownikaWybranegoOgloszenia = int.Parse(wiadomosc);

            int idUzytkownikZalogowanego = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
            bool czyAdmin = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Id == idUzytkownikZalogowanego && u.Uprawnienia == "admin").Any();
            if (idUzytkownikZalogowanego == idUzytkownikaWybranegoOgloszenia || czyAdmin == true)
            {
                OperacjeSerwer.Wyslij("TAK", current);
            }
            else
            {
                OperacjeSerwer.Wyslij("NIE", current);
            }
        }

        public static void MojeDane(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(uzytkownikSerialized, current);
        }

        public static void EdycjaDanychUzytkownika(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string uzytkownikOdKlientaSerialized = OperacjeSerwer.odKlienta;
            var uzytkownikOdKlienta = JsonConvert.DeserializeObject<Uzytkownik>(uzytkownikOdKlientaSerialized);

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            Thread.Sleep(100);
            string login = OperacjeSerwer.odKlienta; // aktualny login uzytkownika

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

                OperacjeSerwer.Wyslij("edytowano", current);
            }
            else
            {
                string komunikat = "Uzytkownik o podanym loginie juz istnieje! Prosze podac inny login.";
                OperacjeSerwer.Wyslij(komunikat, current);
            }
        }

        public static void Uzytkownicy(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            var uzytkownicy = DataBaseLocator.Context.Uzytkownicy.Where(u => u.Login != login).ToList();
            string uzytkownicySerialized = JsonConvert.SerializeObject(uzytkownicy, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeSerwer.Wyslij(uzytkownicySerialized, current);
        }

        public static void AwansUzytkownika(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);

            if (uzytkownik.Uprawnienia == "uzytkownik")
            {
                uzytkownik.Uprawnienia = "admin";
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("awansowano", current);
            }
            else
            {
                string komunikat = "Uzytkownik juz jest administratorem!";
                OperacjeSerwer.Wyslij(komunikat, current);
            }
        }

        public static void ZdegradowanieUzytkownika(byte[] buffer, int BUFFER_SIZE, Socket current)
        {
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, OperacjeSerwer.Odbierz, current);
            string login = OperacjeSerwer.odKlienta;

            var uzytkownik = DataBaseLocator.Context.Uzytkownicy.FirstOrDefault(u => u.Login == login);

            if (uzytkownik.Uprawnienia == "admin")
            {
                uzytkownik.Uprawnienia = "uzytkownik";
                DataBaseLocator.Context.SaveChanges();

                OperacjeSerwer.Wyslij("zdegradowano", current);
            }
            else
            {
                string komunikat = "Uzytkownik jest juz zwyklym uzytkownikiem!";
                OperacjeSerwer.Wyslij(komunikat, current);
            }
        }

        public static void OdlaczenieKlienta(Socket current, List<Socket> clientSockets)
        {
            MainWindowModelWidoku.ListBoxPolaczeniKlienciModelWidoku.Remove(current.RemoteEndPoint.ToString());
            MainWindow.TextBoxLogs.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.TextBoxLogs.Text += "Klient o adresie IP: " + current.RemoteEndPoint.ToString() + " zakonczyl polaczenie.\n";
            }));

            current.Close();
            clientSockets.Remove(current);
        }

        public static void NierozpoznaneRzadanie(Socket current, List<Socket> clientSockets)
        {
            if (!MainWindowModelWidoku.czyWylaczSerwerWcisniety)
            {
                //MessageBox.Show("Rzadanie " + odKlienta + ". Zamykam polaczenie.");
                MessageBox.Show("Nierozpoznane rzadanie od klienta! Zamykam polaczenie.");
                MainWindowModelWidoku.ListBoxPolaczeniKlienciModelWidoku.Remove(current.RemoteEndPoint.ToString());
                MainWindow.TextBoxLogs.Dispatcher.Invoke(new Action(() =>
                {
                    MainWindow.TextBoxLogs.Text += "Klient o adresie IP: " + current.RemoteEndPoint.ToString() + " zakonczyl polaczenie.\n";
                }));

                current.Close();
                clientSockets.Remove(current);
            }
        }
    }
}
