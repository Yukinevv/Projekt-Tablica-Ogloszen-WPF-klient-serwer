using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BibliotekaEncje.Encje;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace Serwer
{
    public class OperacjeSerwer
    {
        private static Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

        private static Socket gniazdoPolaczenia;

        private static string Odbierz()
        {
            var buf = new byte[2048];
            int received = gniazdoPolaczenia.Receive(buf, SocketFlags.None); // otrzymuje wstepna informacje od klienta
            if (received == 0)
            {
                MessageBox.Show("Blad przy otrzymaniu informacji");
                return "";
            }
            var data = new byte[received];
            Array.Copy(buf, data, received);
            string odKlienta = Encoding.ASCII.GetString(data);
            return odKlienta;
        }

        private static void Wyslij(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            gniazdoPolaczenia.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public static async void SerwerOperacje(object listbox)
        {
            EndPoint serverAddress = new IPEndPoint(IPAddress.Any, 11111);
            serverSocket.Bind(serverAddress);
            serverSocket.Listen(5);

            gniazdoPolaczenia = serverSocket.Accept();
            //await Task<Socket>.Factory.StartNew(serverSocket.Accept);

            ListBox ListBox1 = listbox as ListBox;
            ListBox1.Items.Add(gniazdoPolaczenia.RemoteEndPoint);

            await Task.Factory.StartNew(() =>
            {
                while (1 == 1)
                {
                    if (gniazdoPolaczenia.Connected)
                    {
                        string odKlienta = Odbierz();

                        if (odKlienta == "REJESTRACJA")
                        {
                            string zserializowanyObiekt = Odbierz();
                            var obiekt = JsonConvert.DeserializeObject<Uzytkownik>(zserializowanyObiekt);

                            using (var context = new MyDbContext())
                            {
                                context.Uzytkownicy.Add(obiekt);
                                context.SaveChanges();
                            }
                        }
                        else if (odKlienta == "LOGOWANIE")
                        {
                            string login = Odbierz();
                            Wyslij("OK");
                            string haslo = Odbierz();

                            using (var context = new MyDbContext())
                            {
                                bool czyZalogowac = context.Uzytkownicy.Any(u => u.Login == login && u.Haslo == haslo);
                                if (czyZalogowac)
                                {
                                    Wyslij("true");
                                }
                                else
                                {
                                    Wyslij("false");
                                }
                            }
                        }
                        else if (odKlienta == "CZY ADMIN")
                        {
                            string login = Odbierz();
                            using (var context = new MyDbContext())
                            {
                                bool czyAdmin = context.Uzytkownicy.Any(u => u.Login == login && u.Uprawnienia == "admin");
                                if (czyAdmin)
                                {
                                    Wyslij("admin");
                                }
                                else
                                {
                                    Wyslij("nie admin");
                                }
                            }
                        }
                        else if (odKlienta == "KATEGORIE")
                        {
                            using (var context = new MyDbContext())
                            {
                                var kategorie = context.Kategorie.ToList();
                                string katSerialized = JsonConvert.SerializeObject(kategorie);
                                Wyslij(katSerialized);
                            }
                        }
                        else if (odKlienta == "DODANIE KATEGORII")
                        {
                            string kategoriaSerialized = Odbierz();
                            var kategoriaOdKlienta = JsonConvert.DeserializeObject<Kategoria>(kategoriaSerialized);
                            string login = Odbierz();

                            using (var context = new MyDbContext())
                            {
                                bool czyDodac = context.Kategorie.Any(k => k.Nazwa == kategoriaOdKlienta.Nazwa);
                                if (!czyDodac)
                                {
                                    int idUzytkownika = context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
                                    var nowaKategoria = new Kategoria()
                                    {
                                        Nazwa = kategoriaOdKlienta.Nazwa,
                                        Data_utw = DateTime.Now,
                                        UzytkownikId = idUzytkownika
                                    };
                                    context.Kategorie.Add(nowaKategoria);
                                    context.SaveChanges();

                                    Wyslij("Dodano");
                                }
                                else
                                {
                                    string komunikat = "Kategoria o podanej nazwie juz istnieje! Prosze podac inna nazwe.";
                                    Wyslij(komunikat);
                                }             
                            }
                        }
                        else if (odKlienta == "USUNIECIE KATEGORII")
                        {
                            string nazwaKategorii = Odbierz();

                            using (var context = new MyDbContext())
                            {
                                // sprawdzam czy kategoria o podanej przez klienta nazwie znajduje sie w bazie
                                bool czyKategoriaIstnieje = context.Kategorie.Any(k => k.Nazwa == nazwaKategorii);

                                if (czyKategoriaIstnieje)
                                {
                                    int idKategorii = context.Kategorie.Where(k => k.Nazwa == nazwaKategorii).Select(k => k.Id).FirstOrDefault();
                                    // sprawdzam czy w usuwanej kategorii znajduja sie jakies ogloszenia, jezeli nie to usuwam kategorie
                                    bool czyMogeUsunac = context.OgloszeniaKategorie.Any(ok => ok.KategoriaId == idKategorii);

                                    if (!czyMogeUsunac)
                                    {
                                        var kategoria = context.Kategorie.Where(k => k.Nazwa == nazwaKategorii).FirstOrDefault();
                                        context.Kategorie.Remove(kategoria);
                                        context.SaveChanges();

                                        Wyslij("usunieto");
                                    }
                                    else
                                    {
                                        Wyslij("nie usunieto");
                                    }
                                }
                                else
                                {
                                    Wyslij("nie ma takiej kategorii");
                                } 
                            }
                        }
                        else if (odKlienta == "OGLOSZENIA")
                        {
                            string wiadomosc = Odbierz();
                            int idKategorii = int.Parse(wiadomosc);
                            using (var context = new MyDbContext())
                            {
                                var pomocnicza = context.OgloszeniaKategorie.Where(ok => ok.KategoriaId == idKategorii).ToList();
                                var ogloszenia = new List<Ogloszenie>();
                                foreach (var oglkat in pomocnicza)
                                {
                                    var ogl = context.Ogloszenia.Where(o => o.Id == oglkat.OgloszenieId);
                                    ogloszenia.AddRange(ogl);
                                }

                                string oglSerialized = JsonConvert.SerializeObject(ogloszenia, Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                                Wyslij(oglSerialized);
                            }
                        }
                        else if (odKlienta == "DODANIE OGLOSZENIA")
                        {
                            string login = Odbierz();
                            using (var context = new MyDbContext())
                            {
                                int idUzytkownika = context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();

                                // dodanie samego ogloszenia
                                string zserializowanyObiekt = Odbierz();
                                var ogloszenie = JsonConvert.DeserializeObject<Ogloszenie>(zserializowanyObiekt);
                                ogloszenie.UzytkownikId = idUzytkownika;

                                context.Ogloszenia.Add(ogloszenie);
                                context.SaveChanges();

                                Wyslij("Dodano");

                                // dodanie relacji do tabeli OgloszeniaKategorie
                                int idOgloszenia = context.Ogloszenia.OrderBy(o => o.Id).Select(o => o.Id).LastOrDefault();

                                string nazwyKategoriiSerialized = Odbierz();
                                var nazwyKategorii = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);
                                var idKategorii = new List<int>();

                                foreach (var item in nazwyKategorii)
                                {
                                    idKategorii.Add(context.Kategorie.Where(k => k.Nazwa == item).Select(k => k.Id).FirstOrDefault());
                                }
                                foreach (var item in idKategorii)
                                {
                                    context.OgloszeniaKategorie.Add(new OgloszenieKategoria() { OgloszenieId = idOgloszenia, KategoriaId = item });
                                }
                                context.SaveChanges();                    
                            }
                            Wyslij("zakonczono dodawanie");
                        }
                        else if (odKlienta == "USUNIECIE OGLOSZENIA")
                        {
                            string wiadomosc = Odbierz();
                            int idOgloszenia = int.Parse(wiadomosc);
                            using (var context = new MyDbContext())
                            {
                                // usuniecie samego ogloszenia
                                var ogloszenie = context.Ogloszenia.Where(o => o.Id == idOgloszenia).FirstOrDefault();
                                context.Ogloszenia.Remove(ogloszenie);

                                // usuniecie relacji pomiedzy Ogloszeniami a Kategoriami
                                var relacje = context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia);
                                context.OgloszeniaKategorie.RemoveRange(relacje);
                                context.SaveChanges();
                            }
                            Wyslij("Usunieto");
                        }
                        else if (odKlienta == "EDYCJA OGLOSZENIA")
                        {
                            string oglSeralized = Odbierz();
                            var ogloszenieOdKlienta = JsonConvert.DeserializeObject<Ogloszenie>(oglSeralized);

                            using (var context = new MyDbContext())
                            {
                                var ogloszenie = context.Ogloszenia.Where(o => o.Id == ogloszenieOdKlienta.Id).FirstOrDefault();
                                ogloszenie.Tytul = ogloszenieOdKlienta.Tytul;
                                ogloszenie.Data_ed = DateTime.Now;
                                ogloszenie.Tresc = ogloszenieOdKlienta.Tresc;
                                context.SaveChanges();
                            }
                            Wyslij("zedytowano ogloszenie");
                        }
                        else if (odKlienta == "CZY MOZE EDYTOWAC")
                        {
                            string login = Odbierz();
                            Wyslij("OK");
                            string wiadomosc = Odbierz();
                            Debug.WriteLine("Id uzytkownika wybranego ogloszenia = " + wiadomosc);
                            int idUzytkownikaWybranegoOgloszenia = int.Parse(wiadomosc);

                            using (var context = new MyDbContext())
                            {
                                int idUzytkownikZalogowanego = context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
                                bool czyAdmin = context.Uzytkownicy.Where(u => u.Id == idUzytkownikZalogowanego && u.Uprawnienia == "admin").Any();
                                if (idUzytkownikZalogowanego == idUzytkownikaWybranegoOgloszenia || czyAdmin == true)
                                {
                                    Wyslij("TAK");
                                }
                                else
                                {
                                    Wyslij("NIE");
                                }
                            }
                        }
                        else
                        {
                            gniazdoPolaczenia.Close();
                            MessageBox.Show("Nieznane rzadanie wyslania od klienta!");
                            //ListBox1.Items.Remove(gniazdoPolaczenia.RemoteEndPoint);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Klient NIE jest podlaczony! Wylaczam serwer.");
                        serverSocket.Close();
                        break;
                    }
                }
            });
        }
    }
}
