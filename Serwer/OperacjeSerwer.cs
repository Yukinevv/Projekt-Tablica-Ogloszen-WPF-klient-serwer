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
                        else if (odKlienta == "KATEGORIE")
                        {
                            using (var context = new MyDbContext())
                            {
                                var kategorie = context.Kategorie.ToList();
                                string katSerialized = JsonConvert.SerializeObject(kategorie);
                                Wyslij(katSerialized);
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
                            try
                            {
                                string login = Odbierz();
                                using (var context = new MyDbContext())
                                {
                                    int idUzytkownika = context.Uzytkownicy.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefault();
                                    Wyslij(idUzytkownika.ToString());

                                    // dodanie samego ogloszenia
                                    string zserializowanyObiekt = Odbierz();
                                    var ogloszenie = JsonConvert.DeserializeObject<Ogloszenie>(zserializowanyObiekt);
                                    bool czyTytulUnikalny = context.Ogloszenia.Any(o => o.Tytul == ogloszenie.Tytul);
                                    if (!czyTytulUnikalny)
                                    {
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

                                        Wyslij("zakonczono dodawanie");
                                    }
                                    else
                                    {
                                        string komunikat = "Ogloszenie o podanym tytule juz istnieje. Prosze nadac inny tytul.";
                                        Wyslij(komunikat);    
                                    }                       
                                }
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(err.Message);
                            }     
                        }
                        else if (odKlienta == "USUNIECIE OGLOSZENIA")
                        {
                            string tytulOgloszenia = Odbierz();
                            using (var context = new MyDbContext())
                            {
                                // usuniecie samego ogloszenia
                                var ogloszenie = context.Ogloszenia.Where(o => o.Tytul == tytulOgloszenia).FirstOrDefault();
                                context.Ogloszenia.Remove(ogloszenie);

                                // usuniecie relacji pomiedzy Ogloszeniami a Kategoriami
                                var idOgloszenia = context.Ogloszenia.Where(o => o.Tytul == tytulOgloszenia).Select(o => o.Id).FirstOrDefault();
                                var relacje = context.OgloszeniaKategorie.Where(ok => ok.OgloszenieId == idOgloszenia);
                                context.OgloszeniaKategorie.RemoveRange(relacje);
                                context.SaveChanges();

                                Wyslij("Usunieto");
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
